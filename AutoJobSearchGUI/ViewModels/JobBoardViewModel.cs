using AutoJobSearchGUI.Models;
using AutoJobSearchShared;
using AutoJobSearchShared.Models;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    // TODO: SQLite concurrency disabling?, database relative pathing best practices + keep all relative paths within shared folder?
    // TODO: SeleniumTesting.Execute(); inside of Job Search menu item
    // TODO: view and models for specifiying search terms and scoring keywords
    // TODO: filtering/querying of job board results, hiding/deleting of objects. ex. get all favourites, dont show rejected


    public partial class JobBoardViewModel : ViewModelBase
    {
        public delegate void OpenJobListingViewHandler(JobListingModel job);
        public event OpenJobListingViewHandler? OpenJobListingViewRequest;

        [ObservableProperty]
        private List<JobListingModel> _jobListings;

        [ObservableProperty]
        private JobListingModel? _selectedJobListing;

        [ObservableProperty]
        private int _pageIndex;

        [ObservableProperty]
        private int _pageSize;

        public JobBoardViewModel()
        {
            //TestClickCommand = new RelayCommand(TestClick);

            PageIndex = 0;
            PageSize = 25;

            JobListings = GetJobListings(PageIndex, PageSize).Result;
        }

        //public RelayCommand TestClickCommand { get; }
        public void TestClick() // TODO: convert to use RelayCommand?
        {
            if (SelectedJobListing == null) return;
            OpenJobListingViewRequest?.Invoke(SelectedJobListing);
        }

        public void GoToNextPage()
        {
            var jobListings = GetJobListings(PageIndex + 1, PageSize).Result;
            if (jobListings.Count == 0) return;
            PageIndex++;
            JobListings = jobListings;
        }

        public void GoToPreviousPage()
        {
            if (PageIndex - 1 < 0) return;
            PageIndex--;
            JobListings = GetJobListings(PageIndex, PageSize).Result;
        }

        private async Task<List<JobListingModel>> GetJobListings(int pageIndex, int pageSize)
        {
            var jobListings = new List<JobListingModel>();
            var jobs = await SQLiteDb.GetJobListings(pageIndex, pageSize);

            foreach (var job in jobs)
            {
                var jobListing = new JobListingModel
                {
                    Id = job.Id,
                    SearchTerm = job.SearchTerm,
                    CreatedAt = job.CreatedAt,
                    Description = job.Description,
                    Score = job.Score,
                    IsAppliedTo = job.IsAppliedTo,
                    IsInterviewing = job.IsInterviewing,
                    IsRejected = job.IsRejected,
                    IsFavourite = job.IsFavourite
                };

                jobListings.Add(jobListing);
            }

            return jobListings;
        }


    }
}
