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

        private static async Task RunProgram(IServiceProvider serviceProvider, int jobSearchProfileId)
        {
            var db = serviceProvider.GetRequiredService<IDbContext>();
            var scraper = serviceProvider.GetRequiredService<IWebScraper>();
            var utility = serviceProvider.GetRequiredService<JobListingUtility>();

            var jobSearchProfile = await db.GetJobSearchProfileByIdAsync(jobSearchProfileId) ?? throw new NullReferenceException(); // TODO: custom exception

            // TODO: delete
            //var scrapedJobs = await scraper.ScrapeJobs(new List<string>() { "programming jobs toronto" }); 
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
            var db = serviceProvider.GetRequiredService<IDbContext>();
            var scraper = serviceProvider.GetRequiredService<IWebScraper>();
            var utility = serviceProvider.GetRequiredService<JobListingUtility>();

            var existingLinks = await db.GetAllApplicationLinks();
            var scrapedJobs = await scraper.ScrapeJobs(searchTerms);
            var cleanedJobs = await utility.FilterDuplicates(scrapedJobs, existingLinks.ToHashSet());

            await db.SaveJobListings(cleanedJobs);
        }
    }
}