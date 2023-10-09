using AutoJobSearchJobScraper.Constants;
using AutoJobSearchShared.Models;
using FuzzySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchJobScraper.Utility
{
    internal class JobListingUtility
    {
        public JobListingUtility()
        {
            
        }

        // TODO: create unit test project
        public async Task<List<JobListing>> FilterDuplicates(IEnumerable<JobListing> jobListingsPossibleDuplicates, HashSet<string> existingApplicationLinks) // TODO: static?
        {
            var cleanedJobListings = jobListingsPossibleDuplicates.ToList();

            foreach(var jobListing in jobListingsPossibleDuplicates)
            {
                foreach(var link in jobListing.ApplicationLinks)
                {
                    if(existingApplicationLinks.Contains(link.Link))
                    {
                        cleanedJobListings.Remove(jobListing); // TODO: ensure that upon removal, the iterator doesn't lose track of where it was. Might be best to track index then remove at end.
                    }
                    else
                    {
                        existingApplicationLinks.Add(link.Link);
                    }
                }
            }

            await Task.CompletedTask;
            return cleanedJobListings;
        }

        public async Task<List<JobListing>> ApplyScorings(IEnumerable<JobListing> jobListingsUnscored) // TODO: ensure functional programming is occurring
        {
            var jobList = jobListingsUnscored.ToList();

            foreach (var job in jobList)
            {
                foreach (var keyword in ConfigVariables.KEYWORDS_POSITIVE)
                {
                    if (job.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Score++;
                    }
                }

                foreach (var item in ConfigVariables.KEYWORDS_NEGATIVE)
                {
                    if (job.Description.Contains(item, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Score--;
                    }
                }

                // TODO: more robust ensuring that arguments are made lower case
                foreach (var sentiment in ConfigVariables.SENTIMENTS_POSITIVE)
                {
                    if (Fuzz.WeightedRatio(sentiment.ToLower(), job.Description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment.ToLower(), job.Description.ToLower()) >= 50)
                    {
                        job.Score++;
                    }
                }

                foreach (var sentiment in ConfigVariables.SENTIMENTS_NEGATIVE)
                {
                    if (Fuzz.WeightedRatio(sentiment.ToLower(), job.Description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment.ToLower(), job.Description.ToLower()) >= 50)
                    {
                        job.Score--;
                    }
                }
            }

            await Task.CompletedTask;
            return jobList;
        }
    }
}
