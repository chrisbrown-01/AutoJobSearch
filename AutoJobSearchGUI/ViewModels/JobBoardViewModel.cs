using AutoJobSearchConsoleApp.Models;
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


    public partial class JobBoardViewModel : ViewModelBase
    {
        public delegate void OpenJobListingViewHandler(JobListingModel job, IEnumerable<JobListingModel> jobListings);
        public event OpenJobListingViewHandler? OpenJobListingViewRequest;

        [ObservableProperty]
        private JobBoardQueryModel _jobBoardQueryModel;

        private List<JobListingModel> JobListings { get; set; } = default!;

        [ObservableProperty]
        private List<JobListingModel> _jobListingsDisplayed = default!;

        [ObservableProperty]
        private JobListingModel? _selectedJobListing;

        [ObservableProperty]
        private int _pageIndex;

        [ObservableProperty]
        private int _pageSize;

        public JobBoardViewModel()
        {
            //TestClickCommand = new RelayCommand(TestClick);

            JobBoardQueryModel = new();

            PageIndex = 0;
            PageSize = 25;

            RenderDefaultJobBoard();
        }

        public void RenderDefaultJobBoard()
        {
            PageIndex = 0;
            JobListings = GetAllJobListings().Result;
            JobListingsDisplayed = JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
        }

        public void RenderHiddenJobs()
        {
            PageIndex = 0;
            JobListings = GetHiddenJobListings().Result;
            JobListingsDisplayed = JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
        }

        public void RenderFavouriteJobs()
        {
            PageIndex = 0;
            JobListings = GetFavouriteJobListings().Result;
            JobListingsDisplayed = JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
        }

        public void ExecuteQuery()
        {
            // Try to get some performance improvement by doing the initial simple query directly within the SQLite database
            var result = SQLiteDb.ExecuteJobBoardAdvancedQuery(
                JobBoardQueryModel.IsAppliedTo,
                JobBoardQueryModel.IsInterviewing,
                JobBoardQueryModel.IsRejected,
                JobBoardQueryModel.IsFavourite).Result.AsQueryable();

            // Easiest solution is to then do the rest of the querying within .NET on the previous result
            if (JobBoardQueryModel.SearchTermQueryStringEnabled)
            {
                result = result.Where(x => x.SearchTerm.Contains(JobBoardQueryModel.SearchTermQueryString, StringComparison.OrdinalIgnoreCase));
            }

            if (JobBoardQueryModel.JobDescriptionQueryStringEnabled)
            {
                result = result.Where(x => x.Description.Contains(JobBoardQueryModel.JobDescriptionQueryString, StringComparison.OrdinalIgnoreCase));
            }

            if (JobBoardQueryModel.NotesQueryStringEnabled)
            {
                result = result.Where(x => x.Notes.Contains(JobBoardQueryModel.NotesQueryString, StringComparison.OrdinalIgnoreCase));
            }

            if (JobBoardQueryModel.SearchedOnDateEnabled)
            {
                result = result.Where(x => x.CreatedAt.Date == JobBoardQueryModel.SearchedOnDate.Date);
            }

            if (JobBoardQueryModel.SearchedBetweenDatesEnabled)
            {
                result = result.Where(x =>
                x.CreatedAt.Date >= JobBoardQueryModel.SearchedOnDateStart.Date &&
                x.CreatedAt.Date <= JobBoardQueryModel.SearchedOnDateEnd.Date
                );
            }

            if (JobBoardQueryModel.ScoreEqualsEnabled)
            {
                result = result.Where(x => x.Score == JobBoardQueryModel.ScoreEquals);
            }

            if (JobBoardQueryModel.ScoreRangeEnabled)
            {
                result = result.Where(x =>
                x.Score >= JobBoardQueryModel.ScoreRangeMin &&
                x.Score <= JobBoardQueryModel.ScoreRangeMax);
            }

            if (JobBoardQueryModel.SortByScore)
            {
                if (JobBoardQueryModel.OrderByAscending)
                {
                    result = result.OrderBy(x => x.Score);
                }
                else
                {
                    result = result.OrderByDescending(x => x.Score);
                }
            }
            else if (JobBoardQueryModel.SortBySearchTerm)
            {
                if (JobBoardQueryModel.OrderByAscending)
                {
                    result = result.OrderBy(x => x.SearchTerm);
                }
                else
                {
                    result = result.OrderByDescending(x => x.SearchTerm);
                }
            }
            else if(JobBoardQueryModel.SortByCreatedAt)
            {
                if (JobBoardQueryModel.OrderByAscending)
                {
                    result = result.OrderBy(x => x.CreatedAt);
                }
                else
                {
                    result = result.OrderByDescending(x => x.CreatedAt);
                }
            }
            else
            {
                if (JobBoardQueryModel.OrderByAscending)
                {
                    result = result.OrderBy(x => x.Id);
                }
                else
                {
                    result = result.OrderByDescending(x => x.Id);
                }
            }

            PageIndex = 0;
            JobListings = ConvertQueryToDisplayableModel(result.ToList());
            JobListingsDisplayed = JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
        }

        //public RelayCommand TestClickCommand { get; }
        public void OpenJobListing() // TODO: convert to use RelayCommand?
        {
            if (SelectedJobListing == null) return;
            OpenJobListingViewRequest?.Invoke(SelectedJobListing, JobListings);
        }

        public void HideJob()
        {
            if (SelectedJobListing == null) return;
            SelectedJobListing.IsHidden = true;
            JobListings.Remove(SelectedJobListing);
            JobListingsDisplayed.Remove(SelectedJobListing);
        }

        public void GoToNextPage()
        {
            var jobListings = JobListings.Skip((PageIndex + 1) * PageSize).Take(PageSize);
            if (!jobListings.Any()) return;
            PageIndex++;
            JobListingsDisplayed = jobListings.ToList();
        }

        public void GoToPreviousPage()
        {
            if (PageIndex - 1 < 0) return;
            PageIndex--;
            JobListingsDisplayed = JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
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
                    IsFavourite = job.IsFavourite,
                    IsHidden = job.IsHidden
                };

                jobListings.Add(jobListing);
            }

            return jobListings;
        }

        private static async Task<List<JobListingModel>> GetFavouriteJobListings()
        {
            var jobListings = new List<JobListingModel>();
            var jobs = await SQLiteDb.GetFavouriteJobListings();

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
                    IsFavourite = job.IsFavourite,
                    IsHidden = job.IsHidden
                };

                jobListings.Add(jobListing);
            }

            return jobListings;
        }

        private static async Task<List<JobListingModel>> GetHiddenJobListings()
        {
            var jobListings = new List<JobListingModel>();
            var jobs = await SQLiteDb.GetHiddenJobListings();

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
                    IsFavourite = job.IsFavourite,
                    IsHidden = job.IsHidden
                };

                jobListings.Add(jobListing);
            }

            return jobListings;
        }

        private static async Task<List<JobListingModel>> GetAllJobListings()
        {
            var jobListings = new List<JobListingModel>();
            var jobs = await SQLiteDb.GetAllJobListings();

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
                    IsFavourite = job.IsFavourite,
                    IsHidden = job.IsHidden
                };

                jobListings.Add(jobListing);
            }

            return jobListings;
        }
    }
}
