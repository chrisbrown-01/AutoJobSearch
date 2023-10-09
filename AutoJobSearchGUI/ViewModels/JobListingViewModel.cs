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
    public partial class JobListingViewModel : ViewModelBase
    {
        [ObservableProperty]
        private JobListingModel _jobListing;

        private List<JobListingModel> JobListings { get; set; } = default!;

        public JobListingViewModel()
        {
            JobListing = new JobListingModel();
        }

        public void PopulateJobListings(IEnumerable<JobListingModel> jobListings)
        {
            JobListings = jobListings.ToList();
        }

        public async Task GoToPreviousJob()
        {
            var currentIndex = JobListings.IndexOf(JobListing);
            if (currentIndex < 0) return;

            var previousIndex = currentIndex - 1;
            if (previousIndex < 0) return;

            await OpenJobListing(JobListings[previousIndex]);
        }

        public async Task GoToNextJob()
        {
            var currentIndex = JobListings.IndexOf(JobListing);
            if (currentIndex < 0) return;

            var nextIndex = currentIndex + 1;
            if(nextIndex >= JobListings.Count) return;

            await OpenJobListing(JobListings[nextIndex]);
        }

        public async Task OpenJobListing(JobListingModel jobListing)
        {
            if (jobListing.Id == JobListing.Id) return;

            var jobListingDetails = await SQLiteDb.GetJobListingDetailsById(jobListing.Id);
            jobListing.Description = jobListingDetails.Description;
            jobListing.ApplicationLinks = jobListingDetails.ApplicationLinksString;
            jobListing.Notes = jobListingDetails.Notes;

            JobListing = jobListing;
        }
    }
}
