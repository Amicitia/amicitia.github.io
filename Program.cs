using MoreLinq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Amicitia.github.io.PageCreator;
using System.Threading.Tasks;

namespace Amicitia.github.io
{
    class Program
    {
        public static string indexPath; //Path to website root directory
        public static List<Tuple<string, string>> gameList = new List<Tuple<string, string>>() { //Games in dropdown
            new Tuple<string, string>("P5", "Persona 5"),
            new Tuple<string, string>("P5R", "Persona 5 Royal"),
            new Tuple<string, string>("P5S", "Persona 5 Strikers"),
            new Tuple<string, string>("P5D", "Persona 5 Dancing"),
            new Tuple<string, string>("P4", "Persona 4"),
            new Tuple<string, string>("P4G", "Persona 4 Golden"),
            new Tuple<string, string>("P4AU", "Persona 4 Arena Ultimax"),
            new Tuple<string, string>("P4D", "Persona 4 Dancing"),
            new Tuple<string, string>("P3FES", "Persona 3 FES"),
            new Tuple<string, string>("P3P", "Persona 3 Portable"),
            new Tuple<string, string>("PQ", "Persona Q"),
            new Tuple<string, string>("PQ2", "Persona Q2"),
            new Tuple<string, string>("CFB", "Catherine Full Body"),
            new Tuple<string, string>("SMT3", "SMT3: Nocturne")
        };
        public static List<string> tagColors = new List<string>() { "F37E79", "F3BF79", "F3D979", "7AF379", "7998F3", "DE79F3" }; //Hex color values for tags
        public static List<Post> posts; //Posts
        public static int maxPosts = 15; //Number of posts per page

        public static void Main(string[] args)
        {
            // Exe Directory
            indexPath = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));
            // Update .tsv with data from gamebanana
            Task.Run(async () =>
            {
                await Webscraper.UpdateTSVs(indexPath);
            }).GetAwaiter().GetResult();
            // Order post post from .tsv files by most recent)
            posts = Post.Get(indexPath).OrderBy(p => DateTime.Parse(p.Date, CultureInfo.CreateSpecificCulture("en-US"))).ToArray().Reverse().ToList();
            // Delete files if they exist already
            Page.DeleteExisting(indexPath);
            // Create main page with all mods, tools, guides and cheats
            Page.CreateHtml(posts, "index");

            // List all mods, tools, cheats and guides (per game as well)
            Page.CreateType("mod"); // i.e. amicitia.github.io/mods
            Page.CreateType("tool"); // i.e. amicitia.github.io/tools/p5
            Page.CreateType("cheat"); // i.e. amicitia.github.io/cheats/p4
            Page.CreateType("guide"); // i.e. amicitia.github.io/guides/p5r

            // Create pages for all content per game (regardless of type)
            Page.CreateGames(posts); // i.e. amicitia.github.io/game/p3fes

            // Searchable type ppostsages
            Page.CreateAuthors(posts); //amicitia.github.io/author/TGE
            Page.CreateTags(posts); // i.e. amicitia.github.io/tag/BF

            // All individual posts (hyperlinks)
            Page.CreateSingle(posts); // i.e. amicitia.github.io/post/amicitia

            // Create flowscript docs
            Page.FlowscriptDocs(indexPath);
            // Create 404/files pages
            Page.Misc(indexPath);

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
