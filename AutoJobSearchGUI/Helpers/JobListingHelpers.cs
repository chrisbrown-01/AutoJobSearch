using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Helpers
{
    internal class JobListingHelpers
    {
        internal static JobListingModel ConvertJobListingToJobListingModel(JobListing job)
        {
            return new JobListingModel
            {
                Id = job.Id,
                SearchTerm = job.SearchTerm,
                CreatedAt = job.CreatedAt,
                Description = job.Description,
                Score = job.Score,
                IsAppliedTo = job.IsAppliedTo,
                IsInterviewing = job.IsInterviewing,
                IsRejected = job.IsRejected,
                IsFavourite = job.IsFavourite,
                IsHidden = job.IsHidden
            };
        }

        internal static List<JobListingModel> ConvertJobListingsToJobListingModels(IEnumerable<JobListing> jobs)
        {
            var jobListings = new List<JobListingModel>();

            foreach (var job in jobs)
            {
                jobListings.Add(ConvertJobListingToJobListingModel(job));
            }

            return jobListings;
        }
    }
}
