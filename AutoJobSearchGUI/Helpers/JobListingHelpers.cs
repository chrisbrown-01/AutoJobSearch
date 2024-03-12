using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using System.Collections.Generic;

namespace AutoJobSearchGUI.Helpers
{
    internal static class JobListingHelpers
    {
        internal static List<JobListingModel> ConvertJobListingsToJobListingModels(IEnumerable<JobListing> jobs)
        {
            var jobListings = new List<JobListingModel>();

            foreach (var job in jobs)
            {
                jobListings.Add(ConvertJobListingToJobListingModel(job));
            }

            return jobListings;
        }

        internal static JobListingModel ConvertJobListingToJobListingModel(JobListing job)
        {
            return new JobListingModel
            {
                Id = job.Id,
                SearchTerm = job.SearchTerm,
                CreatedAt = job.CreatedAt,
                StatusModifiedAt = job.StatusModifiedAt,
                Description = job.Description,
                Score = job.Score,
                IsToBeAppliedTo = job.IsToBeAppliedTo,
                IsAppliedTo = job.IsAppliedTo,
                IsInterviewing = job.IsInterviewing,
                IsNegotiating = job.IsNegotiating,
                IsRejected = job.IsRejected,
                IsDeclinedOffer = job.IsDeclinedOffer,
                IsAcceptedOffer = job.IsAcceptedOffer,
                IsFavourite = job.IsFavourite,
                IsHidden = job.IsHidden
            };
        }
    }
}