using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    // TODO: add delete listing method. note that you will need to ensure consistency with the contact-job-id records that cascade delete
    public partial class JobListingViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        public delegate void OpenAddContactViewWithAssociatedJobIdHandler(int id);
        public event OpenAddContactViewWithAssociatedJobIdHandler? OpenAddContactViewWithAssociatedJobIdRequest;

        private List<JobListingModel> JobListings { get; set; } = default!;

        private readonly IDbContext _dbContext;

        [ObservableProperty]
        private JobListingModel _jobListing;

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

        [RelayCommand]
        private void PopulateJobListings(IEnumerable<JobListingModel> jobListings)
        {
            JobListings = jobListings.ToList();
        }

        [RelayCommand]
        private void AddAssociatedContact()
        {
            DisableOnChangedEvents(JobListing);
            OpenAddContactViewWithAssociatedJobIdRequest?.Invoke(JobListing.Id);
        }

        [RelayCommand]
        private async Task GoToPreviousJobAsync()
        {
            var currentIndex = JobListings.IndexOf(JobListing);
            if (currentIndex < 0) return;

            var previousIndex = currentIndex - 1;
            if (previousIndex < 0) return;

            DisableOnChangedEvents(JobListing);

            await OpenJobListingAsync(JobListings[previousIndex]);
        }

        [RelayCommand]
        private async Task GoToNextJobAsync()
        {
            var currentIndex = JobListings.IndexOf(JobListing);
            if (currentIndex < 0) return;

            var nextIndex = currentIndex + 1;
            if (nextIndex >= JobListings.Count) return;

            DisableOnChangedEvents(JobListing);

            await OpenJobListingAsync(JobListings[nextIndex]);
        }

        // TODO: check for all .wait() statements and ensure they are properly used
        [RelayCommand]
        private async Task OpenJobListingByIdAsync(int jobListingId)
        {
            if (JobListings is null || !JobListings.Any())
            {
                JobListings = JobListingHelpers.ConvertJobListingsToJobListingModels(await _dbContext.GetAllJobListingsAsync());
            }

            JobListing = JobListings.Single(x => x.Id == jobListingId);
            await OpenJobListingCommand.ExecuteAsync(JobListing);
        }

        [RelayCommand]
        private async Task OpenJobListingAsync(JobListingModel jobListing)
        {
            if (!jobListing.DetailsPopulated)
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
