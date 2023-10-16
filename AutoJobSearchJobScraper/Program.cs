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
        static void Main(string[] args)
        {
            ConfigureLogger();

            // TODO: remove hardcoding
            //if (args.Length < 1) throw new ArgumentException("No arguments provided."); 

            args = new string[1];
            args[0] = "1";

            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddSerilog(Log.Logger, true))
                .AddScoped<IDbContext, SQLiteDbContext>()
                .AddScoped<IWebScraper, SeleniumWebScraper>()
                .AddScoped<JobListingUtility>()
                .BuildServiceProvider();

            if (int.TryParse(args[0], out int jobSearchProfileId))
            {
                Log.Information("Starting application with {@jobSearchProfileId} argument.", jobSearchProfileId);
                RunProgram(serviceProvider, jobSearchProfileId).Wait();
            }
            else
            {
                Log.Information("Starting application with {@args.Count} string arguments.", args.Count());
                RunProgram(serviceProvider, args.AsEnumerable()).Wait();            
            }

            Log.Information("Job scraper application finished executing.");
            Log.CloseAndFlush();
        }

        private static async Task RunProgram(IServiceProvider serviceProvider, int jobSearchProfileId)
        {
            using var db = serviceProvider.GetRequiredService<IDbContext>();
            var scraper = serviceProvider.GetRequiredService<IWebScraper>();
            var utility = serviceProvider.GetRequiredService<JobListingUtility>();

            var jobSearchProfile = await db.GetJobSearchProfileByIdAsync(jobSearchProfileId) ?? 
                throw new NullReferenceException($"Job search profile ID {jobSearchProfileId} not found in database."); 

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
            const string LOG_FILE_NAME = "JobScraperLogFile.json";
            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LOG_FILE_NAME);

            if (!File.Exists(logFilePath))
            {
                var fileStream = File.Create(logFilePath);
                fileStream.Close();
                fileStream.Dispose();
            }

            Log.Logger = new LoggerConfiguration()
                                .MinimumLevel.Information()
                                .WriteTo.Console(new JsonFormatter())
                                .WriteTo.File(new JsonFormatter(), logFilePath)
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