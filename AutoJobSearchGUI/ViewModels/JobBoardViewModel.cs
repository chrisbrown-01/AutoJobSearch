using AutoJobSearchConsoleApp.Models;
using AutoJobSearchGUI.Data;
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


    public partial class JobBoardViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        public delegate Task OpenJobListingViewHandler(JobListingModel job, IEnumerable<JobListingModel> jobListings);
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

        private readonly IDbContext _dbContext;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public JobBoardViewModel() // For View previewer only
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            // Design-time data setup
            JobBoardQueryModel = new();
        }

        public JobBoardViewModel(IDbContext dbContext)
        {
            //TestClickCommand = new RelayCommand(TestClick);

            JobBoardQueryModel = new();
            _dbContext = dbContext;

            PageIndex = 0;
            PageSize = 25; // TODO: move out to config file

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            RenderDefaultJobBoardView();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public async Task RenderDefaultJobBoardView()
        {
            PageIndex = 0;
            JobListings = await GetAllJobListings();
            JobListingsDisplayed = JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
        }

        public async Task RenderHiddenJobs()
        {
            PageIndex = 0;
            JobListings = await GetHiddenJobListings();
            JobListingsDisplayed = JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
        }

        public async Task RenderFavouriteJobs()
        {
            PageIndex = 0;
            JobListings = await GetFavouriteJobListings();
            JobListingsDisplayed = JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
        }

        public async Task ExecuteQuery()
        {
            // Try to get some performance improvement by doing the initial simple query directly within the SQLite database
            var result = await _dbContext.ExecuteJobListingQueryAsync(
                JobBoardQueryModel.IsAppliedTo,
                JobBoardQueryModel.IsInterviewing,
                JobBoardQueryModel.IsRejected,
                JobBoardQueryModel.IsFavourite);

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
            JobListings = ConvertJobListingsToJobListingModels(result);
            JobListingsDisplayed = JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
        }

        public void OpenJobListing() 
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

        private async Task<List<JobListingModel>> GetFavouriteJobListings() 
        {
            var jobs = await _dbContext.GetFavouriteJobListingsAsync(); 
            return ConvertJobListingsToJobListingModels(jobs);
        }

        private async Task<List<JobListingModel>> GetHiddenJobListings()
        {
            var jobs = await _dbContext.GetHiddenJobListingsAsync();
            return ConvertJobListingsToJobListingModels(jobs);
        }

        private List<JobListingModel> ConvertJobListingsToJobListingModels(IEnumerable<AutoJobSearchShared.Models.JobListing> jobs) // TODO: remove namespace from model, maybe improve naming
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

        private async Task<List<JobListingModel>> GetAllJobListings()
        {
            var jobs = await _dbContext.GetAllJobListingsAsync();
            return ConvertJobListingsToJobListingModels(jobs);
        }
    }
}
