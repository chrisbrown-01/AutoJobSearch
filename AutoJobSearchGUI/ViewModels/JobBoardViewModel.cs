using AutoJobSearchGUI.Models;
using AutoJobSearchShared;
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
    // TODO: Menu or tab controls and seperate views for modifiying specific row item, save changes to db features
    // TODO: SeleniumTesting.Execute(); inside of Job Search menu item
    // TODO: view and models for specifiying search terms and scoring keywords
    // TODO: filtering/querying of job board results, hiding/deleting of objects. ex. get all favourites, dont show rejected


    public partial class JobBoardViewModel : ViewModelBase
    {
        public delegate void OpenJobListingViewHandler(JobListingModel job);
        public event OpenJobListingViewHandler? OpenJobListingViewRequest;

        //public RelayCommand TestClickCommand { get; }
        public void TestClick() // TODO: convert to use RelayCommand?
        {
            if (SelectedJobListing == null) return;
            OpenJobListingViewRequest?.Invoke(SelectedJobListing);
        }

        [ObservableProperty]
        private List<JobListingModel> _jobListings;

        [ObservableProperty]
        private JobListingModel? _selectedJobListing;

        public JobBoardViewModel()
        {
            //TestClickCommand = new RelayCommand(TestClick);

            JobListings = new();
            var jobs = SQLiteDb.GetAllJobListings().Result.Take(25);

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

                JobListings.Add(jobListing);
            }
        }


    }
}
