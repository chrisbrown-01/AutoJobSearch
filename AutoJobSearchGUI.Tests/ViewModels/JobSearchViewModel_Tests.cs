using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.ViewModels;
using AutoJobSearchShared.Models;
using FluentAssertions;
using NSubstitute;

namespace AutoJobSearchGUI.Tests.ViewModels
{
    public class JobSearchViewModel_Tests
    {
        private readonly IDbContext _dbContext;
        private readonly IFixture _fixture;
        private readonly JobSearchViewModel _viewModel;

        public JobSearchViewModel_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _dbContext = Substitute.For<IDbContext>();
            _viewModel = new JobSearchViewModel(_dbContext);
        }

        [Fact]
        public async Task CreateNewProfile_HasProfiles_CorrectlyUpdatesProperties()
        {
            // Arrange
            var profiles = _fixture.CreateMany<JobSearchProfile>();
            _dbContext.GetAllJobSearchProfilesAsync().Returns(profiles);

            // Act
            await _viewModel.CreateNewProfileCommand.ExecuteAsync(null);

            // Assert
            await _dbContext.Received().CreateJobSearchProfileAsync(Arg.Any<JobSearchProfile>());
            _viewModel.SearchProfiles.Should().NotBeNullOrEmpty();
            _viewModel.SelectedSearchProfile.Should().NotBeNull();
            _viewModel.SelectedSearchProfile.Should().Be(_viewModel.SearchProfiles.Last());
        }

        [Fact]
        public async Task DeleteCurrentProfile_SelectedSearchProfileIdIsLessThanOne_DoesNotDeleteProfile()
        {
            // Arrange
            _viewModel.SelectedSearchProfile = new JobSearchProfileModel() { Id = 0 };

            // Act
            await _viewModel.DeleteCurrentProfileCommand.ExecuteAsync(null);

            // Assert
            await _dbContext.DidNotReceive().DeleteJobSearchProfileAsync(Arg.Any<int>());
        }

        [Fact]
        public async Task DeleteCurrentProfile_SelectedSearchProfileIsNull_DoesNotDeleteProfile()
        {
            // Arrange
            _viewModel.SelectedSearchProfile = null!;

            // Act
            await _viewModel.DeleteCurrentProfileCommand.ExecuteAsync(null);

            // Assert
            await _dbContext.DidNotReceive().DeleteJobSearchProfileAsync(Arg.Any<int>());
        }

        [Fact]
        public async Task DeleteCurrentProfile_SelectedSearchProfileIsValid_DeletesProfileAndRendersDefaultView()
        {
            // Arrange
            var selectedSearchProfile = new JobSearchProfileModel() { Id = 100 }; // Any positive integer for ID
            _viewModel.SelectedSearchProfile = selectedSearchProfile;
            _dbContext.GetAllJobSearchProfilesAsync().Returns(_fixture.CreateMany<JobSearchProfile>());

            // Act
            await _viewModel.DeleteCurrentProfileCommand.ExecuteAsync(null);

            // Assert
            await _dbContext.Received().DeleteJobSearchProfileAsync(selectedSearchProfile.Id);
            await _dbContext.Received().GetAllJobSearchProfilesAsync();
            _viewModel.SelectedSearchProfile.Should().NotBeNull();
            _viewModel.SearchProfiles.Should().NotBeNullOrEmpty();
            _viewModel.SearchProfiles.Should().AllSatisfy(x => x.EnableEvents.Should().BeTrue());
        }

        [Fact]
        public async Task RenderDefaultJobSearchViewAsync_CorrectlySwitchesToFirstProfile()
        {
            // Arrange
            var jobSearchProfiles = _fixture.CreateMany<JobSearchProfile>();
            _dbContext.GetAllJobSearchProfilesAsync().Returns(jobSearchProfiles);

            var jobSearchProfileModels = JobSearchProfileHelpers.ConvertProfilesToMvvmModel(jobSearchProfiles);
            jobSearchProfileModels.First().EnableEvents = true;

            // Act
            await _viewModel.RenderDefaultJobSearchViewCommand.ExecuteAsync(null);

            // Assert
            _viewModel.SelectedSearchProfile.Should().BeEquivalentTo(jobSearchProfileModels.First());
            _viewModel.SelectedSearchProfile.Should().NotBe(null);
            _viewModel.SearchProfiles.Should().NotBeNullOrEmpty();
        }
    }
}