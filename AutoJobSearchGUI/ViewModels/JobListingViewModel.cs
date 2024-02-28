﻿using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.Services;
using AutoJobSearchGUI.Views;
using AutoJobSearchShared;
using AutoJobSearchShared.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class JobListingViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        private const string EDIT_BUTTON_DEFAULT_COLOUR = "Gray";
        private const string EDIT_BUTTON_ENABLED_COLOUR = "YellowGreen";

        public delegate void CreateNewContactWithAssociatedJobIdHandler(int jobId);
        public event CreateNewContactWithAssociatedJobIdHandler? CreateNewContactWithAssociatedJobIdRequest;

        public delegate void ChangeViewToContactHandler(int contactId);
        public event ChangeViewToContactHandler? ChangeViewToContactRequest;

        public delegate void UpdateJobBoardViewHandler();
        public event UpdateJobBoardViewHandler? UpdateJobBoardViewRequest;

        public delegate void OpenJobBoardViewHandler();
        public event OpenJobBoardViewHandler? OpenJobBoardViewRequest;

        private readonly IDbContext _dbContext;

        [ObservableProperty]
        private string _editButtonColour = EDIT_BUTTON_DEFAULT_COLOUR;

        [ObservableProperty]
        private bool _isEditModeEnabled;

        [ObservableProperty]
        private JobListingModel _jobListing;

        [ObservableProperty]
        private IEnumerable<int> _associatedContactIds = new List<int>();

        [ObservableProperty]
        private bool _isNavigateToContactButtonEnabled;

        [ObservableProperty]
        private int _selectedContactId;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public JobListingViewModel() // For View previewer only
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            JobListing = new JobListingModel();
        }

        public JobListingViewModel(IDbContext dbContext)
        {
            JobListing = new JobListingModel();
            _dbContext = dbContext;
        }

        [RelayCommand]
        private async Task ViewFileAsync()
        {
            // TODO: test for linux & mac
            //Process.Start(new ProcessStartInfo(hashedFilePath) { UseShellExecute = true }); 
        }

        [RelayCommand]
        private async Task UploadFileAsync() // TODO: try/catch, const strings, linux testing
        {
            var filesService = App.Current?.Services?.GetService<IFilesService>();
            if (filesService is null) return;

            var file = await filesService.OpenFileAsync();
            if (file is null) return;

            var filePath = file.TryGetLocalPath();

            if (String.IsNullOrEmpty(filePath)) return;

            var fileExtension = Path.GetExtension(filePath);

            if (String.IsNullOrEmpty(fileExtension)) return;

            var jobListingAssociatedFilesDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JobListingAssociatedFiles");
            Directory.CreateDirectory(jobListingAssociatedFilesDirectoryPath);
       
            using var stream = File.OpenRead(filePath);
            using var md5 = MD5.Create();
            var hash = await md5.ComputeHashAsync(stream);
            var hashString = Convert.ToHexString(hash);

            var hashedFileNameAndExtension = $"{hashString}{fileExtension}";

            var hashedFilePath = Path.Join(jobListingAssociatedFilesDirectoryPath, hashedFileNameAndExtension);

            File.Copy(filePath, hashedFilePath, true);

            // TODO: customize for specific file. currently only handles resumes.
            if(JobListing.JobListingAssociatedFiles == null)
            {
                var jobListingAssociatedFiles = new JobListingAssociatedFiles();

                jobListingAssociatedFiles.Id = JobListing.Id;
                jobListingAssociatedFiles.Resume = hashedFileNameAndExtension;

                JobListing.JobListingAssociatedFiles = jobListingAssociatedFiles;

                await _dbContext.CreateJobListingAssociatedFilesAsync(jobListingAssociatedFiles);
            }
        }

        [RelayCommand]
        private void ViewContact()
        {
            if (SelectedContactId < 1) return;

            DisableOnChangedEvents(JobListing);
            ChangeViewToContactRequest?.Invoke(SelectedContactId);
        }

        [RelayCommand]
        private void AddAssociatedContact()
        {
            DisableOnChangedEvents(JobListing);
            CreateNewContactWithAssociatedJobIdRequest?.Invoke(JobListing.Id);
        }

        [RelayCommand]
        private void ToggleEditButtonColour()
        {
            IsEditModeEnabled = !IsEditModeEnabled;

        }

        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditModeEnabled = !IsEditModeEnabled;
            SetEditButtonColour();
        }

        private void SetEditButtonColour()
        {
            if (IsEditModeEnabled)
            {
                EditButtonColour = EDIT_BUTTON_ENABLED_COLOUR;
            }
            else
            {
                EditButtonColour = EDIT_BUTTON_DEFAULT_COLOUR;
            }
        }

        [RelayCommand]
        private async Task CreateJobAsync()
        {
            DisableOnChangedEvents(JobListing);

            var newJob = await _dbContext.CreateJobAsync();
            var newJobListingModel = JobListingHelpers.ConvertJobListingToJobListingModel(newJob);
            Singletons.JobListings.Add(newJobListingModel);
            UpdateJobBoardViewRequest?.Invoke();
            await OpenJobListingAsync(newJobListingModel); 
        }

        [RelayCommand]
        private async Task DeleteJobAsync()
        {
            JobListingModel? nextJobToDisplay;

            var currentIndex = Singletons.JobListings.IndexOf(JobListing);

            var nextJob = Singletons.JobListings.ElementAtOrDefault(currentIndex + 1);
            var previousJob = Singletons.JobListings.ElementAtOrDefault(currentIndex - 1);

            if (nextJob != null) // Try to choose the next job as the new one to display.
            {
                nextJobToDisplay = nextJob;
            }
            else if (previousJob != null) // If next job is not available, try to choose the previous one.
            {
                nextJobToDisplay = previousJob;
            }
            else // Otherwise we have no job at all to display.
            {
                nextJobToDisplay = null;
            }

            DisableOnChangedEvents(JobListing);

            await _dbContext.DeleteJobAsync(JobListing.Id);
            Singletons.JobListings.Remove(JobListing);
            UpdateJobBoardViewRequest?.Invoke();

            if (nextJobToDisplay != null)
            {
                await OpenJobListingAsync(nextJobToDisplay);
            }
            else
            {
                OpenJobBoardViewRequest?.Invoke(); // Return to Job Board view if no jobs are available to display.
            }
        }

        [RelayCommand]
        private async Task GoToPreviousJobAsync()
        {
            var currentIndex = Singletons.JobListings.IndexOf(JobListing);
            if (currentIndex < 0) return;

            var previousIndex = currentIndex - 1;
            if (previousIndex < 0) return;

            DisableOnChangedEvents(JobListing);
            await OpenJobListingAsync(Singletons.JobListings[previousIndex]);
        }

        [RelayCommand]
        private async Task GoToNextJobAsync()
        {
            var currentIndex = Singletons.JobListings.IndexOf(JobListing);
            if (currentIndex < 0) return;

            var nextIndex = currentIndex + 1;
            if (nextIndex >= Singletons.JobListings.Count) return;

            DisableOnChangedEvents(JobListing);
            await OpenJobListingAsync(Singletons.JobListings[nextIndex]);
        }

        [RelayCommand]
        private async Task OpenJobListingByIdAsync(int jobListingId)
        {
            JobListing = Singletons.JobListings.Single(x => x.Id == jobListingId);
            await OpenJobListingCommand.ExecuteAsync(JobListing);
        }

        [RelayCommand]
        private async Task OpenJobListingAsync(JobListingModel jobListing)
        {
            SelectedContactId = -1; // Prevent erroneous navigations to contact when updating the view model.
            IsEditModeEnabled = false;
            SetEditButtonColour();

            if (!jobListing.DetailsPopulated)
            {
                var jobListingDetails = await _dbContext.GetJobListingDetailsByIdAsync(jobListing.Id);
                jobListing.Description = jobListingDetails.Description;
                jobListing.ApplicationLinks = jobListingDetails.ApplicationLinksString;
                jobListing.Notes = jobListingDetails.Notes;
                jobListing.JobListingAssociatedFiles = jobListingDetails.JobListingAssociatedFiles;

                jobListing.DetailsPopulated = true;
            }

            AssociatedContactIds = Singletons.Contacts.Where(x => x.JobListingIds.Contains(jobListing.Id)).Select(x => x.Id);
            IsNavigateToContactButtonEnabled = AssociatedContactIds.Any();

            JobListing = jobListing;
            EnableOnChangedEvents(JobListing);
        }

        /// <summary>
        /// Allows events to fire. This method should be called after the view model properties have been fully instantiated.
        /// </summary>
        /// <param name="jobListing"></param>
        private void EnableOnChangedEvents(JobListingModel jobListing)
        {
            jobListing.EnableEvents = true;
        }

        /// <summary>
        /// Prevent events from firing. This method should be called in preparation of instantiating new view model properties.
        /// </summary>
        /// <param name="jobListing"></param>
        private void DisableOnChangedEvents(JobListingModel jobListing)
        {
            jobListing.EnableEvents = false;
        }
    }
}
