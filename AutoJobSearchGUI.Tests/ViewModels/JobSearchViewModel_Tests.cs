using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.ViewModels;
using AutoJobSearchShared.Models;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Tests.ViewModels
{
    public class JobSearchViewModel_Tests
    {
        private readonly JobSearchViewModel _viewModel;
        private readonly IDbContext _dbContext;
        private readonly IFixture _fixture;

        public JobSearchViewModel_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _dbContext = Substitute.For<IDbContext>();
            _viewModel = new JobSearchViewModel(_dbContext);
        }

        [Fact]
        public async void CreateNewProfile_HasProfiles_CorrectlyUpdatesProperties()
        {
            // Arrange
            var profiles = _fixture.CreateMany<JobSearchProfile>();
            _dbContext.GetAllJobSearchProfilesAsync().Returns(profiles);

            // Act
            _viewModel.CreateNewProfile();

            // Assert
            await _dbContext.Received().CreateJobSearchProfileAsync(Arg.Any<JobSearchProfile>());
            _viewModel.SearchProfiles.Should().NotBeNullOrEmpty();
            _viewModel.SelectedSearchProfile.Should().NotBeNull();
            _viewModel.SelectedSearchProfile.Should().Be(_viewModel.SearchProfiles.Last());
        }

        [Fact]
        public async void DeleteCurrentProfile_SelectedSearchProfileIsNull_DoesNotDeleteProfile()
        {
            // Arrange
            _viewModel.SelectedSearchProfile = null;

            // Act
            _viewModel.DeleteCurrentProfile();

            // Assert
            await _dbContext.DidNotReceive().DeleteJobSearchProfileAsync(Arg.Any<int>());
        }

        [Fact]
        public async void DeleteCurrentProfile_SelectedSearchProfileIdIsLessThanOne_DoesNotDeleteProfile()
        {
            // Arrange
            _viewModel.SelectedSearchProfile = new JobSearchProfileModel() { Id = 0 };

            // Act
            _viewModel.DeleteCurrentProfile();

            // Assert
            await _dbContext.DidNotReceive().DeleteJobSearchProfileAsync(Arg.Any<int>());
        }

        [Fact]
        public async void DeleteCurrentProfile_SelectedSearchProfileIsValid_DeletesProfileAndRendersDefaultView()
        {
            // Arrange
            var selectedSearchProfile = new JobSearchProfileModel() { Id = 100 }; // Any positive integer for ID
            _viewModel.SelectedSearchProfile = selectedSearchProfile;
            _dbContext.GetAllJobSearchProfilesAsync().Returns(_fixture.CreateMany<JobSearchProfile>());

            // Act
            _viewModel.DeleteCurrentProfile();

            // Assert
            await _dbContext.Received().DeleteJobSearchProfileAsync(selectedSearchProfile.Id);
            await _dbContext.Received().GetAllJobSearchProfilesAsync();
            _viewModel.SelectedSearchProfile.Should().NotBeNull();
            _viewModel.SearchProfiles.Should().NotBeNullOrEmpty();
            _viewModel.SearchProfiles.Should().AllSatisfy(x => x.EnableEvents.Should().BeTrue());
        }
    }
}
