using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.Services;
using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Helpers;
using AutoJobSearchShared.Models;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class JobListingViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        private const int NUMBER_OF_MOST_COMMON_WORDS_TO_DISPLAY = 20;
        private const string EDIT_BUTTON_DEFAULT_FONT_WEIGHT = "Regular";
        private const string EDIT_BUTTON_ENABLED_FONT_WEIGHT = "ExtraBold";

        public delegate void CreateNewContactWithAssociatedJobIdHandler(int jobId);

        public event CreateNewContactWithAssociatedJobIdHandler? CreateNewContactWithAssociatedJobIdRequest;

        public delegate void ChangeViewToContactHandler(int contactId);

        public event ChangeViewToContactHandler? ChangeViewToContactRequest;

        public delegate void UpdateJobBoardViewHandler();

        public event UpdateJobBoardViewHandler? UpdateJobBoardViewRequest;

        public delegate void OpenJobBoardViewHandler();

        public event OpenJobBoardViewHandler? OpenJobBoardViewRequest;

        private readonly IDbContext _dbContext;

        public static JobListingsAssociatedFilesStringField Resume => JobListingsAssociatedFilesStringField.Resume;
        public static JobListingsAssociatedFilesStringField CoverLetter => JobListingsAssociatedFilesStringField.CoverLetter;
        public static JobListingsAssociatedFilesStringField File1 => JobListingsAssociatedFilesStringField.File1;
        public static JobListingsAssociatedFilesStringField File2 => JobListingsAssociatedFilesStringField.File2;
        public static JobListingsAssociatedFilesStringField File3 => JobListingsAssociatedFilesStringField.File3;

        private const string ASSOCIATED_FILES_DIRECTORY_NAME = "JobListingAssociatedFiles";

        private readonly string _associatedFilesDirectoryPath;

        [ObservableProperty]
        private string _editButtonFontWeight = EDIT_BUTTON_DEFAULT_FONT_WEIGHT;

        [ObservableProperty]
        private bool _isEditModeEnabled;

        [ObservableProperty]
        private bool _isViewFilesEnabled;

        [ObservableProperty]
        private bool _isViewResumeEnabled;

        [ObservableProperty]
        private bool _isViewCoverLetterEnabled;

        [ObservableProperty]
        private bool _isViewFile1Enabled;

        [ObservableProperty]
        private bool _isViewFile2Enabled;

        [ObservableProperty]
        private bool _isViewFile3Enabled;

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

            _associatedFilesDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ASSOCIATED_FILES_DIRECTORY_NAME); // TODO: ensure this works properly in the release version.
            Directory.CreateDirectory(_associatedFilesDirectoryPath); // This method is automatically skipped if the directory already exists.
        }

        [RelayCommand]
        private async Task DisplayMostCommonWordsAsync()
        {
            var mostCommonWords = StringHelpers.HighestFrequencyWordsInString(JobListing.Description, StringHelpers.CommonWords, NUMBER_OF_MOST_COMMON_WORDS_TO_DISPLAY);

            var box = MessageBoxManager.GetMessageBoxStandard(
                    "Most Common Words In Job Description",
                    string.Join(Environment.NewLine, mostCommonWords),
                    MsBox.Avalonia.Enums.ButtonEnum.Ok);

            await box.ShowAsync();
        }

        [RelayCommand]
        private async Task ViewFileAsync(JobListingsAssociatedFilesStringField fileField) // TODO: test for linux & mac
        {
            if (JobListing.JobListingAssociatedFiles == null)
            {
                Log.Warning("The ViewFileAsync method was executed but the JobListing.JobListingAssociatedFiles reference was null.");
                await DisplayViewFileErrorMessageAsync();
                return;
            }

            string fileToOpen;

            switch (fileField)
            {
                case JobListingsAssociatedFilesStringField.Resume:
                    fileToOpen = JobListing.JobListingAssociatedFiles.Resume;
                    await AttemptToOpenFileAsync(fileToOpen);
                    break;

                case JobListingsAssociatedFilesStringField.CoverLetter:
                    fileToOpen = JobListing.JobListingAssociatedFiles.CoverLetter;
                    await AttemptToOpenFileAsync(fileToOpen);
                    break;

                case JobListingsAssociatedFilesStringField.File1:
                    fileToOpen = JobListing.JobListingAssociatedFiles.File1;
                    await AttemptToOpenFileAsync(fileToOpen);
                    break;

                case JobListingsAssociatedFilesStringField.File2:
                    fileToOpen = JobListing.JobListingAssociatedFiles.File2;
                    await AttemptToOpenFileAsync(fileToOpen);
                    break;

                case JobListingsAssociatedFilesStringField.File3:
                    fileToOpen = JobListing.JobListingAssociatedFiles.File3;
                    await AttemptToOpenFileAsync(fileToOpen);
                    break;

                default:
                    Log.Warning($"{nameof(fileField)} enum type could not be resolved when attempting to open file for viewing.");
                    break;
            }

            async Task AttemptToOpenFileAsync(string fileToOpen)
            {
                if (string.IsNullOrWhiteSpace(fileToOpen))
                {
                    Log.Warning("ViewFileAsync method attempted to open a file where the name string was null or whitespace.");
                    await DisplayViewFileErrorMessageAsync();
                    return;
                }

                var completePath = Path.Combine(_associatedFilesDirectoryPath, fileToOpen);

                if (!File.Exists(completePath))
                {
                    Log.Warning("ViewFileAsync method attempted to open a file that does not exist.");
                    await DisplayViewFileErrorMessageAsync();
                    return;
                }

                try
                {
                    Process.Start(new ProcessStartInfo(completePath) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    Log.Error("Exception thrown when trying to start a process for viewing a file. Exception details: {@ex}", ex);
                    await DisplayViewFileErrorMessageAsync();
                }
            }

            async Task DisplayViewFileErrorMessageAsync()
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "File Cannot Be Viewed",
                    "An issue was encountered when trying to view the file.",
                    MsBox.Avalonia.Enums.ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error);

                await box.ShowAsync();
            }
        }

        [RelayCommand]
        private async Task UploadFileAsync(JobListingsAssociatedFilesStringField fileField) // TODO: test for linux & mac
        {
            var filesService = App.Current?.Services?.GetService<IFilesService>();

            if (filesService is null)
            {
                Log.Warning("The UploadFileAsync method was executed but the IFilesService reference was null.");
                await DisplayUploadFileErrorMessageAsync();
                return;
            }

            var file = await filesService.OpenFileAsync();
            if (file is null) return;

            var filePath = file.TryGetLocalPath();

            if (String.IsNullOrWhiteSpace(filePath))
            {
                Log.Warning("The UploadFileAsync method was executed but the file path string was null or white space.");
                await DisplayUploadFileErrorMessageAsync();
                return;
            }

            var fileExtension = Path.GetExtension(filePath);

            if (String.IsNullOrWhiteSpace(fileExtension))
            {
                Log.Warning("The UploadFileAsync method was executed but the file extension string was null or white space.");
                await DisplayUploadFileErrorMessageAsync();
                return;
            }

            var hashedFile = await HashFileAndSaveToLocalDirectoryAsync(filePath, fileExtension);

            if (JobListing.JobListingAssociatedFiles == null)
            {
                var jobListingAssociatedFiles = new JobListingAssociatedFiles();

                jobListingAssociatedFiles.Id = JobListing.Id;

                switch (fileField)
                {
                    case JobListingsAssociatedFilesStringField.Resume:
                        jobListingAssociatedFiles.Resume = hashedFile;
                        break;

                    case JobListingsAssociatedFilesStringField.CoverLetter:
                        jobListingAssociatedFiles.CoverLetter = hashedFile;
                        break;

                    case JobListingsAssociatedFilesStringField.File1:
                        jobListingAssociatedFiles.File1 = hashedFile;
                        break;

                    case JobListingsAssociatedFilesStringField.File2:
                        jobListingAssociatedFiles.File2 = hashedFile;
                        break;

                    case JobListingsAssociatedFilesStringField.File3:
                        jobListingAssociatedFiles.File3 = hashedFile;
                        break;

                    default:
                        throw new ArgumentException($"{nameof(fileField)} enum type could not be resolved");
                }

                JobListing.JobListingAssociatedFiles = jobListingAssociatedFiles;

                await _dbContext.CreateJobListingAssociatedFilesAsync(jobListingAssociatedFiles);
            }
            else
            {
                switch (fileField)
                {
                    case JobListingsAssociatedFilesStringField.Resume:
                        JobListing.JobListingAssociatedFiles.Resume = hashedFile;
                        break;

                    case JobListingsAssociatedFilesStringField.CoverLetter:
                        JobListing.JobListingAssociatedFiles.CoverLetter = hashedFile;
                        break;

                    case JobListingsAssociatedFilesStringField.File1:
                        JobListing.JobListingAssociatedFiles.File1 = hashedFile;
                        break;

                    case JobListingsAssociatedFilesStringField.File2:
                        JobListing.JobListingAssociatedFiles.File2 = hashedFile;
                        break;

                    case JobListingsAssociatedFilesStringField.File3:
                        JobListing.JobListingAssociatedFiles.File3 = hashedFile;
                        break;

                    default:
                        throw new ArgumentException($"{nameof(fileField)} enum type could not be resolved");
                }

                await _dbContext.UpdateJobListingAssociatedFilesAsync(JobListing.JobListingAssociatedFiles);
            }

            UpdateViewModelInteractivityStates();

            async Task DisplayUploadFileErrorMessageAsync()
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "File Cannot Be Uploaded",
                    "An issue was encountered when trying to upload the file.",
                    MsBox.Avalonia.Enums.ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error);

                await box.ShowAsync();
            }
        }

        /// <summary>
        /// Computes the MD5 hash of the specified file, creates a duplicate file where the name is the value of the MD5 hash, saves the file within the GUI
        /// local directory, then returns the hashed file name and extension.
        /// </summary>
        /// <param name="originalFilePath"></param>
        /// <param name="fileExtension"></param>
        /// <returns>The file name and extension of the hashed file that was saved within the GUI local directory.</returns>
        private async Task<string> HashFileAndSaveToLocalDirectoryAsync(string originalFilePath, string fileExtension)
        {
            using var stream = File.OpenRead(originalFilePath);
            using var md5 = MD5.Create();
            var hash = await md5.ComputeHashAsync(stream);
            var hashString = Convert.ToHexString(hash);

            var hashedFileNameAndExtension = $"{hashString}{fileExtension}";

            var hashedFilePath = Path.Join(_associatedFilesDirectoryPath, hashedFileNameAndExtension);

            File.Copy(originalFilePath, hashedFilePath, true);

            return hashedFileNameAndExtension;
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
        private void ToggleEditMode()
        {
            IsEditModeEnabled = !IsEditModeEnabled;
            SetEditViewProperties();
        }

        private void SetEditViewProperties()
        {
            if (IsEditModeEnabled)
            {
                EditButtonFontWeight = EDIT_BUTTON_ENABLED_FONT_WEIGHT;
            }
            else
            {
                EditButtonFontWeight = EDIT_BUTTON_DEFAULT_FONT_WEIGHT;
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
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Confirm Delete Job",
                "Are you sure you want to delete this job listing? This action cannot be reversed.",
                MsBox.Avalonia.Enums.ButtonEnum.OkAbort,
                MsBox.Avalonia.Enums.Icon.Warning);

            var result = await box.ShowAsync();

            if (result != MsBox.Avalonia.Enums.ButtonResult.Ok) return;

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

            JobListing = jobListing;
            EnableOnChangedEvents(JobListing);

            UpdateViewModelInteractivityStates();
        }

        private void UpdateViewModelInteractivityStates()
        {
            SelectedContactId = -1; // Prevent erroneous navigations to contact when updating the view model.
            IsEditModeEnabled = false;
            SetEditViewProperties();

            IsNavigateToContactButtonEnabled = AssociatedContactIds.Any();

            IsViewFilesEnabled = JobListing?.JobListingAssociatedFiles is not null;

            if (!IsViewFilesEnabled)
            {
                IsViewResumeEnabled = false;
                IsViewCoverLetterEnabled = false;
                IsViewFile1Enabled = false;
                IsViewFile2Enabled = false;
                IsViewFile3Enabled = false;
            }
            else
            {
                IsViewResumeEnabled = !string.IsNullOrWhiteSpace(JobListing?.JobListingAssociatedFiles?.Resume);
                IsViewCoverLetterEnabled = !string.IsNullOrWhiteSpace(JobListing?.JobListingAssociatedFiles?.CoverLetter);
                IsViewFile1Enabled = !string.IsNullOrWhiteSpace(JobListing?.JobListingAssociatedFiles?.File1);
                IsViewFile2Enabled = !string.IsNullOrWhiteSpace(JobListing?.JobListingAssociatedFiles?.File2);
                IsViewFile3Enabled = !string.IsNullOrWhiteSpace(JobListing?.JobListingAssociatedFiles?.File3);
            }
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