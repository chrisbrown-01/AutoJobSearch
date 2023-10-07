﻿using AutoJobSearchConsoleApp.Models;
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
        private JobBoardQueryModel _jobBoardQueryModel;

        private List<JobListingModel> JobListings { get; set; }

        [ObservableProperty]
        private List<JobListingModel> _jobListingsDisplayed;

        [ObservableProperty]
        private JobListingModel? _selectedJobListing;

        [ObservableProperty]
        private int _pageIndex;

        [ObservableProperty]
        private int _pageSize;

        [ObservableProperty]
        private bool _isFavouritesFilterEnabled;

        partial void OnIsFavouritesFilterEnabledChanged(bool value)
        {
            if (value == true)
            {
                JobListingsDisplayed = JobListings.Where(x => x.IsFavourite == value).ToList();
            }

            else JobListingsDisplayed = JobListings;
        }

        public JobBoardViewModel()
        {
            //TestClickCommand = new RelayCommand(TestClick);

            JobBoardQueryModel = new();

            PageIndex = 0;
            PageSize = 25;

            JobListings = GetJobListings(PageIndex, PageSize).Result;
            JobListingsDisplayed = JobListings; // TODO: convert to void method call
        }

        public void ExecuteQuery()
        {
            var test = JobBoardQueryModel;

            //var jobListingQueryValues = new AutoJobSearchShared.Models.JobListing()
            //{

            //};


            //SQLiteDb.ExecuteJobBoardQuery
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
            JobListingsDisplayed = JobListings;
        }

        public void GoToPreviousPage()
        {
            if (PageIndex - 1 < 0) return;
            PageIndex--;
            JobListings = GetJobListings(PageIndex, PageSize).Result;
            JobListingsDisplayed = JobListings;
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
                    // TODO: skip IsHidden properties
                };

                jobListings.Add(jobListing);
            }

            return jobListings;
        }
    }
}