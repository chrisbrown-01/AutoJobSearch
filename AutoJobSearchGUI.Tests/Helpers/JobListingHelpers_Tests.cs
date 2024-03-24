using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using FluentAssertions;

namespace AutoJobSearchGUI.Tests.Helpers
{
    public class JobListingHelpers_Tests
    {
        private readonly IFixture _fixture;

        public JobListingHelpers_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public void ConvertJobListingsToJobListingModels_CorrectlyCompletesConversion()
        {
            // Arrange
            var jobListings = _fixture.CreateMany<JobListing>().ToList();

            // Act
            var jobListingModels = JobListingHelpers.ConvertJobListingsToJobListingModels(jobListings);

            // Assert
            jobListingModels.Should().NotBeNullOrEmpty();
            jobListingModels.Should().AllBeOfType<JobListingModel>();
            jobListingModels.Should().HaveCount(jobListings.Count);

            for (int i = 0; i < jobListings.Count; i++)
            {
                jobListingModels[i].Id.Should().Be(jobListings[i].Id);
                jobListingModels[i].CreatedAt.Should().Be(jobListings[i].CreatedAt);
                jobListingModels[i].SearchTerm.Should().Be(jobListings[i].SearchTerm);
                jobListingModels[i].StatusModifiedAt.Should().Be(jobListings[i].StatusModifiedAt);
                jobListingModels[i].Description.Should().Be(jobListings[i].Description);
                jobListingModels[i].Score.Should().Be(jobListings[i].Score);
                jobListingModels[i].IsToBeAppliedTo.Should().Be(jobListings[i].IsToBeAppliedTo);
                jobListingModels[i].IsAppliedTo.Should().Be(jobListings[i].IsAppliedTo);
                jobListingModels[i].IsInterviewing.Should().Be(jobListings[i].IsInterviewing);
                jobListingModels[i].IsNegotiating.Should().Be(jobListings[i].IsNegotiating);
                jobListingModels[i].IsRejected.Should().Be(jobListings[i].IsRejected);
                jobListingModels[i].IsDeclinedOffer.Should().Be(jobListings[i].IsDeclinedOffer);
                jobListingModels[i].IsAcceptedOffer.Should().Be(jobListings[i].IsAcceptedOffer);
                jobListingModels[i].IsFavourite.Should().Be(jobListings[i].IsFavourite);
                jobListingModels[i].IsHidden.Should().Be(jobListings[i].IsHidden);
                jobListingModels[i].EnableEvents.Should().BeFalse();
                jobListingModels[i].DetailsPopulated.Should().BeFalse();
            }
        }

        [Fact]
        public void ConvertJobListingToJobListingModel_CorrectlyCompletesConversion()
        {
            // Arrange
            var jobListing = _fixture.Create<JobListing>();

            // Act
            var jobListingModel = JobListingHelpers.ConvertJobListingToJobListingModel(jobListing);

            // Assert
            jobListingModel.Should().NotBeNull();
            jobListingModel.Should().BeOfType<JobListingModel>();

            jobListingModel.Id.Should().Be(jobListing.Id);
            jobListingModel.CreatedAt.Should().Be(jobListing.CreatedAt);
            jobListingModel.SearchTerm.Should().Be(jobListing.SearchTerm);
            jobListingModel.StatusModifiedAt.Should().Be(jobListing.StatusModifiedAt);
            jobListingModel.Description.Should().Be(jobListing.Description);
            jobListingModel.Score.Should().Be(jobListing.Score);
            jobListingModel.IsToBeAppliedTo.Should().Be(jobListing.IsToBeAppliedTo);
            jobListingModel.IsAppliedTo.Should().Be(jobListing.IsAppliedTo);
            jobListingModel.IsInterviewing.Should().Be(jobListing.IsInterviewing);
            jobListingModel.IsNegotiating.Should().Be(jobListing.IsNegotiating);
            jobListingModel.IsRejected.Should().Be(jobListing.IsRejected);
            jobListingModel.IsDeclinedOffer.Should().Be(jobListing.IsDeclinedOffer);
            jobListingModel.IsAcceptedOffer.Should().Be(jobListing.IsAcceptedOffer);
            jobListingModel.IsFavourite.Should().Be(jobListing.IsFavourite);
            jobListingModel.IsHidden.Should().Be(jobListing.IsHidden);
            jobListingModel.EnableEvents.Should().BeFalse();
            jobListingModel.DetailsPopulated.Should().BeFalse();
        }
    }
}