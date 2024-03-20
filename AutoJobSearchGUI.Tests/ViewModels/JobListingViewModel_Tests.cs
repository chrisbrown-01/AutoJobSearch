using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.Services;
using AutoJobSearchGUI.ViewModels;
using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AutoJobSearchGUI.Tests.ViewModels
{
    public class JobListingViewModel_Tests
    {
        private readonly IDbContext _dbContext;
        private readonly IFilesService _filesService;
        private readonly IFixture _fixture;
        private readonly JobListingViewModel _viewModel;

        public JobListingViewModel_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _dbContext = Substitute.For<IDbContext>();
            _filesService = Substitute.For<IFilesService>();
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

        [Fact]
        public void ViewContact_ValidContactId_InvokesChangeViewEvent()
        {
            // Arrange
            _viewModel.SelectedContactId = Math.Abs(_fixture.Create<int>());
            bool wasCalled = false;
            _viewModel.ChangeViewToContactRequest += (id) => wasCalled = true;

            // Act
            _viewModel.ViewContactCommand.Execute(null);

            // Assert
            _viewModel.JobListing.EnableEvents.Should().Be(false);
            wasCalled.Should().Be(true);
        }

        [Fact]
        public void ViewContact_InvalidContactId_DoesNotInvokeChangeViewEvent()
        {
            // Arrange
            _viewModel.SelectedContactId = 0;
            bool wasCalled = false;
            _viewModel.ChangeViewToContactRequest += (id) => wasCalled = true;

            // Act
            _viewModel.ViewContactCommand.Execute(null);

            // Assert
            _viewModel.JobListing.EnableEvents.Should().Be(false);
            wasCalled.Should().Be(false);
        }

        [Fact]
        public void AddAssociatedContact_InvokesEvent()
        {
            // Arrange
            _viewModel.JobListing = _fixture.Create<JobListingModel>();
            bool wasCalled = false;
            _viewModel.CreateNewContactWithAssociatedJobIdRequest += (id) => wasCalled = true;

            // Act
            _viewModel.AddAssociatedContactCommand.Execute(null);

            // Assert
            _viewModel.JobListing.EnableEvents.Should().Be(false);
            wasCalled.Should().Be(true);
        }

        [Fact]
        public void ToggleEditMode_Inverts_IsEditModeEnabled_Property()
        {
            // Arrange
            _viewModel.IsEditModeEnabled = _fixture.Create<bool>();
            var initialIsEditModeEnabled = _viewModel.IsEditModeEnabled;

            if (initialIsEditModeEnabled)
            {
                _viewModel.EditButtonFontWeight = "ExtraBold";
            }
            else
            {
                _viewModel.EditButtonFontWeight = "Regular";
            }

            var initialEditButtonFontWeight = _viewModel.EditButtonFontWeight;

            // Act
            _viewModel.ToggleEditModeCommand.Execute(null);

            // Assert
            _viewModel.IsEditModeEnabled.Should().Be(!initialIsEditModeEnabled);
            _viewModel.EditButtonFontWeight.Should().NotBe(initialEditButtonFontWeight);
        }


        [Fact]
        public async void CreateJobAsync_CreatesNewJob()
        {
            // Arrange
            _viewModel.JobListing = _fixture.Create<JobListingModel>();

            var newJob = _fixture.Create<JobListing>();
            var newJobListingModel = JobListingHelpers.ConvertJobListingToJobListingModel(newJob);

            Singletons.JobListings = _fixture.CreateMany<JobListingModel>().ToList();
            var initialJobListingsCount = Singletons.JobListings.Count;

            Singletons.Contacts = _fixture.CreateMany<ContactModel>().ToList();

            bool wasCalled = false;
            _viewModel.UpdateJobBoardViewRequest += () => wasCalled = true;

            _dbContext.CreateJobAsync().Returns(newJob);
            _dbContext.GetJobListingDetailsByIdAsync(newJob.Id).Returns(newJob);

            // Act
            await _viewModel.CreateJobCommand.ExecuteAsync(null);

            // Assert
            wasCalled.Should().Be(true);
            await _dbContext.Received().CreateJobAsync();
            Singletons.JobListings.Count.Should().Be(initialJobListingsCount + 1);
        }

        [Fact]
        public async void GoToPreviousJobAsync_GoesToPreviousJob_WhenValid()
        {
            // Arrange
            Singletons.JobListings = _fixture.CreateMany<JobListingModel>().ToList();
            Singletons.Contacts = _fixture.CreateMany<ContactModel>().ToList();
            _viewModel.JobListing = Singletons.JobListings.Last();
            var initialJobListing = _viewModel.JobListing;

            var newJob = _fixture.Create<JobListing>();
            _dbContext.GetJobListingDetailsByIdAsync(Arg.Any<int>()).Returns(newJob);

            // Act
            await _viewModel.GoToPreviousJobCommand.ExecuteAsync(null);

            // Assert
            _viewModel.JobListing.Should().NotBe(initialJobListing);
        }

        [Fact]
        public async void GoToPreviousJobAsync_DoesNotGoToPreviousJob_WhenInvalid()
        {
            // Arrange
            Singletons.JobListings = _fixture.CreateMany<JobListingModel>().ToList();
            _viewModel.JobListing = Singletons.JobListings.First();
            var initialJobListing = _viewModel.JobListing;

            // Act
            await _viewModel.GoToPreviousJobCommand.ExecuteAsync(null);

            // Assert
            _viewModel.JobListing.Should().Be(initialJobListing);
        }

        [Fact]
        public async void GoToNextJobAsync_GoesToNextJob_WhenValid()
        {
            // Arrange
            Singletons.JobListings = _fixture.CreateMany<JobListingModel>().ToList();
            Singletons.Contacts = _fixture.CreateMany<ContactModel>().ToList();
            _viewModel.JobListing = Singletons.JobListings.First();
            var initialJobListing = _viewModel.JobListing;

            var newJob = _fixture.Create<JobListing>();
            _dbContext.GetJobListingDetailsByIdAsync(Arg.Any<int>()).Returns(newJob);

            // Act
            await _viewModel.GoToNextJobCommand.ExecuteAsync(null);

            // Assert
            _viewModel.JobListing.Should().NotBe(initialJobListing);
        }

        [Fact]
        public async void GoToNextJobAsync_DoesNotGoToNextJob_WhenInvalid()
        {
            // Arrange
            Singletons.JobListings = _fixture.CreateMany<JobListingModel>().ToList();
            _viewModel.JobListing = Singletons.JobListings.Last();
            var initialJobListing = _viewModel.JobListing;

            // Act
            await _viewModel.GoToNextJobCommand.ExecuteAsync(null);

            // Assert
            _viewModel.JobListing.Should().Be(initialJobListing);
        }

        [Fact]
        public async void OpenJobListingByIdAsync_OpensJobListingWithCorrectId()
        {
            // Arrange
            Singletons.JobListings = _fixture.CreateMany<JobListingModel>().ToList();
            Singletons.Contacts = _fixture.CreateMany<ContactModel>().ToList();

            var jobId = _fixture.Create<int>();
            var jobIdModel = new JobListingModel() { Id = jobId, DetailsPopulated = true };
            Singletons.JobListings.Add(jobIdModel);

            // Act
            await _viewModel.OpenJobListingByIdCommand.ExecuteAsync(jobId);

            // Assert
            _viewModel.JobListing.Id.Should().Be(jobId);
        }

        [Fact]
        public async void OpenJobListingAsync_OpensCorrectJobListing_WhenDetails_ArePopulated()
        {
            // Arrange
            Singletons.JobListings = _fixture.CreateMany<JobListingModel>().ToList();
            Singletons.Contacts = _fixture.CreateMany<ContactModel>().ToList();

            var jobId = _fixture.Create<int>();
            var jobListingModel = new JobListingModel() { Id = jobId, DetailsPopulated = true };
            Singletons.JobListings.Add(jobListingModel);

            // Act
            await _viewModel.OpenJobListingCommand.ExecuteAsync(jobListingModel);

            // Assert
            _viewModel.JobListing.Id.Should().Be(jobId);
        }

        [Fact]
        public async void OpenJobListingAsync_OpensCorrectJobListing_WhenDetails_AreNotPopulated()
        {
            // Arrange
            Singletons.JobListings = _fixture.CreateMany<JobListingModel>().ToList();
            Singletons.Contacts = _fixture.CreateMany<ContactModel>().ToList();

            var jobId = _fixture.Create<int>();
            var jobListingModel = new JobListingModel() { Id = jobId, DetailsPopulated = true };
            Singletons.JobListings.Add(jobListingModel);

            var jobListing = _fixture.Create<JobListing>();
            _dbContext.GetJobListingDetailsByIdAsync(Arg.Any<int>()).Returns(jobListing);

            // Act
            await _viewModel.OpenJobListingCommand.ExecuteAsync(jobListingModel);

            // Assert
            _viewModel.JobListing.Id.Should().Be(jobId);
        }

        /*
// Arrange

// Act

// Assert
        */
    }
}