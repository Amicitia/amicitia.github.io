using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Amicitia.github.io.PageCreator
{
    public class Post
    {
        public Post(int ID = 0, string Type = "", string Title = "", List<string> Games = null, List<string> Authors = null, float Version = 0, string Date = "", List<string> Tags = null, string Hyperlink = "", string Description = "", string EmbedURL = "", string DownloadURL = "", string DownloadText = "", string DownloadIcon = "", string DownloadURL2 = "", string DownloadText2 = "", string DownloadIcon2 = "", string UpdateText = "", string SourceURL = "", string GuideURL = "", string GuideText = "", string ThreadURL = "")
        {
            this.ID = ID;
            this.Type = Type;
            this.Title = Title;
            this.Games = Games;
            this.Authors = Authors;
            this.Version = Version;
            this.Date = Date;
            this.Tags = Tags;
            this.Hyperlink = Hyperlink;
            this.Description = Description;
            this.EmbedURL = EmbedURL;
            this.DownloadURL = DownloadURL;
            this.DownloadText = DownloadText;
            this.DownloadIcon = DownloadIcon;
            this.DownloadURL2 = DownloadURL2;
            this.DownloadText2 = DownloadText2;
            this.DownloadIcon2 = DownloadIcon2;
            this.UpdateText = UpdateText;
            this.SourceURL = SourceURL;
            this.GuideURL = GuideURL;
            this.GuideText = GuideText;
            this.ThreadURL = ThreadURL;
        }
        public int ID { get; set; } = 0;
        public string Type { get; set; } = "";
        public string Title { get; set; } = "";
        public List<string> Games { get; set; } = new List<string>();
        public List<string> Authors { get; set; } = new List<string>();
        public float Version { get; set; } = 0;
        public string Date { get; set; } = "";
        public List<string> Tags { get; set; } = new List<string>();
        public string Hyperlink { get; set; } = "";
        public string Description { get; set; } = "";
        public string EmbedURL { get; set; } = "";
        public string DownloadURL { get; set; } = "";
        public string DownloadText { get; set; } = "";
        public string DownloadIcon { get; set; } = "";
        public string DownloadURL2 { get; set; } = "";
        public string DownloadText2 { get; set; } = "";
        public string DownloadIcon2 { get; set; } = "";
        public string UpdateText { get; set; } = "";
        public string SourceURL { get; set; } = "";
        public string GuideURL { get; set; } = "";
        public string GuideText { get; set; } = "";
        public string ThreadURL { get; set; } = "";
        public static List<Post> Get(string indexPath)
        {
            List<Post> posts = new List<Post>();
            int index = 0;
            //For each TSV file...
            foreach (var tsv in Directory.GetFiles($"{indexPath}\\db").Where(x => Path.GetExtension(x).Equals(".tsv")))
            {
                string type = Path.GetFileNameWithoutExtension(tsv).Replace("s", ""); //TSV filename, nonplural
                string[] tsvFile = File.ReadAllLines(tsv);
                for (int i = 1; i < tsvFile.Length; i++)
                {
                    //Separate tabs and remove whitespace/quotation marks
                    var split = tsvFile[i].Split('\t');
                    for (int x = 0; x < split.Count(); x++)
                        split[x] = Sanitize(split[x]);

                    if (split.Any(x => !String.IsNullOrEmpty(x)))
                    {
                        Post post = new Post(0, "", "", new List<string>(), new List<string>(), 0, "", new List<string>(), "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                        if (type == "mod" || type == "tool")
                            post = new Post(index, type, split[1], split[2].Split(',').ToList(), split[3].Split(',').ToList(), Convert.ToSingle(split[4]), split[5], split[6].Split(',').ToList(), split[0], split[7], split[9], split[10], split[11], split[12], split[15], split[16], split[17], split[8], split[13], split[18], split[19], split[14]);
                        else if (type == "cheat")
                            post = new Post(index, type, split[1], split[2].Split(',').ToList(), split[3].Split(',').ToList(), 1, split[4], split[5].Split(',').ToList(), split[0], split[6], "", "", "", "", "", "", "", split[7], "", "", "", split[8]);
                        else if (type == "guide")
                            post = new Post(index, type, split[1], split[2].Split(',').ToList(), split[3].Split(',').ToList(), 1, split[4], split[5].Split(',').ToList(), split[0], split[6], split[7], "", "", "", "", "", "", "", "", split[8], split[9], "");
                        index++;
                        posts.Add(post);
                    }
                }
            }
            return posts;
        }

        public static string Sanitize(string s)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s))
                return string.Empty;

            return s.TrimStart('\"').TrimEnd('\"').Trim();
        }

        public static string Write(Post post, bool single)
        {
            string result;

            //Post Summary
            result = $"<div class=\"toggle\"><div class=\"toggle-title\"><table><tbody><tr><td width='25%'>";
            //Hyperlink
            result += $"<a href=\"https://amicitia.github.io/post/{post.Hyperlink}\"><div class=\"getlink\"><i class=\"fas fa-link\" aria-hidden=\"true\"></i></div></a>";
            //New Label
            if (DateTime.Compare(DateTime.Now.AddDays(-30), DateTime.Parse(post.Date, CultureInfo.CreateSpecificCulture("en-US"))) <= 0)
            {
                result += "<div class=\"ribbon\"><div class=\"new\">NEW</div></div>";
            }
            //Thumbnail
            if (post.EmbedURL.Contains("youtu"))
            {
                string videoID = post.EmbedURL.Substring(post.EmbedURL.IndexOf("v=") + 2);
                string ytThumb = $"https://img.youtube.com/vi/{videoID}/default.jpg";
                result += $"<a href=\"{post.EmbedURL}\"><img src=\"{ytThumb}\" height=\"auto\" width=\"auto\"></td>";
            }
            else if (post.EmbedURL.Contains("streamable.com"))
            {
                result += $"<div style=\"width:100%;height:0px;position:relative;padding-bottom:56.250%;\"><iframe src=\"{post.EmbedURL}\" frameborder=\"0\"width=\"100%\"height=\"100%\"allowfullscreen style=\"width:100%;height:100%;position:absolute;\"></iframe></div>";
            }
            else if (post.EmbedURL != null && post.EmbedURL.Trim() != "")
                result += $"<img src=\"{post.EmbedURL}\" height=\"auto\" width=\"auto\"></td>";
            else
                result += "<img src=\"https://i.imgur.com/5I5Vos8.png\" height=\"auto\" width=\"auto\"></td>";

            //Visible Post Details
            result += $"<td width='50%'><h2>{post.Title}</h2><h5>{post.Games.First()} {post.Type} by ";
            //Author
            foreach (string author in post.Authors.Where(x => !x.Equals("Unknown Author") && !String.IsNullOrWhiteSpace(x)))
            {
                result += $"<a href=\"https://amicitia.github.io/author/{author.Trim()}\">{author.Trim()}</a>";
                if (post.Authors.IndexOf(author) != post.Authors.Count() - 1)
                    result += ", ";
            }
            result += $"</h5></td><td width='25%'><center><h5>{post.Date}</h5></center></td></tr></tbody></table></div>";
            //Hidden Post Details
            if (!single)
                result += $"<div class=\"toggle-inner\">";
            else
                result += "<div>";
            result += $"<div class=\"cheat\"><table><tbody><tr><td>{post.Description}";
            //Update
            if (!String.IsNullOrEmpty(post.UpdateText) && post.Type != "cheat")
                result += $"<br><div class=\"update\">{post.UpdateText}</div>";

            result += "</td><td>";
            //Download
            if (post.Type == "mod" || post.Type == "tool")
            {
                result += $"<center><font size=\"4\"><a href=\"{post.DownloadURL}\"><i class=\"fas fa-{post.DownloadIcon}\" aria-hidden=\"true\"></i> Download {post.DownloadText}</a><br>";
                if (post.DownloadURL2 != "")
                    result += $"<a href=\"{post.DownloadURL2}\"><i class=\"fas fa-{post.DownloadIcon2}\" aria-hidden=\"true\"></i> Download {post.DownloadText2}</a><br>";
                if (post.SourceURL != "")
                    result += $"<a href=\"{post.SourceURL}\"><i class=\"fas fa-file-code\" aria-hidden=\"true\"></i> Source Code</a>";
                result += "</center></div>";
            }
            else if (post.Type == "cheat")
                result += $"<div id=\"cheat{post.ID}\" onclick=\"copyDivToClipboard('cheat{post.ID}')\"><div class=\"cheatcode\">{post.UpdateText}</div></div>";

            //Related Guides
            if (!String.IsNullOrEmpty(post.GuideURL))
                result += $"<br><center><a href=\"{post.GuideURL}\"><i class=\"fas fa-info-circle\" aria-hidden=\"true\"></i> {post.GuideText}</a></center>";
            //Thread Link
            if (!String.IsNullOrEmpty(post.ThreadURL))
                result += $"<center><a href=\"{post.ThreadURL}\"><i class=\"fas fa-comment\" aria-hidden=\"true\"></i> Feedback</a></center>";

            //End of table
            result += "</tr></tbody></table>";

            //Tags
            int color = 0;
            foreach (string tag in post.Tags.Where(x => !String.IsNullOrWhiteSpace(x)))
            {
                if (color == Program.tagColors.Count())
                    color = 0;
                result += $"<a href=\"https://amicitia.github.io/tag/{tag.Trim()}\"><div class=\"tag\" style=\"border-left: 4px solid #{Program.tagColors[color++]}; \"><p class=\"noselect\">{tag}</p></div></a>";
            }
            //End Entry
            result += "</div></div></div>";

            return result;
        }
    }
}
