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

            var result = SQLiteDb.ExecuteJobBoardQuery(
                JobBoardQueryModel.IsAppliedTo,
                JobBoardQueryModel.IsInterviewing,
                JobBoardQueryModel.IsRejected,
                JobBoardQueryModel.IsFavourite,
                JobBoardQueryModel.IsHidden).Result.ToList();

            if (JobBoardQueryModel.SearchTermQueryStringEnabled)
            {
                result = result.Where(x => x.SearchTerm.Contains(JobBoardQueryModel.SearchTermQueryString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (JobBoardQueryModel.JobDescriptionQueryStringEnabled)
            {
                result = result.Where(x => x.Description.Contains(JobBoardQueryModel.JobDescriptionQueryString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (JobBoardQueryModel.SearchedOnDateEnabled)
            {
                result = result.Where(x => x.CreatedAt.Date == JobBoardQueryModel.SearchedOnDate.Date).ToList();
            }

            if (JobBoardQueryModel.SearchedBetweenDatesEnabled)
            {
                result = result.Where(x => 
                x.CreatedAt.Date >= JobBoardQueryModel.SearchedOnDateStart.Date &&
                x.CreatedAt.Date <= JobBoardQueryModel.SearchedOnDateEnd.Date
                ).ToList();
            }

            if(JobBoardQueryModel.ScoreEqualsEnabled)
            {
                result = result.Where(x => x.Score == JobBoardQueryModel.ScoreEquals).ToList();
            }

            if (JobBoardQueryModel.ScoreRangeEnabled)
            {
                result = result.Where(x => 
                x.Score >= JobBoardQueryModel.ScoreRangeMin &&
                x.Score <= JobBoardQueryModel.ScoreRangeMax).ToList();
            }

            if(JobBoardQueryModel.SortByScore)
            {
                if (JobBoardQueryModel.OrderByDescending)
                {
                    result = result.OrderByDescending(x => x.Score).ToList();
                }
                else
                {
                    result = result.OrderBy(x => x.Score).ToList();
                }
            }
            else if (JobBoardQueryModel.SortByCreatedAt)
            {
                if (JobBoardQueryModel.OrderByDescending)
                {
                    result = result.OrderByDescending(x => x.CreatedAt).ToList();
                }
                else
                {
                    result = result.OrderBy(x => x.CreatedAt).ToList();
                }
            }
            else if (JobBoardQueryModel.SortBySearchTerm)
            {
                if (JobBoardQueryModel.OrderByDescending)
                {
                    result = result.OrderByDescending(x => x.SearchTerm).ToList();
                }
                else
                {
                    result = result.OrderBy(x => x.SearchTerm).ToList();
                }  
            }
            else
            {
                if (JobBoardQueryModel.OrderByDescending)
                {
                    result = result.OrderByDescending(x => x.Id).ToList();
                }
                else
                {
                    result = result.OrderBy(x => x.Id).ToList();
                }
            }

            // TODO: how to do paging for this
            JobListings = ConvertQueryToDisplayableModel(result);
            JobListingsDisplayed = JobListings.Take(25).ToList();
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

        private List<JobListingModel> ConvertQueryToDisplayableModel(List<AutoJobSearchShared.Models.JobListing> jobs)
        {
            var jobListings = new List<JobListingModel>();

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
