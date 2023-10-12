using AutoJobSearchJobScraper.Data;
using AutoJobSearchJobScraper.Utility;
using AutoJobSearchJobScraper.WebScraper;
using AutoJobSearchShared.Helpers;
using AutoJobSearchShared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AutoJobSearchJobScraper
{
    internal class Program
    {
        // TODO: logging
        static void Main(string[] args)
        {
            // if (args.Length < 1) throw new ArgumentException("No arguments provided."); // TODO: custom exception

            var serviceProvider = new ServiceCollection()
                .AddScoped<IDbContext, SQLiteDbContext>()
                .AddScoped<IWebScraper, SeleniumWebScraper>()
                .AddScoped<JobListingUtility>()
                .BuildServiceProvider();

            RunProgram(serviceProvider, 38).Wait();

            //if (int.TryParse(args[0], out int jobSearchProfileId))
            //{
            //    RunProgram(serviceProvider, jobSearchProfileId).Wait();
            //}
            //else
            //{
            //    RunProgram(serviceProvider, args.AsEnumerable()).Wait(); // TODO: find all manual declaration of List conversions and convert to this
            //}

        }

        private static async Task RunProgram(IServiceProvider serviceProvider, int jobSearchProfileId)
        {
            var db = serviceProvider.GetRequiredService<IDbContext>();
            var scraper = serviceProvider.GetRequiredService<IWebScraper>();
            var utility = serviceProvider.GetRequiredService<JobListingUtility>();

            var jobSearchProfile = await db.GetJobSearchProfileByIdAsync(jobSearchProfileId);
            if (jobSearchProfile == null) throw new NullReferenceException(); // TODO: custom exception

            var scrapedJobs = await scraper.ScrapeJobs(StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.Searches));

            var existingLinks = await db.GetAllApplicationLinks();

            var cleanedJobs = await utility.FilterDuplicates(scrapedJobs, existingLinks.ToHashSet());

            var scoredJobs = await utility.ApplyScorings(
                cleanedJobs,
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.KeywordsPositive),
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.KeywordsNegative),
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.SentimentsPositive),
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.SentimentsNegative));

            await db.SaveJobListings(scoredJobs);
        }

        private static async Task RunProgram(IServiceProvider serviceProvider, IEnumerable<string> searchTerms)
        {
            var db = new SQLiteDbContext();
            var scraper = new SeleniumWebScraper();
            var utility = new JobListingUtility();

            var existingLinks = await db.GetAllApplicationLinks();
            var scrapedJobs = await scraper.ScrapeJobs(searchTerms);
            var cleanedJobs = await utility.FilterDuplicates(scrapedJobs, existingLinks.ToHashSet());

            await db.SaveJobListings(cleanedJobs);
        }
    }
}