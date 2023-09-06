using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchConsoleApp
{
    internal class LocalFileTesting
    {
        // TODO: create tests
        private const string RAW_PAGE_SOURCE_FILE_PATH = "..\\..\\..\\DotNetTennesseJobsSeachStart0.txt"; // 10 list items, each have atleast 1 apply link
        private const string STARTING_INDEX_KEY = "CollapseJob description";
        private const string ENDING_INDEX_KEY = "Show full description"; // "Show full description" or if none found, "Report this listing"

        public static async Task Execute()
        {
            var textFile = await File.ReadAllTextAsync(RAW_PAGE_SOURCE_FILE_PATH);
            var doc = new HtmlDocument();
            doc.LoadHtml(textFile);

            var liElements = doc.DocumentNode.SelectNodes("//li").ToList();
            var jobListings = new List<JobListing>();

            foreach (var li in liElements)
            {
                var links = li.Descendants("a")
                    .Where(a => a.InnerText.Contains("apply", StringComparison.OrdinalIgnoreCase));

                var listing = new JobListing();

                listing.InnerText = li.InnerText; // TODO: html decode (ie. convert &amp; to regular &)

                foreach(var link in links)
                {
                    listing.Links.Add(link.OuterHtml);
                }

                jobListings.Add(listing);
            }

            foreach(var listing in jobListings)
            {
                // TODO: check for if there are multiple results, experiment with using LastIndexOf
                var startingIndex = listing.InnerText.IndexOf(STARTING_INDEX_KEY);
                var endingIndex = listing.InnerText.IndexOf(ENDING_INDEX_KEY); // if not found, value is -1. Therefore do logic statements where if either are -1, don't modify the current InnerText property.

                if (startingIndex == -1 || endingIndex == -1) continue;

                try
                {
                    listing.InnerText = listing.InnerText.Substring(startingIndex + STARTING_INDEX_KEY.Length, endingIndex - (startingIndex + STARTING_INDEX_KEY.Length));
                }
                catch
                {
                    Console.WriteLine("Substring error");
                }


                Console.WriteLine();
            }

            await Task.CompletedTask;
        }

        public class JobListing
        {
            public string InnerText { get; set; } = string.Empty;
            public List<string> Links { get; set; } = new();
        }
    }
}
