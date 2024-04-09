using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Tests.Helpers
{
    public class JobSearchProfileHelpers_Tests
    {
        private readonly IFixture _fixture;

        public JobSearchProfileHelpers_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public void ConvertProfilesToMvvmModel_CorrectlyCompletesConversion()
        {
            // Arrange
            var jobSearchProfiles = _fixture.CreateMany<JobSearchProfile>().ToList();

            // Act
            var jobSearchProfileModels = JobSearchProfileHelpers.ConvertProfilesToMvvmModel(jobSearchProfiles);

            // Assert
            jobSearchProfileModels.Should().NotBeNullOrEmpty();
            jobSearchProfileModels.Should().AllBeOfType<JobSearchProfileModel>();
            jobSearchProfileModels.Should().HaveCount(jobSearchProfiles.Count);

            for (int i = 0; i < jobSearchProfiles.Count; i++)
            {
                jobSearchProfileModels[i].Id.Should().Be(jobSearchProfiles[i].Id);
                jobSearchProfileModels[i].KeywordsNegative.Should().Be(jobSearchProfiles[i].KeywordsNegative);
                jobSearchProfileModels[i].KeywordsPositive.Should().Be(jobSearchProfiles[i].KeywordsPositive);
                jobSearchProfileModels[i].MaxJobListingIndex.Should().Be(jobSearchProfiles[i].MaxJobListingIndex);
                jobSearchProfileModels[i].ProfileName.Should().Be(jobSearchProfiles[i].ProfileName);
                jobSearchProfileModels[i].Searches.Should().Be(jobSearchProfiles[i].Searches);
                jobSearchProfileModels[i].SentimentsNegative.Should().Be(jobSearchProfiles[i].SentimentsNegative);
                jobSearchProfileModels[i].SentimentsPositive.Should().Be(jobSearchProfiles[i].SentimentsPositive);
                jobSearchProfileModels[i].EnableEvents.Should().BeFalse();
            }
        }

        [Fact]
        public void ConvertProfileToMvvmModel_CorrectlyCompletesConversion()
        {
            // Arrange
            var jobSearchProfile = _fixture.Create<JobSearchProfile>();

            // Act
            var jobSearchProfileModel = JobSearchProfileHelpers.ConvertProfileToMvvmModel(jobSearchProfile);

            // Assert
            jobSearchProfileModel.Should().NotBeNull();
            jobSearchProfileModel.Should().BeOfType<JobSearchProfileModel>();

            jobSearchProfileModel.Id.Should().Be(jobSearchProfile.Id);
            jobSearchProfileModel.KeywordsNegative.Should().Be(jobSearchProfile.KeywordsNegative);
            jobSearchProfileModel.KeywordsPositive.Should().Be(jobSearchProfile.KeywordsPositive);
            jobSearchProfileModel.MaxJobListingIndex.Should().Be(jobSearchProfile.MaxJobListingIndex);
            jobSearchProfileModel.ProfileName.Should().Be(jobSearchProfile.ProfileName);
            jobSearchProfileModel.Searches.Should().Be(jobSearchProfile.Searches);
            jobSearchProfileModel.SentimentsNegative.Should().Be(jobSearchProfile.SentimentsNegative);
            jobSearchProfileModel.SentimentsPositive.Should().Be(jobSearchProfile.SentimentsPositive);
            jobSearchProfileModel.EnableEvents.Should().BeFalse();
        }
    }
}