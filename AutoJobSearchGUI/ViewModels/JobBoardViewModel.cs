﻿using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class JobBoardViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        private const int DEFAULT_PAGE_SIZE = 50;

        private readonly IDbContext _dbContext;

        [ObservableProperty]
        private JobBoardQueryModel _jobBoardQueryModel;

        [ObservableProperty]
        private List<JobListingModel> _jobListingsDisplayed = default!;

        [ObservableProperty]
        private int _pageIndex;

        [ObservableProperty]
        private int _pageSize = DEFAULT_PAGE_SIZE;

        [ObservableProperty]
        private JobListingModel? _selectedJobListing;

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

            RenderDefaultJobBoardViewCommand.Execute(null);
        }

        public delegate void OpenJobListingViewByIdHandler(int jobId, bool changedViaPreviousOrForwardButton);

        public delegate void OpenJobListingViewHandler(JobListingModel job);

        public delegate void ResetViewHistoryHandler();

        public event OpenJobListingViewByIdHandler? OpenJobListingViewByIdRequest;
        public event OpenJobListingViewHandler? OpenJobListingViewRequest;
        public event ResetViewHistoryHandler? ResetViewHistoryRequest;

        public void UpdateJobBoard()
        {
            DisableOnChangedEvents(JobListingsDisplayed);
            JobListingsDisplayed = Singletons.JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            EnableOnChangedEvents(JobListingsDisplayed);
        }

        [RelayCommand]
        private async Task CreateJobAsync()
        {
            var newJob = await _dbContext.CreateJobListingAsync();
            var newJobListingModel = JobListingHelpers.ConvertJobListingToJobListingModel(newJob);
            Singletons.JobListings.Add(newJobListingModel);
            JobListingsDisplayed.Add(newJobListingModel);
            SelectedJobListing = newJobListingModel;
            OpenJobListing();
        }

        [RelayCommand]
        private async Task ClearAllJobListingStatuses()
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Confirm Clear All Statuses",
                "This will change all statuses for all displayed job listings (Applied, Favourite, Hidden, etc.) to the default unchecked value. " +
                "Are you sure you want to proceed?",
                MsBox.Avalonia.Enums.ButtonEnum.OkAbort,
                MsBox.Avalonia.Enums.Icon.Warning);

            var result = await box.ShowAsync();

            if (result != MsBox.Avalonia.Enums.ButtonResult.Ok)
                return;

            foreach(var job in JobListingsDisplayed)
            {
                JobListingHelpers.ClearAllJobListingStatuses(job);
            }
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

            if (result != MsBox.Avalonia.Enums.ButtonResult.Ok) return;

            await _dbContext.DeleteAllJobListingsAsync();
            await RenderDefaultJobBoardViewAsync();
        }

        [RelayCommand]
        private async Task DeleteJobAsync()
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Confirm Delete Job",
                "Are you sure you want to delete this job listing? This action cannot be reversed.",
                MsBox.Avalonia.Enums.ButtonEnum.OkAbort,
                MsBox.Avalonia.Enums.Icon.Warning);

            var result = await box.ShowAsync();

            if (result != MsBox.Avalonia.Enums.ButtonResult.Ok) return;

            if (SelectedJobListing == null) return;
            await _dbContext.DeleteJobListingAsync(SelectedJobListing.Id);
            Singletons.JobListings.Remove(SelectedJobListing);
            JobListingsDisplayed.Remove(SelectedJobListing);

            SelectedJobListing = null;

            ResetViewHistoryRequest?.Invoke();
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

        [RelayCommand]
        private async Task ExecuteQueryAsync()
        {
            var result = await _dbContext.ExecuteJobListingQueryAsync(
               JobBoardQueryModel.JobDescriptionQueryStringEnabled,
               JobBoardQueryModel.NotesQueryStringEnabled,
               JobBoardQueryModel.ColumnFiltersEnabled,
               JobBoardQueryModel.IsToBeAppliedTo,
               JobBoardQueryModel.IsAppliedTo,
               JobBoardQueryModel.IsInterviewing,
               JobBoardQueryModel.IsNegotiating,
               JobBoardQueryModel.IsRejected,
               JobBoardQueryModel.IsDeclinedOffer,
               JobBoardQueryModel.IsAcceptedOffer,
               JobBoardQueryModel.IsFavourite);

            if (JobBoardQueryModel.SearchTermQueryStringEnabled)
            {
                result = result.Where(x => x.SearchTerm.Contains(JobBoardQueryModel.SearchTermQueryString.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (JobBoardQueryModel.JobDescriptionQueryStringEnabled)
            {
                result = result.Where(x => x.Description.Contains(JobBoardQueryModel.JobDescriptionQueryString.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (JobBoardQueryModel.NotesQueryStringEnabled)
            {
                result = result.Where(x => x.Notes.Contains(JobBoardQueryModel.NotesQueryString.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (JobBoardQueryModel.CreatedAtDateEnabled)
            {
                result = result.Where(x => x.CreatedAt.Date == JobBoardQueryModel.CreatedAtDate.Date);
            }

            if (JobBoardQueryModel.CreatedBetweenDatesEnabled)
            {
                result = result.Where(x =>
                x.CreatedAt.Date >= JobBoardQueryModel.CreatedBetweenDateStart.Date &&
                x.CreatedAt.Date <= JobBoardQueryModel.CreatedBetweenDateEnd.Date);
            }

            if (JobBoardQueryModel.ModifiedAtDateEnabled)
            {
                result = result.Where(x => x.StatusModifiedAt.Date == JobBoardQueryModel.ModifiedAtDate.Date);
            }

            if (JobBoardQueryModel.ModifiedBetweenDatesEnabled)
            {
                result = result.Where(x =>
                x.StatusModifiedAt.Date >= JobBoardQueryModel.ModifiedBetweenDateStart.Date &&
                x.StatusModifiedAt.Date <= JobBoardQueryModel.ModifiedBetweenDateEnd.Date);
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
            else if (JobBoardQueryModel.SortByModifiedAt)
            {
                if (JobBoardQueryModel.OrderByAscending)
                {
                    result = result.OrderBy(x => x.StatusModifiedAt);
                }
                else
                {
                    result = result.OrderByDescending(x => x.StatusModifiedAt);
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

        private async Task<List<JobListingModel>> GetAllJobListingsAsync()
        {
            var jobs = await _dbContext.GetAllJobListingsAsync();
            return JobListingHelpers.ConvertJobListingsToJobListingModels(jobs);
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

        [RelayCommand]
        private void HideJob()
        {
            if (SelectedJobListing == null) return;
            SelectedJobListing.IsHidden = true;
            Singletons.JobListings.Remove(SelectedJobListing);
            JobListingsDisplayed.Remove(SelectedJobListing);
        }

        partial void OnPageSizeChanged(int value)
        {
            if (value < 1 || value > 100)
            {
                PageSize = DEFAULT_PAGE_SIZE;
                return;
            }

            UpdateJobBoard();
        }

        [RelayCommand]
        private async Task OpenJobByIdAsync(string jobIdTextBoxInput)
        {
            if (Int32.TryParse(jobIdTextBoxInput, out int jobId))
            {
                // Make sure the job ID exists before proceeding
                var allJobListings = await _dbContext.GetAllJobListingsAsync();
                var allJobIds = allJobListings.Select(x => x.Id);

                if (allJobIds is null || !allJobIds.Contains(jobId))
                    return;

                OpenJobListingViewByIdRequest?.Invoke(jobId, false);
            }
        }
        [RelayCommand]
        private void OpenJobListing()
        {
            if (SelectedJobListing == null) return;
            DisableOnChangedEvents(JobListingsDisplayed);
            OpenJobListingViewRequest?.Invoke(SelectedJobListing);
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
        private async Task RenderFavouriteJobsAsync()
        {
            PageIndex = 0;
            Singletons.JobListings = await GetFavouriteJobListingsAsync();
            JobListingsDisplayed = Singletons.JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            EnableOnChangedEvents(JobListingsDisplayed);
        }

        [RelayCommand]
        private async Task RenderHiddenJobsAsync()
        {
            PageIndex = 0;
            Singletons.JobListings = await GetHiddenJobListingsAsync();
            JobListingsDisplayed = Singletons.JobListings.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            EnableOnChangedEvents(JobListingsDisplayed);
        }
    }
}