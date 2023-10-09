using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Task.Run(RenderDefaultJobSearchView);
        }

        private async Task RenderDefaultJobSearchView()
        {
            var allProfiles = await _dbContext.GetAllJobSearchProfiles();

            if (!allProfiles.Any())
            {
                await CreateNewProfile();
                return;
            }

            SearchProfiles = ConvertProfilesToMvvmModel(allProfiles);
            if (!SearchProfiles.Any()) throw new Exception("No objects could be rendered for SearchProfiles"); // TODO: proper logging, custom exception
            SelectedSearchProfile = SearchProfiles.First();
        }

        public async Task CreateNewProfile()
        {
            // TODO: how to throw exceptions in avalonia? relaycommands?
            //throw new Exception();

            await _dbContext.CreateNewJobSearchProfile(new JobSearchProfile());

            var allProfiles = await _dbContext.GetAllJobSearchProfiles();

            if (!allProfiles.Any()) throw new Exception("No objects could be rendered for SearchProfiles"); // TODO: proper logging, custom exception

            SearchProfiles = ConvertProfilesToMvvmModel(allProfiles);

            SelectedSearchProfile = SearchProfiles.Last();

            await Task.CompletedTask;
        }

        public async Task DeleteCurrentProfile()
        {
            if (SelectedSearchProfile == null || SelectedSearchProfile.Id < 1) return;

            await _dbContext.DeleteJobSearchProfile(SelectedSearchProfile.Id);
            await RenderDefaultJobSearchView();

            await Task.CompletedTask;
        }

        private List<JobSearchProfileModel> ConvertProfilesToMvvmModel(IEnumerable<JobSearchProfile> profiles)
        {
            var profilesMvvm = new List<JobSearchProfileModel>();   

            foreach(var profile in profiles)
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
