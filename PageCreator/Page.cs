﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Amicitia.github.io.Program;

namespace Amicitia.github.io.PageCreator
{
    public class Page
    {
        public static void DeleteExisting(string indexPath)
        {
            //Remove each of these folders (and their files) from exe directory
            string[] thingsToDelete = new string[] { "author", "cheats", "game", "guides", "index", "mods", "post", "tag", "tools" };

            foreach (var thing in thingsToDelete)
            {
                foreach (var file in Directory.GetFiles(indexPath, "*.*", SearchOption.AllDirectories))
                {
                    if (file.EndsWith(thing + ".html"))
                        File.Delete(file);
                }
                foreach (var dir in Directory.GetDirectories(indexPath, "*", SearchOption.AllDirectories))
                {
                    if (Path.GetFileName(dir) == thing)
                        Directory.Delete(dir, true);
                }
            }
        }

        public static void CreateSingle(List<Post> posts)
        {
            //Create single-post pages for hyperlinks
            foreach (var post in posts)
            {
                List<Post> singlePost = new List<Post>() { post };
                CreateHtml(singlePost, $"post\\{post.Id}");
            }
        }

        public static void Create(string content, string url, int pageNumber, bool morePages)
        {
            //Html Head Tag contents
            string html = Properties.Resources.IndexHeader;
            //Top of page, site navigation
            string pageName = "";

            foreach (var split in url.Replace(".html", "").Split('\\').Reverse())
            {
                if (gameList.Any(g => g.Item1.Equals(split)))
                {
                    html = html.Replace($"value=\"{split.ToUpper()}\">", $"value=\"{split.ToUpper()}\" selected>");
                    pageName += gameList.Single(x => x.Item1.Equals(split)).Item2;
                }
                else if (split == "mods" || split == "tools" || split == "guides" || split == "cheats")
                {
                    html = html.Replace($"value=\"{split}\">", $"value=\"{split}\" selected>");
                    pageName += $" {split} ";
                }
            }
            //Change page title
            if (!String.IsNullOrEmpty(pageName))
                html = html.Replace("Amicitia Mods</title>", $"Amicitia - {pageName}</title>");

            //Closing header div before content
            html += Properties.Resources.IndexSidebar;

            // Set up for pagination and ref link depth
            int depth = url.Count(c => c == '\\');
            string url2 = url.Replace(".html", "");
            if (depth == 1)
                url2 = url2.Replace($"{url}\\{url}", $"{url}");
            // Table for pagination
            string pagination = "<center><nav class=\"pagination\" role=\"navigation\"><div class=\"nav-links\">";

            // Previous Page
            if (pageNumber > 1)
            {
                if (pageNumber == 2)
                    pagination += $"<a class=\"page-numbers\" href=\"https:\\\\amicitia.github.io\\{url2.Replace($"\\{pageNumber}", "")}\">1</a>";
                else
                    pagination += $"<a class=\"page-numbers\" href=\"https:\\\\amicitia.github.io\\{url2.Replace($"\\{pageNumber}", $"\\{pageNumber - 1}")}\">{pageNumber - 1}</a>";
            }
            // Current Page
            pagination += $"<span aria-current=\"page\" class=\"page-numbers current\">{pageNumber}</span>";
            // Next Page
            if (morePages)
            {
                if (pageNumber == 1)
                    pagination += $"<a class=\"page-numbers\" href=\"https:\\\\amicitia.github.io\\{url2}\\2\">2</a>";
                else
                    pagination += $"<a class=\"page-numbers\" href=\"https:\\\\amicitia.github.io\\{url2.Replace($"\\{pageNumber}", $"\\{pageNumber + 1}")}\">{pageNumber + 1}</a>";
            }
            // End pagination table
            pagination += "</div></nav></center>";

            // Append content, navigation and footer to content
            html += content; // Body Content
            html += Properties.Resources.IndexFooter; // Footer
            html = html.Replace("<!--Pagination-->", pagination); // Pagination

            // Replace links based on depth
            if (depth == 1)
            {
                html = html.Replace("\"css", "\"../css");
                html = html.Replace("\"js", "\"../js");
                html = html.Replace("\"images", "\"../images");
            }
            else if (depth == 2)
            {
                html = html.Replace("\"../", "\"../../");
                html = html.Replace("\"css", "\"../../css");
                html = html.Replace("\"js", "\"../../js");
                html = html.Replace("\"images", "\"../../images");
            }
            else if (depth == 3)
            {
                html = html.Replace("\"../../", "\"../../../");
                html = html.Replace("\"css", "\"../../../css");
                html = html.Replace("\"js", "\"../../../js");
                html = html.Replace("\"images", "\"../../../images");
            }

            // Create page
            string htmlPath = Path.Combine(indexPath, url);
            Directory.CreateDirectory(Path.GetDirectoryName(htmlPath));
            File.WriteAllText(htmlPath, html);
            Console.WriteLine(htmlPath);
        }

