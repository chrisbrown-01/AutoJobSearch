using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.Services;
using AutoJobSearchShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
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
            RenderDefaultJobSearchView();
        }

        public void ExecuteJobSearch()
        {
            Log.Information("Executing job search for job search profile {@id}", SelectedSearchProfile.Id);
            JobScraperService.StartJobScraper(SelectedSearchProfile.Id);
        }

        private async void RenderDefaultJobSearchView()
        {
            var allProfiles = await _dbContext.GetAllJobSearchProfilesAsync();

            if (!allProfiles.Any())
            {
                CreateNewProfile();
                return;
            }

            SearchProfiles = ConvertProfilesToMvvmModel(allProfiles);

            if (!SearchProfiles.Any())
            {
                Log.Error($"No job search profiles were loaded into {nameof(SearchProfiles)} view model property.");
                return;
            }

            SelectedSearchProfile = SearchProfiles.First();
            EnableOnChangedEvents(SearchProfiles);
        }

        public async void CreateNewProfile()
        {
            await _dbContext.CreateJobSearchProfileAsync(new JobSearchProfile());

            var allProfiles = await _dbContext.GetAllJobSearchProfilesAsync();

            if (!allProfiles.Any())
                throw new ApplicationException("No job search profiles could be loaded for the Job Search page.");

            SearchProfiles = ConvertProfilesToMvvmModel(allProfiles);

            SelectedSearchProfile = SearchProfiles.Last();
            EnableOnChangedEvents(SearchProfiles);
        }

        public async void DeleteCurrentProfile()
        {
            if (SelectedSearchProfile == null || SelectedSearchProfile.Id < 1) return;

            await _dbContext.DeleteJobSearchProfileAsync(SelectedSearchProfile.Id);
            RenderDefaultJobSearchView();
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

        private List<JobSearchProfileModel> ConvertProfilesToMvvmModel(IEnumerable<JobSearchProfile> profiles)
        {
            var profilesMvvm = new List<JobSearchProfileModel>();

            foreach (var profile in profiles)
            {
                profilesMvvm.Add(ConvertProfileToMvvmModel(profile));
            }

            return profilesMvvm;
        }

        private JobSearchProfileModel ConvertProfileToMvvmModel(JobSearchProfile profile)
        {
            var result = new JobSearchProfileModel()
            {
                Id = profile.Id,
                ProfileName = profile.ProfileName,
                Searches = profile.Searches,
                KeywordsPositive = profile.KeywordsPositive,
                KeywordsNegative = profile.KeywordsNegative,
                SentimentsPositive = profile.SentimentsPositive,
                SentimentsNegative = profile.SentimentsNegative
            };

            return result;
        }
    }
}
