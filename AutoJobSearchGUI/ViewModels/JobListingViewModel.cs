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

        public void GoToPreviousJob()
        {
            var currentIndex = JobListings.IndexOf(JobListing);
            if (currentIndex < 0) return;

            var previousIndex = currentIndex - 1;
            if (previousIndex < 0) return;

            OpenJobListing(JobListings[previousIndex]);
        }

        public void GoToNextJob()
        {
            var currentIndex = JobListings.IndexOf(JobListing);
            if (currentIndex < 0) return;

            var nextIndex = currentIndex + 1;
            if(nextIndex >= JobListings.Count) return;

            OpenJobListing(JobListings[nextIndex]);
        }

        public void OpenJobListing(JobListingModel jobListing)
        {
            if (jobListing.Id == JobListing.Id) return;

            // TODO: consolidate into single SQL query, make async
            var description = SQLiteDb.GetDescriptionById(jobListing.Id).Result;
            var applicationLinks = SQLiteDb.GetApplicationLinksById(jobListing.Id).Result;
            var notes = SQLiteDb.GetNotesById(jobListing.Id).Result;

            jobListing.Description = description;
            jobListing.ApplicationLinks = applicationLinks;
            jobListing.Notes = notes;

            JobListing = jobListing;
        }
    }
}
