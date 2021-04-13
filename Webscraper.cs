using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Amicitia.github.io.PageCreator;
using System.Globalization;

namespace Amicitia.github.io
{

    class Webscraper
    {
        public static List<Tuple<string, int>> gameList = new List<Tuple<string, int>>() {
            new Tuple<string, int>("P3FES", 8502),
            new Tuple<string, int>("P3P", 8583),
            new Tuple<string, int>("P3D", 8747),
            new Tuple<string, int>("P4", 8761),
            new Tuple<string, int>("P4G", 8263),
            new Tuple<string, int>("P4D", 8769), 
            new Tuple<string, int>("P5", 7545),
            new Tuple<string, int>("P5R", 8464),
            //P5D
            new Tuple<string, int>("P5S", 9099)
            //SMT3
            //CFB
            //P4AU
        };
        public static List<Post> Posts = new List<Post>();

        public static void GBUpdateTSVs(string indexPath)
        {
            // Load Existing TSVs
            Posts = PageCreator.Post.Get(indexPath);

            // For each game on Gamebanana...
            foreach (var game in gameList)
            {
                // Load Page
                using (var client = new WebClient())
                {
                    Console.WriteLine($"Loading {game.Item1} page on Gamebanana...");
                    string gamePage = client.DownloadString($"https://gamebanana.com/games/{game.Item2}");
                    // Hacky workaround: Loads from local dir if found
                    if (File.Exists($"C:\\Users\\Ryan\\Downloads\\GB\\{game.Item1}.html"))
                        gamePage = client.DownloadString($"C:\\Users\\Ryan\\Downloads\\GB\\{game.Item1}.html");
                    HtmlAgilityPack.HtmlDocument gameDoc = new HtmlAgilityPack.HtmlDocument();
                    gameDoc.LoadHtml(gamePage);

                    // TODO: While Load More button exists, keep pressing it.
                    // Until then, this only fetches the 20 most recent GB submissions.

                    // Get post list from page
                    HtmlNodeCollection records = gameDoc.DocumentNode.SelectNodes("//records//record");
                    if (records != null)
                    {
                        foreach (var record in records)
                        {
                            // Make sure it is a submission and not a section category or audio related
                            if (record.ChildNodes[2].ChildNodes[1].ChildNodes.Count() > 1 && !record.ChildNodes[1].InnerHtml.Contains("audio controls"))
                            {
                                // Get post data from preview
                                Post post = new Post();

                                // Get type of post
                                string spriteicon = record.ChildNodes[4].ChildNodes[1].Attributes["class"].Value.Replace("SubmissionType ", "");
                                if (spriteicon == "Skin" || spriteicon == "Gamefile" || spriteicon == "Gui" || spriteicon == "Sounds" || spriteicon == "Wip" || spriteicon == "Effect" || spriteicon == "Map")
                                {
                                    post.Type = "mod";
                                    post.Tags = new List<string>() { spriteicon };
                                }
                                else if (spriteicon == "Tool")
                                {
                                    post.Type = "tool";
                                    post.Tags = new List<string>() { spriteicon };
                                }
                                else if (spriteicon == "Tutorial")
                                {
                                    post.Type = "guide";
                                    post.Tags = new List<string>() { spriteicon };
                                }

                                // Skip post if question or request
                                if (post.Type != "")
                                {
                                    post.Games = new List<string>() { game.Item1 };
                                    post.Title = record.ChildNodes[2].ChildNodes[1].ChildNodes[1].ChildNodes[0].InnerText;
                                    post.Authors = new List<string>() { record.ChildNodes[7].ChildNodes[3].ChildNodes[3].InnerText.Replace("\n", "") };
                                    try
                                    {
                                        post.DownloadURL = record.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["href"].Value;
                                    }
                                    catch
                                    {
                                        post.DownloadURL = record.ChildNodes[2].ChildNodes[1].ChildNodes[1].Attributes["href"].Value;
                                    }
                                    post.ID = Convert.ToInt32(post.DownloadURL.Split('/').Last());
                                    string date = record.ChildNodes[3].ChildNodes[1].ChildNodes[2].Attributes["title"].Value.Split('@')[0].Trim();
                                    string format = "MMM dd yyyy";
                                    if (date.Split(' ')[1].Length < 2)
                                        format = "MMM d yyyy";
                                    post.Date = DateTime.ParseExact(date, format, CultureInfo.CreateSpecificCulture("en-US")).ToString("d", DateTimeFormatInfo.InvariantInfo);
                                    post.DownloadText = "View on Gamebanana";
                                    post.DownloadIcon = "fa fa-pepper-hot";

                                    Console.WriteLine($"Loading {post.Title} from Gamebanana...");
                                    // Download post page and get rest of required data
                                    string postPage = client.DownloadString(post.DownloadURL);
                                    HtmlAgilityPack.HtmlDocument postDoc = new HtmlAgilityPack.HtmlDocument();
                                    postDoc.LoadHtml(postPage);

                                    var screenshots = postDoc.GetElementbyId("ScreenshotsModule");
                                    if (screenshots != null)
                                        post.EmbedURL = screenshots.ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["data-src"].Value;

                                    post.Description = postDoc.GetElementbyId("ItemProfileModule").ChildNodes[1].ChildNodes[0].InnerText;

                                    //if (post.Description == "") // Get first paragraph if no short description
                                    //post.Description = postDoc.DocumentNode.SelectSingleNode("//[@id='ItemProfileModule']//div//article").InnerText.Replace("<br>\n<br>", "|").Split('|')[0];

                                    var updates = postDoc.GetElementbyId("UpdatesModule");
                                    if (updates != null)
                                    {
                                        post.UpdateText = "<b>Updates</b>: ";
                                        foreach (var node in updates.SelectNodes("//code"))
                                            post.UpdateText += "- " + node.InnerText.Replace("\n", "").Replace("\t", "") + "<br>";
                                    }

                                    // If post is already accounted for in tsv...
                                    if (Posts.Any(x => x.DownloadURL.Equals(post.DownloadURL)))
                                    {
                                        // Remove existing post from list
                                        Post tempPost = Posts.First(x => x.DownloadURL.Equals(post.DownloadURL));
                                        int index = Posts.IndexOf(tempPost);
                                        Posts.Remove(tempPost);
                                        // ... Update post and add back to list
                                        Posts.Insert(index, post);
                                    }
                                    else
                                        Posts.Add(post);
                                }
                            }
                        }
                    }
                }
            }
            // Save new TSVs
            Console.WriteLine("Updating TSV files with Gamebanana posts...");
            List<Post> Mods = Posts.Where(x => x.Type.ToUpper().Equals("MOD")).ToList();
            List<Post> Tools = Posts.Where(x => x.Type.ToUpper().Equals("TOOL")).ToList();
            List<Post> Guides = Posts.Where(x => x.Type.ToUpper().Equals("GUIDE")).ToList();
            List<string> lines = new List<string>();
            // Mods
            foreach (var post in Mods)
                lines.Add($"{post.ID}\t{post.Title}\t{String.Join(", ", post.Games)}\t{String.Join(", ", post.Authors)}\t1\t{post.Date}\t{String.Join(", ", post.Tags)}\t{post.Description}\t{post.UpdateText}\t{post.EmbedURL}\t{post.DownloadURL}\t{post.DownloadText}\t{post.DownloadIcon}\t{post.SourceURL}\t{post.ThreadURL}\t{post.DownloadURL2}\t{post.DownloadText2}\t{post.DownloadIcon2}\t{post.GuideURL}\t{post.GuideText}");
            File.WriteAllLines($"{indexPath}//db//mods.tsv", lines.ToArray());
            // Tools
            lines = new List<string>();
            foreach (var post in Tools)
                lines.Add($"{post.ID}\t{post.Title}\t{String.Join(", ", post.Games)}\t{String.Join(", ", post.Authors)}\t1\t{post.Date}\t{String.Join(", ", post.Tags)}\t{post.Description}\t{post.UpdateText}\t{post.EmbedURL}\t{post.DownloadURL}\t{post.DownloadText}\t{post.DownloadIcon}\t{post.SourceURL}\t{post.ThreadURL}\t{post.DownloadURL2}\t{post.DownloadText2}\t{post.DownloadIcon2}\t{post.GuideURL}\t{post.GuideText}");
            File.WriteAllLines($"{indexPath}//db//tools.tsv", lines.ToArray());
            // Guides
            lines = new List<string>();
            foreach (var post in Guides)
                lines.Add($"{post.ID}\t{post.Title}\t{String.Join(", ", post.Games)}\t{String.Join(", ", post.Authors)}\t1\t{post.Date}\t{String.Join(", ", post.Tags)}\t{post.Description}\t{post.UpdateText}\t{post.EmbedURL}\t{post.DownloadURL}\t{post.DownloadText}\t{post.DownloadIcon}\t{post.SourceURL}\t{post.ThreadURL}\t{post.DownloadURL2}\t{post.DownloadText2}\t{post.DownloadIcon2}\t{post.GuideURL}\t{post.GuideText}");
            File.WriteAllLines($"{indexPath}//db//guides.tsv", lines.ToArray());
        }
    }
}
