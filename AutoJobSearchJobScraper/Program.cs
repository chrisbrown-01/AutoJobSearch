using AutoJobSearchJobScraper.Data;
using AutoJobSearchJobScraper.Utility;
using AutoJobSearchJobScraper.WebScraper;
using AutoJobSearchShared.Helpers;
using AutoJobSearchShared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using System;

namespace AutoJobSearchJobScraper
{
    internal class Program
    {
        // TODO: uninstall unused nuget packages
        static void Main(string[] args)
        {
            ConfigureLogger();

            // if (args.Length < 1) throw new ArgumentException("No arguments provided."); // TODO: custom exception

            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddSerilog(Log.Logger, true))
                .AddScoped<IDbContext, SQLiteDbContext>()
                .AddScoped<IWebScraper, SeleniumWebScraper>()
                .AddScoped<JobListingUtility>()
                .BuildServiceProvider();

            Log.Information("Starting job scraper application.");
            RunProgram(serviceProvider, 38).Wait(); // TODO: remove hardcoding
            //TestConcurrencyIssues(serviceProvider).Wait();
            Log.CloseAndFlush();

            //if (int.TryParse(args[0], out int jobSearchProfileId))
            //{
            //    Log.Information("Starting application with {@jobSearchProfileId} argument.", jobSearchProfileId);
            //    RunProgram(serviceProvider, jobSearchProfileId).Wait();
            //    Log.Information("Job scraper application finished executing.");
            //}
            //else
            //{
            //    Log.Information("Starting application with {@args.Count} string arguments.", args.Count());
            //    RunProgram(serviceProvider, args.AsEnumerable()).Wait(); // TODO: find all manual declaration of List conversions and convert to this
            //    Log.Information("Job scraper application finished executing.");
            //}

        }

        private static async Task TestConcurrencyIssues(IServiceProvider serviceProvider)
        {
            using var db = serviceProvider.GetRequiredService<IDbContext>();

            var testEntries = new List<JobListing>();

            for (int i = 0; i < 1; i++)
            {
                var jobListing = new JobListing();
                testEntries.Add(jobListing);
            }

            await db.SaveJobListingsAsync(testEntries);
        }

        private static async Task RunProgram(IServiceProvider serviceProvider, int jobSearchProfileId)
        {
            using var db = serviceProvider.GetRequiredService<IDbContext>();
            var scraper = serviceProvider.GetRequiredService<IWebScraper>();
            var utility = serviceProvider.GetRequiredService<JobListingUtility>();

            var jobSearchProfile = await db.GetJobSearchProfileByIdAsync(jobSearchProfileId) ?? 
                throw new NullReferenceException($"Job search profile ID {jobSearchProfileId} not found in database."); 

            // TODO: delete
            //var scrapedJobs = await scraper.ScrapeJobsAsync(new List<string>() { "programming jobs waterloo" }); 
            var scrapedJobs = await scraper.ScrapeJobsAsync(StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.Searches));

            var existingLinks = await db.GetAllApplicationLinksAsync();

            var filteredJobs = await utility.FilterDuplicatesAsync(scrapedJobs, existingLinks.ToHashSet());

            var scoredJobs = await utility.ApplyScoringsAsync(
                filteredJobs,
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.KeywordsPositive),
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.KeywordsNegative),
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.SentimentsPositive),
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.SentimentsNegative));

            await db.SaveJobListingsAsync(scoredJobs);
        }

        private static async Task RunProgram(IServiceProvider serviceProvider, IEnumerable<string> searchTerms)
        {
            using var db = serviceProvider.GetRequiredService<IDbContext>();
            var scraper = serviceProvider.GetRequiredService<IWebScraper>();
            var utility = serviceProvider.GetRequiredService<JobListingUtility>();

            var existingLinks = await db.GetAllApplicationLinksAsync();
            var scrapedJobs = await scraper.ScrapeJobsAsync(searchTerms);
            var filteredJobs = await utility.FilterDuplicatesAsync(scrapedJobs, existingLinks.ToHashSet());

            await db.SaveJobListingsAsync(filteredJobs);
        }

        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                                .MinimumLevel.Information()
                                .WriteTo.Console(new JsonFormatter())
                                .WriteTo.File(new JsonFormatter(), "JobScraperLogFile.json")
                                .CreateLogger();

            // Attach event handler for unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                var exception = (Exception)eventArgs.ExceptionObject;
                Log.Fatal(exception, "An unhandled exception occurred.");
            };
        }
    }
}