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
        private List<JobSearchProfileModel> _searchProfiles;

        [ObservableProperty]
        private JobSearchProfileModel _selectedSearchProfile;

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
            SearchProfiles = ConvertProfilesToMvvmModel(_dbContext.GetAllJobSearchProfiles().Result);

            if (SearchProfiles.Any())
            {
                SelectedSearchProfile = SearchProfiles.First();
            }
            else SelectedSearchProfile = new();
        }

        public async Task CreateNewProfile()
        {
            var newProfile = new JobSearchProfile()
            {
                Searches = SelectedSearchProfile.Searches,
                KeywordsPositive = SelectedSearchProfile.KeywordsPositive,
                KeywordsNegative = SelectedSearchProfile.KeywordsNegative,
                SentimentsPositive = SelectedSearchProfile.SentimentsPositive,
                SentimentsNegative = SelectedSearchProfile.SentimentsNegative
            };

            await _dbContext.CreateNewJobSearchProfile(newProfile);

            var allProfiles = await _dbContext.GetAllJobSearchProfiles();
            SearchProfiles = ConvertProfilesToMvvmModel(allProfiles);

            await Task.CompletedTask;
        }

        private List<JobSearchProfileModel> ConvertProfilesToMvvmModel(IEnumerable<JobSearchProfile> profiles)
        {
            var profilesMvvm = new List<JobSearchProfileModel>();   

            // TODO: extract to method?
            foreach(var profile in profiles)
            {
                var profileMvvm = new JobSearchProfileModel()
                {
                    Id = profile.Id,
                    ProfileName = profile.ProfileName,
                    Searches = profile.Searches,
                    KeywordsPositive = profile.KeywordsPositive,
                    KeywordsNegative = profile.KeywordsNegative,
                    SentimentsPositive = profile.SentimentsPositive,
                    SentimentsNegative = profile.SentimentsNegative
                };

                profilesMvvm.Add(profileMvvm);
            }

            return profilesMvvm;
        }

        //partial void OnSelectedComboBoxItemChanged(string? value)
        //{
        //    if (value.IsNullOrEmpty()) return;
        //    Debug.WriteLine($"selected value is {value}");
        //}
    }
}