        internal static void FlowscriptDocs(string indexPath)
        {
            foreach (string page in new string[] { "compiling", "decompiling", "flowscript", "hookingfunctions", "importing", "messagescript" })
            {
                string content = "";
                content += Properties.Resources.IndexHeader;
                content += Properties.Resources.IndexSidebar;
                content += File.ReadAllText(Path.Combine(Path.Combine(Path.Combine(indexPath, "Templates"), "Flowscript"), page + ".html"));
                content += Properties.Resources.IndexFooter;
                content = content.Replace("\"all\">Game", "\"all\">Flowscript").Replace("\"all\">Modding Resources", "\"all\">Docs");
                File.WriteAllText(Path.Combine(Path.Combine(indexPath, "docs"), page + ".html"), content);
            }
        }

        internal static void Misc(string indexPath)
        {
            foreach (string page in new string[] { "404", "files" })
            {
                string content = "";
                content += Properties.Resources.IndexHeader;
                content += Properties.Resources.IndexSidebar;
                content += File.ReadAllText(Path.Combine(Path.Combine(Path.Combine(indexPath, "Templates"), "Misc"), page + ".html"));
                content += Properties.Resources.IndexFooter;
                if (page == "404")
                    content = content.Replace("\"all\">Game", "\"all\">404").Replace("\"all\">Modding Resources", "\"all\">Not Found");
                else if (page == "files")
                    content = content.Replace("\"all\">Game", "\"all\">Dumped").Replace("\"all\">Modding Resources", "\"all\">Files");
                File.WriteAllText(Path.Combine(indexPath, page + ".html"), content);
            }
        }

        public static void CreateHtml(List<Post> posts, string url)
        {
            string content = "";
            int pages = 1; // Complete pages so far
            int pagePosts = 0; // Posts on this page so far
            int totalPages = Convert.ToInt32(RoundUp(Convert.ToDecimal(posts.Count) / Convert.ToDecimal(maxPosts), 0)); // Total number of pages
            //For each post...
            for (int i = 0; i < posts.Count; i++)
            {
                //Start of page
                if (pagePosts == 0)
                {
                    content += Properties.Resources.PostTableHeader;
                    //Show total number of results if not a single post page
                    if (!url.Contains("\\post\\"))
                        content = content.Replace("(0 results)", $"({posts.Count} results)").Replace("Page 0/0", $"Page {pages}/{totalPages}");
                    if (posts.Count == 0) //Inform user if no posts found
                        content += $"<br><center>Sorry! No posts matching your query were found. Please check again later.</center>";
                    else if (url.Contains("p5r")) //Show Pan thank you message
                        content += "<center>Special thanks to <a href=\"https://twitter.com/regularpanties\">@regularpanties</a> for the generous donation of a 6.72 PS4<br>and a plethora of documentation that made this section possible.</center><br>";

                    bool matchFound = false; //Show more resources if post is a mod or tool
                    if (!matchFound && (url.Contains("mods") || url.Contains("game")))
                    {
                        if (url.Contains("\\p5") && !url.Contains("\\p5r") && !url.Contains("\\p5s"))
                            content += "<br><center>To learn how to run P5 mods, see <a href=\"https://shrinefox.com/guides/2019/04/19/persona-5-rpcs3-modding-guide-1-downloads-and-setup\">this guide.</a></center>";
                        else if (url.Contains("\\p5r"))
                            content += "<br><center>To learn how to install and run P5R mods, see <a href=\"https://shrinefox.com/guides/2020/09/30/modding-persona-5-royal-jp-on-ps4-fw-6-72\">this guide</a>.";
                        else if (url.Contains("\\p3fes") || url.Contains("\\p4.html") || url.Contains("\\smt3.html"))
                            content += "<br><center>To learn how to run these mods, see <a href=\"https://amicitia.github.io/post/hostfs-guide\">this guide.</a></center>";
                        else if (url.Contains("\\p4g"))
                            content += "<br><center>To learn how to mod the PC version of P4G, see <a href=\"https://gamebanana.com/tuts/13379\">this guide.</a><br>More P4G PC mods available at <a href=\"https://gamebanana.com/games/8263\">gamebanana.com</a>.</center>";
                        else
                            content += "<br><center>To learn how to use these mods, see <a href=\"https://shrinefox.com/guides/2019/04/19/persona-5-rpcs3-modding-guide-1-downloads-and-setup\">this guide.</a> Although it's focused on Persona 5, the latter half applies to other games as well.</center>";
                        matchFound = true;
                    }
                }
                pagePosts++;

                // Add content to page after header
                if (url.Contains("post"))
                    content += Post.Write(posts[i], true);
                else
                    content += Post.Write(posts[i], false);

                // Add new row if divislbe by 3
                if (pagePosts % 3 == 0)
                    content += "<br>";

                // End of page, create new page
                if (pagePosts == maxPosts || posts.Count - 1 == i)
                {
                    pagePosts = 0;
                    if (pages == 1)
                        Create(content, $"{url}.html", pages, posts.Count - (pages * maxPosts) > 0);
                    else
                        Create(content, $"{url}\\{pages}.html", pages, posts.Count - (pages * maxPosts) > 0);
                    content = "";

                    pages++;
                }
            }
        }

