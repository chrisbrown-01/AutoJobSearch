using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoJobSearchConsoleApp
{
    internal class LocalFileTesting
    {
        // TODO: create tests
        private const string RAW_PAGE_SOURCE_FILE_PATH = "..\\..\\..\\DotNetTennesseJobsSeachStart0.txt"; // 10 jobList items, each have atleast 1 apply link
        private const string STARTING_INDEX_KEY = "CollapseJob description";
        private const string ENDING_INDEX_KEY = "Show full description"; // "Show full description" or if none found, "Report this job"
        //private const string REGEX_URL_PATTERN = @"href=\\""(https ?://[^\""]*)\""";
        //private const string REGEX_URL_PATTERN = @"https?://\S+";
        private const string REGEX_URL_PATTERN = @"https?://[^\s""]+";

        public static async Task MultiFileTest()
        {
            var doc = new HtmlDocument();
            var jobListings = new List<JobPosting>();

            for (int i = 1; i < 12; i++)
            {
                var textFile = await File.ReadAllTextAsync($"..\\..\\..\\DotNetTennessee{i}of11.txt");
                doc.LoadHtml(textFile);

                jobListings.AddRange(ExtractJobs(doc));
            }

            Console.WriteLine();
        }

        public static async Task SingleFileTest()
        {
            var textFile = await File.ReadAllTextAsync(RAW_PAGE_SOURCE_FILE_PATH);
            var doc = new HtmlDocument();
            doc.LoadHtml(textFile);

            var jobListings = ExtractJobs(doc);

            Console.WriteLine();
        }

        private static List<JobPosting> ExtractJobs(HtmlDocument htmlDocument)
        {
            var jobList = new List<JobPosting>();

            var liElements = htmlDocument.DocumentNode.SelectNodes("//li").ToList();            

            foreach (var li in liElements)
            {
                var links = li.Descendants("a")
                    .Where(a => a.InnerText.Contains("apply", StringComparison.OrdinalIgnoreCase));

                var listing = new JobPosting();

                listing.InnerText = li.InnerText; // TODO: html decode (ie. convert &amp; to regular &)

                // TODO: extract to method
                var startingIndex = listing.InnerText.IndexOf(STARTING_INDEX_KEY);
                var endingIndex = listing.InnerText.IndexOf(ENDING_INDEX_KEY);

                if (startingIndex != -1 && endingIndex != -1)
                {
                    try
                    {
                        listing.InnerTextCleaned = listing.InnerText.Substring(startingIndex + STARTING_INDEX_KEY.Length, endingIndex - (startingIndex + STARTING_INDEX_KEY.Length));
                    }
                    catch
                    {
                        Console.WriteLine("Substring error");
                    }
                }

                foreach (var link in links)
                {
                    listing.LinksOuterHtml.Add(link.OuterHtml);

                    MatchCollection matches = Regex.Matches(link.OuterHtml, REGEX_URL_PATTERN);

                    foreach (Match match in matches)
                    {
                        // listing.Links.Add(match.Value.Replace('"', ' ').Trim()); 
                        listing.Links.Add(match.Value);
                    }
                }

                jobList.Add(listing);
            }

            return jobList;
        }

        // TODO: move to seperate class file
        public class JobPosting
        {
            public string InnerText { get; set; } = string.Empty;
            public string InnerTextCleaned { get; set; } = string.Empty;

            public List<string> LinksOuterHtml { get; set; } = new();
            public List<string> Links { get; set; } = new();
        }
    }
}
