using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    // TODO: add margin/padding to views
    public partial class JobListingViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        [ObservableProperty]
        private JobListingModel _jobListing;

        private List<JobListingModel> JobListings { get; set; } = default!;

        private readonly IDbContext _dbContext;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public JobListingViewModel() // For View previewer only
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            JobListing = new JobListingModel();
        }

        public JobListingViewModel(IDbContext dbContext)
        {
            JobListing = new JobListingModel();
            _dbContext = dbContext;
        }

        public void PopulateJobListings(IEnumerable<JobListingModel> jobListings)
        {
            JobListings = jobListings.ToList();
        }

        public async Task GoToPreviousJob()
        {
            // TODO: investigate why calling tasks does not properly throw exception. may need to convert to relaycommand
            //throw new Exception("test");

            var currentIndex = JobListings.IndexOf(JobListing);
            if (currentIndex < 0) return;

            var previousIndex = currentIndex - 1;
            if (previousIndex < 0) return;

            DisableOnChangedEvents(JobListing);

            await OpenJobListing(JobListings[previousIndex]);
        }

        public async Task GoToNextJob()
        {
            var currentIndex = JobListings.IndexOf(JobListing);
            if (currentIndex < 0) return;

            var nextIndex = currentIndex + 1;
            if(nextIndex >= JobListings.Count) return;

            DisableOnChangedEvents(JobListing);

            await OpenJobListing(JobListings[nextIndex]);
        }

        public async Task OpenJobListing(JobListingModel jobListing)
        {
            if(!jobListing.DetailsPopulated)
            {
                var jobListingDetails = await _dbContext.GetJobListingDetailsByIdAsync(jobListing.Id);
                jobListing.Description = jobListingDetails.Description;
                jobListing.ApplicationLinks = jobListingDetails.ApplicationLinksString;
                jobListing.Notes = jobListingDetails.Notes;
                jobListing.DetailsPopulated = true;
            }

            JobListing = jobListing;
            EnableOnChangedEvents(JobListing);
        }

        /// <summary>
        /// Allows events to fire. This method should be called after the view model properties have been fully instantiated.
        /// </summary>
        /// <param name="jobListing"></param>
        private void EnableOnChangedEvents(JobListingModel jobListing)
        {
            jobListing.EnableEvents = true;
        }

        /// <summary>
        /// Prevent events from firing. This method should be called in preparation of instantiating new view model properties.
        /// </summary>
        /// <param name="jobListing"></param>
        private void DisableOnChangedEvents(JobListingModel jobListing)
        {
            jobListing.EnableEvents = false;
        }
    }
}
