using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.ViewModels;
using AutoJobSearchShared.Models;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Tests.ViewModels
{
    public class JobListingViewModel_Tests
    {
        private readonly JobListingViewModel _viewModel;
        private readonly IDbContext _dbContext;
        private readonly IFixture _fixture;

        public JobListingViewModel_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _dbContext = Substitute.For<IDbContext>();
            _viewModel = new JobListingViewModel(_dbContext);
        }

        [Fact]
        public async void OpenJobListing_DetailsNotPopulated_UpdatesPropertiesCorrectly()
        {
            // Arrange
            var jobListingWithoutDetails = _fixture.Create<JobListingModel>();
            jobListingWithoutDetails.DetailsPopulated = false;

            var jobListingWithDetails = _fixture.Create<JobListing>();

            Singletons.Contacts = new List<ContactModel>();

            _dbContext.GetJobListingDetailsByIdAsync(jobListingWithoutDetails.Id).Returns(jobListingWithDetails);

            // Act
            await _viewModel.OpenJobListingCommand.ExecuteAsync(jobListingWithoutDetails);

            // Assert
            await _dbContext.Received().GetJobListingDetailsByIdAsync(jobListingWithoutDetails.Id);
            _viewModel.JobListing.Id.Should().Be(jobListingWithoutDetails.Id);
            _viewModel.JobListing.SearchTerm.Should().Be(jobListingWithoutDetails.SearchTerm);
            _viewModel.JobListing.CreatedAt.Should().Be(jobListingWithoutDetails.CreatedAt);
            _viewModel.JobListing.Description.Should().Be(jobListingWithDetails.Description);
            _viewModel.JobListing.ApplicationLinks.Should().Be(jobListingWithDetails.ApplicationLinksString);
            _viewModel.JobListing.Score.Should().Be(jobListingWithoutDetails.Score);
            _viewModel.JobListing.DetailsPopulated.Should().BeTrue();
            _viewModel.JobListing.EnableEvents.Should().BeTrue();
            _viewModel.JobListing.IsAppliedTo.Should().Be(jobListingWithoutDetails.IsAppliedTo);
            _viewModel.JobListing.IsInterviewing.Should().Be(jobListingWithoutDetails.IsInterviewing);
            _viewModel.JobListing.IsRejected.Should().Be(jobListingWithoutDetails.IsRejected);
            _viewModel.JobListing.IsFavourite.Should().Be(jobListingWithoutDetails.IsFavourite);
            _viewModel.JobListing.IsHidden.Should().Be(jobListingWithoutDetails.IsHidden);
            _viewModel.JobListing.Notes.Should().Be(jobListingWithDetails.Notes);
        }

        [Fact]
        public async void OpenJobListing_DetailsPopulated_UpdatesPropertiesCorrectly()
        {
            // Arrange
            var jobListingWithDetails = _fixture.Create<JobListingModel>();
            Singletons.Contacts = new List<ContactModel>();
            jobListingWithDetails.DetailsPopulated = true;
            jobListingWithDetails.EnableEvents = false;

            // Act
            await _viewModel.OpenJobListingCommand.ExecuteAsync(jobListingWithDetails);

            // Assert
            await _dbContext.DidNotReceive().GetJobListingDetailsByIdAsync(Arg.Any<int>());
            _viewModel.JobListing.Should().BeEquivalentTo(jobListingWithDetails);
            _viewModel.JobListing.DetailsPopulated.Should().BeTrue();
            _viewModel.JobListing.EnableEvents.Should().BeTrue();
        }
    }
}