        public static void CreateGames(List<Post> posts)
        {
            //For each game...
            foreach (var game in gameList)
            {
                //Get games from each post
                List<Post> postsByGame = new List<Post>();
                foreach (var post in posts)
                {
                    if (post.Games.Any(x => x.ToUpper().Equals(game.Item1.ToUpper())))
                        postsByGame.Add(post);
                }
                //Create 
                CreateHtml(postsByGame, $"game\\{game.Item1}");
            }
        }

        public static void CreateAuthors(List<Post> posts)
        {
            //Get list of individual authors from all posts
            List<string> uniqueAuthors = new List<string>();
            foreach (var post in posts)
            {
                foreach (var author in post.Authors)
                    if (!uniqueAuthors.Contains(author.Trim()))
                        uniqueAuthors.Add(author.Trim());
            }

            //Create individual pages for each unique creator
            foreach (var author in uniqueAuthors)
            {
                var newpost = posts.Where(p => p.Authors.Any(x => x.Trim().Equals(author))).ToList();
                Page.CreateHtml(newpost, $"author\\{author}");
            }
        }

        public static void CreateTags(List<Post> posts)
        {
            //Get list of individual tags from all posts
            List<string> uniqueTags = new List<string>();
            foreach (var post in posts)
            {
                foreach (var tags in post.Tags)
                    if (!uniqueTags.Contains(tags.Trim()))
                        uniqueTags.Add(tags.Trim());
            }

            //Create individual pages for each unique creator
            foreach (var tag in uniqueTags)
            {
                var newpost = posts.Where(p => p.Tags.Any(x => x.Trim().Equals(tag))).ToList();
                Page.CreateHtml(newpost, $"tag\\{tag}");
            }
        }

        public static void CreateType(string type)
        {
            //Get list of all post matching type
            List<Post> typepost = posts.Where(p => p.Type.Equals(type)).ToList();

            //Create page with all posts matching type
            Page.CreateHtml(typepost, type.ToLower() + "s");
            foreach (var game in gameList)
            {
                List<Post> typepostByGame = new List<Post>();
                foreach (var post in typepost)
                {
                    if (post.Games.Any(x => x.Trim().ToUpper().Equals(game.Item1.ToUpper())))
                        typepostByGame.Add(post);
                }
                CreateHtml(typepostByGame, $"{type.ToLower()}s\\{game.Item1}");
            }
        }

        public static decimal RoundUp(decimal numero, int numDecimales)
        {
            decimal valorbase = Convert.ToDecimal(Math.Pow(10, numDecimales));
            decimal resultado = Decimal.Round(numero * 1.00000000M, numDecimales + 1, MidpointRounding.AwayFromZero) * valorbase;
            decimal valorResiduo = 10M * (resultado - Decimal.Truncate(resultado));

            if (valorResiduo > 0)
            {
                if (valorResiduo >= 5)
                {
                    var ajuste = Convert.ToDecimal(Math.Pow(10, -(numDecimales + 1)));
                    numero += ajuste;
                    return Decimal.Round(numero * 1.00000000M, numDecimales, MidpointRounding.AwayFromZero);
                }
                else
                    return Decimal.Round(numero * 1.00M, numDecimales, MidpointRounding.AwayFromZero) + 1;
            }
            else
            {
                return Decimal.Round(numero * 1.00M, numDecimales, MidpointRounding.AwayFromZero);
            }
        }

        private string[] splitUrl(string url)
        {
            Match match = Regex.Match(url, @"\:|\.(.{2,3}(?=/))"); // Regex Pattern
            if (match.Success)  // check if it has a valid match
            {
                string split = match.Groups[0].Value; // get the matched text
                int index = url.IndexOf(split);
                return new string[]
                {
            url.Substring(0, index + split.Length),
            url.Substring(index + (split.Length), url.Length - (index + split.Length))
                };
            }

            return null;
        }
    }
}
