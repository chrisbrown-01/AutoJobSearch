using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared;
using AutoJobSearchShared.Models;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    // TODO: rename view properties so that "string" isn't included
    public partial class JobBoardViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        public delegate void OpenJobListingViewHandler(JobListingModel job);
        public event OpenJobListingViewHandler? OpenJobListingViewRequest;

        [ObservableProperty]
        private JobBoardQueryModel _jobBoardQueryModel;

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
            JobBoardQueryModel = new();
            _dbContext = dbContext;

            PageIndex = 0;
            PageSize = 50;

            RenderDefaultJobBoardViewCommand.Execute(null);
        }

        [RelayCommand]
        private async Task DeleteAllRecordsAsync()
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Confirm Delete All Records",
                "Are you sure you want to delete all job listings from the database? This action cannot be reversed.",
                MsBox.Avalonia.Enums.ButtonEnum.OkAbort,
                MsBox.Avalonia.Enums.Icon.Warning);

            var result = await box.ShowAsync();

            if (result == MsBox.Avalonia.Enums.ButtonResult.Ok)
            {
                await _dbContext.DeleteAllJobListingsAsync();
                await RenderDefaultJobBoardViewAsync();
            }           
        }

        [RelayCommand]
        private async Task RenderDefaultJobBoardViewAsync()
        {
            PageIndex = 0;
            Singletons.JobListings = await GetAllJobListingsAsync(); 
            JobListingsDisplayed = Singletons.JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            EnableOnChangedEvents(JobListingsDisplayed);

            JobBoardQueryModel = new();
        }

        [RelayCommand]
        private async Task RenderHiddenJobsAsync()
        {
            PageIndex = 0;
            Singletons.JobListings = await GetHiddenJobListingsAsync();
            JobListingsDisplayed = Singletons.JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            EnableOnChangedEvents(JobListingsDisplayed);
        }

        [RelayCommand]
        private async Task RenderFavouriteJobsAsync()
        {
            PageIndex = 0;
            Singletons.JobListings = await GetFavouriteJobListingsAsync();
            JobListingsDisplayed = Singletons.JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            EnableOnChangedEvents(JobListingsDisplayed);
        }

        [RelayCommand]
        private async Task ExecuteQueryAsync()
        {
            // TODO: change ascending to descending
            var result = await _dbContext.ExecuteJobListingQueryAsync(
               JobBoardQueryModel.ColumnFiltersEnabled,
               JobBoardQueryModel.IsAppliedTo,
               JobBoardQueryModel.IsInterviewing,
               JobBoardQueryModel.IsRejected,
               JobBoardQueryModel.IsFavourite);

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
                result = result.Where(x => x.Score == JobBoardQueryModel.ScoreEquals); // TODO: add nullable and null checking, ClickMode handling in view
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
            else if (JobBoardQueryModel.SortByCreatedAt)
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
            Singletons.JobListings = JobListingHelpers.ConvertJobListingsToJobListingModels(result);
            JobListingsDisplayed = Singletons.JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            EnableOnChangedEvents(JobListingsDisplayed);
        }

        [RelayCommand]
        private void OpenJobListing()
        {
            if (SelectedJobListing == null) return;
            DisableOnChangedEvents(JobListingsDisplayed);
            OpenJobListingViewRequest?.Invoke(SelectedJobListing);
        }

        [RelayCommand]
        private void HideJob()
        {
            if (SelectedJobListing == null) return;
            SelectedJobListing.IsHidden = true;
            Singletons.JobListings.Remove(SelectedJobListing);
            JobListingsDisplayed.Remove(SelectedJobListing);
        }

        [RelayCommand]
        private void GoToNextPage()
        {
            var jobListings = Singletons.JobListings.Skip((PageIndex + 1) * PageSize).Take(PageSize);
            if (!jobListings.Any()) return;

            DisableOnChangedEvents(JobListingsDisplayed);
            PageIndex++;
            JobListingsDisplayed = jobListings.ToList();
            EnableOnChangedEvents(JobListingsDisplayed);
        }

        [RelayCommand]
        private void GoToPreviousPage()
        {
            if (PageIndex - 1 < 0) return;

            DisableOnChangedEvents(JobListingsDisplayed);
            PageIndex--;
            JobListingsDisplayed = Singletons.JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            EnableOnChangedEvents(JobListingsDisplayed);
        }

        /// <summary>
        /// Allows events to fire. This method should be called after the view model properties have been fully instantiated.
        /// </summary>
        /// <param name="jobListingModels"></param>
        private void EnableOnChangedEvents(IEnumerable<JobListingModel> jobListingModels)
        {
            foreach (var jobListingModel in jobListingModels)
            {
                jobListingModel.EnableEvents = true;
            }
        }

        /// <summary>
        /// Prevent events from firing. This method should be called in preparation of instantiating new view model properties.
        /// </summary>
        /// <param name="jobListingModels"></param>
        private void DisableOnChangedEvents(IEnumerable<JobListingModel> jobListingModels)
        {
            foreach (var jobListingModel in jobListingModels)
            {
                jobListingModel.EnableEvents = false;
            }
        }

        private async Task<List<JobListingModel>> GetFavouriteJobListingsAsync()
        {
            var jobs = await _dbContext.GetFavouriteJobListingsAsync();
            return JobListingHelpers.ConvertJobListingsToJobListingModels(jobs);
        }

        private async Task<List<JobListingModel>> GetHiddenJobListingsAsync()
        {
            var jobs = await _dbContext.GetHiddenJobListingsAsync();
            return JobListingHelpers.ConvertJobListingsToJobListingModels(jobs);
        }

        private async Task<List<JobListingModel>> GetAllJobListingsAsync()
        {
            var jobs = await _dbContext.GetAllJobListingsAsync();
            return JobListingHelpers.ConvertJobListingsToJobListingModels(jobs);
        }
    }
}
