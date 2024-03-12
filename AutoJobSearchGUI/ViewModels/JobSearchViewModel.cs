using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.Services;
using AutoJobSearchShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// TODO: run format and code cleanup

namespace AutoJobSearchGUI.ViewModels
{
    public partial class JobSearchViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        [ObservableProperty]
        private List<JobSearchProfileModel> _searchProfiles = new();

        [ObservableProperty]
        private JobSearchProfileModel _selectedSearchProfile = new();

        private readonly IDbContext _dbContext;

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
        private void ExecuteJobSearch()
        {
            Log.Information("Executing job search for job search profile {@id}", SelectedSearchProfile!.Id);
            JobScraperService.StartJobScraper(SelectedSearchProfile.Id);
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
    }
}
