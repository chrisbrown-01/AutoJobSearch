using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Helpers
{
    internal static class JobSearchProfileHelpers
    {
        internal static List<JobSearchProfileModel> ConvertProfilesToMvvmModel(IEnumerable<JobSearchProfile> profiles)
        {
            var profilesMvvm = new List<JobSearchProfileModel>();

            foreach (var profile in profiles)
            {
                profilesMvvm.Add(ConvertProfileToMvvmModel(profile));
            }

            return profilesMvvm;
        }

        internal static JobSearchProfileModel ConvertProfileToMvvmModel(JobSearchProfile profile)
        {
            return new JobSearchProfileModel()
            {
                Id = profile.Id,
                MaxJobListingIndex = profile.MaxJobListingIndex,
                ProfileName = profile.ProfileName,
                Searches = profile.Searches,
                KeywordsPositive = profile.KeywordsPositive,
                KeywordsNegative = profile.KeywordsNegative,
                SentimentsPositive = profile.SentimentsPositive,
                SentimentsNegative = profile.SentimentsNegative
            };
        }
    }
}
