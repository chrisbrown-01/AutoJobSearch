using AutoJobSearchJobScraper.Data;
using AutoJobSearchJobScraper.Utility;
using AutoJobSearchJobScraper.WebScraper;
using AutoJobSearchShared.Helpers;
using AutoJobSearchShared.Models;

namespace AutoJobSearchJobScraper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RunProgram(38);

            //if (args.Length < 1) throw new ArgumentException("No arguments provided."); // TODO: custom exception

            //if (int.TryParse(args[0], out int jobSearchProfileId))
            //{
            //    RunProgram(jobSearchProfileId);
            //}
            //else
            //{
            //    RunProgram(new List<string>(args)); // TODO: find all manual declaration of List conversions and convert to this
            //}
        }

        private static async Task RunProgram(int jobSearchProfileId)
        {
            var test = jobSearchProfileId;

            var db = new SQLiteDbContext();
            var scraper = new SeleniumWebScraper();
            var utility = new JobListingUtility();

            var jobSearchProfile = await db.GetJobSearchProfileByIdAsync(jobSearchProfileId);
            if (jobSearchProfile == null) throw new NullReferenceException(); // TODO: custom exception

            var scrapedJobs = new List<JobListing>();
            //var scrapedJobs = await scraper.ScrapeJobs(StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.Searches));

            var existingLinks = await db.GetAllApplicationLinks();
            var cleanedJobs = await utility.FilterDuplicates(scrapedJobs, existingLinks.ToHashSet());

            //var scoredJobs = await utility.ApplyScorings(
            //    cleanedJobs,
            //    StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.KeywordsPositive),
            //    StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.KeywordsNegative),
            //    StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.SentimentsPositive),
            //    StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.SentimentsNegative));

            //await db.SaveJobListings(scoredJobs);
        }

        private static async Task RunProgram(IEnumerable<string> searchTerms)
        {
            var test = searchTerms;

            //var db = new SQLiteDbContext();
            //var scraper = new SeleniumWebScraper();
            //var utility = new JobListingUtility();

            //var existingLinks = await db.GetAllApplicationLinks();
            //var scrapedJobs = await scraper.ScrapeJobs(searchTerms);
            //var cleanedJobs = await utility.FilterDuplicates(scrapedJobs, existingLinks.ToHashSet());

            //await db.SaveJobListings(scoredJobs);
        }
    }
}