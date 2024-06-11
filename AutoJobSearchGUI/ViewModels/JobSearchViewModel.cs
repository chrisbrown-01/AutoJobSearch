using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.Services;
using AutoJobSearchShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class JobSearchViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        private readonly IDbContext _dbContext;

        [ObservableProperty]
        private List<JobSearchProfileModel> _searchProfiles = new();

        [ObservableProperty]
        private JobSearchProfileModel _selectedSearchProfile = new();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public JobSearchViewModel() // For View previewer only
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            SearchProfiles = new();
            SelectedSearchProfile = new();
        }

        public JobSearchViewModel(IDbContext dbContext)
        {
            _dbContext = dbContext;
            RenderDefaultJobSearchViewCommand.Execute(null);
        }

        [RelayCommand]
        private async Task CreateNewProfileAsync()
        {
            await _dbContext.CreateJobSearchProfileAsync(new JobSearchProfile());

            var allProfiles = await _dbContext.GetAllJobSearchProfilesAsync();

            if (!allProfiles.Any())
                throw new ApplicationException("No job search profiles could be loaded for the Job Search page.");

            SearchProfiles = JobSearchProfileHelpers.ConvertProfilesToMvvmModel(allProfiles);

            SelectedSearchProfile = SearchProfiles.Last();
            EnableOnChangedEvents(SearchProfiles);
        }

        [RelayCommand]
        private async Task DeleteCurrentProfileAsync()
        {
            if (SelectedSearchProfile == null || SelectedSearchProfile.Id < 1) return;

            await _dbContext.DeleteJobSearchProfileAsync(SelectedSearchProfile.Id);
            await RenderDefaultJobSearchViewCommand.ExecuteAsync(null);
        }

        /// <summary>
        /// Allows events to fire. This method should be called after the view model properties have been fully instantiated.
        /// </summary>
        /// <param name="profiles"></param>
        private void EnableOnChangedEvents(IEnumerable<JobSearchProfileModel> profiles)
        {
            foreach (var profile in profiles)
            {
                profile.EnableEvents = true;
            }
        }

        [RelayCommand]
        private async Task ExecuteJobSearch()
        {
            Log.Information("Executing job search for job search profile {@id}", SelectedSearchProfile!.Id);
            JobScraperService.StartJobScraper(SelectedSearchProfile.Id);

            var box = MessageBoxManager.GetMessageBoxStandard(
                "",
                "Automated job search has been executed.\r\n" +
                "\r\n" +
                "After the automated web browsers and the console window have closed,\r\n" +
                "navigate to the Job Board and press 'Options' --> 'Go To Default View' to view the new listings.\r\n" +
                "\r\n" +
                "Do not close the web browsers or console window manually, as they will close on their own once completed.",
                MsBox.Avalonia.Enums.ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Info);

            await box.ShowAsync();
        }

        [RelayCommand]
        private async Task RenderDefaultJobSearchViewAsync()
        {
            var allProfiles = await _dbContext.GetAllJobSearchProfilesAsync();

            if (!allProfiles.Any())
            {
                await CreateNewProfileAsync();
                return;
            }

            SearchProfiles = JobSearchProfileHelpers.ConvertProfilesToMvvmModel(allProfiles);

            if (!SearchProfiles.Any())
            {
                Log.Error($"No job search profiles were loaded into {nameof(SearchProfiles)} view model property.");
                return;
            }

            SelectedSearchProfile = SearchProfiles.First();
            EnableOnChangedEvents(SearchProfiles);
        }
    }
}