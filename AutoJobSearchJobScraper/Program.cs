using AutoJobSearchJobScraper.Data;
using AutoJobSearchJobScraper.Utility;
using AutoJobSearchJobScraper.WebScraper;

namespace AutoJobSearchJobScraper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RunProgram();
        }

        private static async Task RunProgram()
        {
            var scraper = new SeleniumWebScraper();
            var utility = new JobListingUtility();
            var db = new SQLiteDbContext();

            //var existingLinks = await db.GetAllApplicationLinks();
            var scrapedJobs = await scraper.ScrapeJobs("c# developer toronto");
            //var cleanedJobs = await utility.FilterDuplicates(scrapedJobs, existingLinks.ToHashSet());
            //var scoredJobs = await utility.ApplyScorings(cleanedJobs);

            //await db.SaveJobListings(scoredJobs);
        }
    }
}