using AutoJobSearchJobScraper.Data;
using AutoJobSearchJobScraper.Utility;
using AutoJobSearchJobScraper.WebScraper;

namespace AutoJobSearchJobScraper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            RunProgram();
        }

        private static async Task RunProgram()
        {
            var scraper = new SeleniumWebScraper();
            var utility = new JobListingUtility();
            var db = new SQLiteDbContext();

            //var existingLinks = await db.
            var scrapedJobs = await scraper.ScrapeJobs("c# developer toronto");
            var cleanedJobs = await utility.FilterDuplicates(scrapedJobs);
        }
    }
}