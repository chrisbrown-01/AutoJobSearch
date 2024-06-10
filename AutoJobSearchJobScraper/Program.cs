using AutoJobSearchJobScraper.Data;
using AutoJobSearchJobScraper.Utility;
using AutoJobSearchJobScraper.WebScraper;
using AutoJobSearchShared.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Json;

namespace AutoJobSearchJobScraper
{
    internal class Program
    {
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
                                //.WriteTo.Console(new JsonFormatter())
                                .WriteTo.File(new JsonFormatter(), logFilePath)
                                .CreateLogger();

            // Attach event handler for unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                var exception = (Exception)eventArgs.ExceptionObject;
                Log.Fatal(exception, "An unhandled exception occurred.");
            };
        }

        private static void Main(string[] args)
        {
            ConfigureLogger();

            if (args.Length < 1) throw new ArgumentException("No arguments provided.");

            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddSerilog(Log.Logger, true))
                .AddScoped<IDbContext, SQLiteDbContext>()
                .AddScoped<IWebScraper, SeleniumWebScraper>()
                .AddScoped<JobListingUtility>()
                .BuildServiceProvider();

            if (int.TryParse(args[0], out int jobSearchProfileId))
            {
                Log.Information("Starting application with {@jobSearchProfileId} argument.", jobSearchProfileId);
                RunProgram(serviceProvider, jobSearchProfileId).GetAwaiter().GetResult();
            }
            else
            {
                Log.Information("Starting application with {@args.Count} string arguments.", args.Length);
                RunProgram(serviceProvider, args.AsEnumerable()).GetAwaiter().GetResult();
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

            var scrapedJobs = await scraper.ScrapeJobsAsync(
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.Searches),
                jobSearchProfile.MaxJobListingIndex);

            var existingLinks = await db.GetAllApplicationLinksAsync();

            var filteredJobs = await utility.FilterDuplicatesAsync(scrapedJobs, existingLinks.ToHashSet());

            var scoredJobs = await utility.ApplyScoringsAsync(
                filteredJobs,
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.KeywordsPositive),
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.KeywordsNegative),
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.SentimentsPositive),
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(jobSearchProfile.SentimentsNegative));

            await db.SaveJobListingsAsync(scoredJobs);

            Console.WriteLine(
                "\r\n" +
                "All jobs have finished processing. " +
                "Please refresh the Job Board (click 'Options', then 'Go To Default View') to see all new jobs. " +
                "You can now close this window." +
                "\r\n");
        }

        private static async Task RunProgram(IServiceProvider serviceProvider, IEnumerable<string> searchTerms)
        {
            using var db = serviceProvider.GetRequiredService<IDbContext>();
            var scraper = serviceProvider.GetRequiredService<IWebScraper>();
            var utility = serviceProvider.GetRequiredService<JobListingUtility>();

            var existingLinks = await db.GetAllApplicationLinksAsync();
            var scrapedJobs = await scraper.ScrapeJobsAsync(searchTerms, null);
            var filteredJobs = await utility.FilterDuplicatesAsync(scrapedJobs, existingLinks.ToHashSet());

            await db.SaveJobListingsAsync(filteredJobs);
        }
    }
}