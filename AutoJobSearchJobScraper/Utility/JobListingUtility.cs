using AutoJobSearchJobScraper.Exceptions;
using AutoJobSearchShared.Models;
using FuzzySharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoJobSearchJobScraper.Utility
{
    internal class JobListingUtility
    {
        private readonly ILogger<JobListingUtility> _logger;
        private readonly int PARTIAL_FUZZ_RATIO_THRESHOLD;
        private readonly int WEIGHTED_FUZZ_RATIO_THRESHOLD;

        public JobListingUtility(ILogger<JobListingUtility> logger)
        {
            _logger = logger;

            var builder = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);

            var config = builder.Build();

            try
            {
                WEIGHTED_FUZZ_RATIO_THRESHOLD = config.GetValue<int>(nameof(WEIGHTED_FUZZ_RATIO_THRESHOLD));
            }
            catch (InvalidOperationException)
            {
                throw new AppSettingsFileArgumentException(
                    $"Failed to read {nameof(WEIGHTED_FUZZ_RATIO_THRESHOLD)} from appsettings.json config file. " +
                    $"Ensure that {nameof(WEIGHTED_FUZZ_RATIO_THRESHOLD)} is an integer.");
            }

            if (WEIGHTED_FUZZ_RATIO_THRESHOLD < 1)
            {
                throw new AppSettingsFileArgumentException(
                    $"Failed to read {nameof(WEIGHTED_FUZZ_RATIO_THRESHOLD)} from appsettings.json config file. " +
                    $"Ensure that {nameof(WEIGHTED_FUZZ_RATIO_THRESHOLD)} has a value greater than 0.");
            }

            try
            {
                PARTIAL_FUZZ_RATIO_THRESHOLD = config.GetValue<int>(nameof(PARTIAL_FUZZ_RATIO_THRESHOLD));
            }
            catch (InvalidOperationException)
            {
                throw new AppSettingsFileArgumentException(
                    $"Failed to read {nameof(PARTIAL_FUZZ_RATIO_THRESHOLD)} from appsettings.json config file. " +
                    $"Ensure that {nameof(PARTIAL_FUZZ_RATIO_THRESHOLD)} is an integer.");
            }

            if (PARTIAL_FUZZ_RATIO_THRESHOLD < 1)
            {
                throw new AppSettingsFileArgumentException(
                    $"Failed to read {nameof(PARTIAL_FUZZ_RATIO_THRESHOLD)} from appsettings.json config file. " +
                    $"Ensure that {nameof(PARTIAL_FUZZ_RATIO_THRESHOLD)} has a value greater than 0.");
            }
        }

        public async Task<IEnumerable<JobListing>> ApplyScoringsAsync(
            IEnumerable<JobListing> jobListingsUnscored,
            IEnumerable<string> keywordsPositive,
            IEnumerable<string> keywordsNegative,
            IEnumerable<string> sentimentsPositive,
            IEnumerable<string> sentimentsNegative)
        {
            _logger.LogInformation("Applying scorings to job listings. " +
                              "{@jobListingsUnscored.Count}, " +
                              "{@keywordsPositive.Count}, " +
                              "{@keywordsNegative.Count}, " +
                              "{@sentimentsPositive.Count}, " +
                              "{@sentimentsNegative.Count}",
                              jobListingsUnscored.Count(),
                              keywordsPositive.Count(),
                              keywordsNegative.Count(),
                              sentimentsPositive.Count(),
                              sentimentsNegative.Count());

            Console.WriteLine("\r\nApplying scorings to job listings. DO NOT CLOSE THIS WINDOW YET.\r\n");

            sentimentsPositive = sentimentsPositive.Select(s => s.ToLower());
            sentimentsNegative = sentimentsNegative.Select(s => s.ToLower());

            var jobList = jobListingsUnscored.ToList();

            var counter = 0;
            var lockObj = new object();

            Parallel.ForEach(jobList, job =>
            {
                foreach (var keyword in keywordsPositive)
                {
                    if (job.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Score++;
                    }
                }

                foreach (var item in keywordsNegative)
                {
                    if (job.Description.Contains(item, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Score--;
                    }
                }

                foreach (var sentiment in sentimentsPositive)
                {
                    // TODO: experiment with thresholds
                    if (Fuzz.WeightedRatio(sentiment, job.Description.ToLower()) >= WEIGHTED_FUZZ_RATIO_THRESHOLD &&
                       Fuzz.PartialRatio(sentiment, job.Description.ToLower()) >= PARTIAL_FUZZ_RATIO_THRESHOLD)
                    {
                        job.Score++;
                    }
                }

                foreach (var sentiment in sentimentsNegative)
                {
                    if (Fuzz.WeightedRatio(sentiment, job.Description.ToLower()) >= WEIGHTED_FUZZ_RATIO_THRESHOLD &&
                       Fuzz.PartialRatio(sentiment, job.Description.ToLower()) >= PARTIAL_FUZZ_RATIO_THRESHOLD)
                    {
                        job.Score--;
                    }
                }

                lock (lockObj)
                {
                    int progress = ++counter;
                    Console.WriteLine("{0} of {1} processed...", progress, jobList.Count);
                }
            });

            await Task.CompletedTask;
            return jobList;
        }

        public async Task<IEnumerable<JobListing>> FilterDuplicatesAsync(
                    IEnumerable<JobListing> jobListingsPossibleDuplicates,
            HashSet<string> existingApplicationLinks)
        {
            _logger.LogInformation("Filtering duplicate job listings. " +
                                   "{@jobListingsPossibleDuplicates.Count}, " +
                                   "{@existingApplicationLinks.Count}",
                                    jobListingsPossibleDuplicates.Count(),
                                    existingApplicationLinks.Count);

            var cleanedJobListings = jobListingsPossibleDuplicates.ToList();

            foreach (var jobListing in jobListingsPossibleDuplicates)
            {
                bool isJobDuplicate = false;

                foreach (var link in jobListing.ApplicationLinks)
                {
                    if (existingApplicationLinks.Contains(link.Link))
                    {
                        cleanedJobListings.Remove(jobListing);
                        isJobDuplicate = true;
                        break;
                    }
                }

                if (isJobDuplicate) continue;

                foreach (var link in jobListing.ApplicationLinks)
                {
                    existingApplicationLinks.Add(link.Link);
                }
            }

            await Task.CompletedTask;

            _logger.LogInformation(
                "Completed filtering for duplicate job listings. " +
                "Returning {@cleanedJobListings.Count} unique job listings.",
                cleanedJobListings.Count);

            return cleanedJobListings;
        }
    }
}