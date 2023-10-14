using AutoJobSearchShared.Models;
using FuzzySharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchJobScraper.Utility
{
    internal class JobListingUtility
    {
        private readonly ILogger<JobListingUtility> _logger;

        public JobListingUtility(ILogger<JobListingUtility> logger)
        {
            _logger = logger;
        }

        // TODO: create unit test project
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

            foreach(var jobListing in jobListingsPossibleDuplicates)
            {
                bool isJobDuplicate = false;

                foreach(var link in jobListing.ApplicationLinks)
                {
                    if(existingApplicationLinks.Contains(link.Link))
                    {
                        cleanedJobListings.Remove(jobListing);
                        isJobDuplicate = true;
                        break;
                    }
                }

                if (isJobDuplicate) continue;

                foreach(var link in jobListing.ApplicationLinks)
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

            sentimentsPositive = sentimentsPositive.Select(s => s.ToLower());
            sentimentsNegative = sentimentsNegative.Select(s => s.ToLower());

            var jobList = jobListingsUnscored.ToList();

            // TODO: test speed improvements over single threaded method (use 500 jobs)
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
                    if (Fuzz.WeightedRatio(sentiment, job.Description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment, job.Description.ToLower()) >= 50)
                    {
                        job.Score++;
                    }
                }

                foreach (var sentiment in sentimentsNegative)
                {
                    if (Fuzz.WeightedRatio(sentiment, job.Description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment, job.Description.ToLower()) >= 50)
                    {
                        job.Score--;
                    }
                }
            });

            /*
            foreach (var job in jobList)
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
                    if (Fuzz.WeightedRatio(sentiment, job.Description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment, job.Description.ToLower()) >= 50)
                    {
                        job.Score++;
                    }
                }

                foreach (var sentiment in sentimentsNegative)
                {
                    if (Fuzz.WeightedRatio(sentiment, job.Description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment, job.Description.ToLower()) >= 50)
                    {
                        job.Score--;
                    }
                }
            }
            */

            await Task.CompletedTask;
            return jobList;
        }
    }
}
