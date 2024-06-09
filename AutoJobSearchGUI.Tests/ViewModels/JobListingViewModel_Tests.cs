using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.Services;
using AutoJobSearchGUI.ViewModels;
using AutoJobSearchShared.Models;
using FluentAssertions;
using NSubstitute;

namespace AutoJobSearchGUI.Tests.ViewModels
{
    public class JobListingViewModel_Tests
    {
        private readonly IDbContext _dbContext;
        private readonly IFilesService _filesService;
        private readonly IFixture _fixture;
        private readonly List<ContactModel> _singletonContacts;
        private readonly List<JobListingModel> _singletonJobListings;
        private readonly JobListingViewModel _viewModel;

        public JobListingViewModel_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _dbContext = Substitute.For<IDbContext>();
            _filesService = Substitute.For<IFilesService>();
            _viewModel = new JobListingViewModel(_dbContext);
            _singletonJobListings = _fixture.CreateMany<JobListingModel>().ToList();
            _singletonContacts = _fixture.CreateMany<ContactModel>().ToList();
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
        public async Task CreateJobAsync_CreatesNewJob()
        {
            // Arrange
            Singletons.Contacts = _singletonContacts;

            Singletons.JobListings = _singletonJobListings;
            var initialJobListingsCount = Singletons.JobListings.Count;

            _viewModel.JobListing = _fixture.Create<JobListingModel>();

            var newJob = _fixture.Create<JobListing>();

            bool wasCalled = false;
            _viewModel.UpdateJobBoardViewRequest += () => wasCalled = true;

            _dbContext.CreateJobListingAsync().Returns(newJob);
            _dbContext.GetJobListingByIdAsync(newJob.Id, false).Returns(newJob);

            // Act
            await _viewModel.CreateJobCommand.ExecuteAsync(null);

            // Assert
            wasCalled.Should().Be(true);
            await _dbContext.Received().CreateJobListingAsync();

            var count = Singletons.JobListings.Count;

            count.Should().Be(initialJobListingsCount + 1);
        }

        [Fact]
        public async Task GoToNextJobAsync_DoesNotGoToNextJob_WhenInvalid()
        {
            // Arrange
            Singletons.JobListings = _singletonJobListings;
            _viewModel.JobListing = Singletons.JobListings.Last();
            var initialJobListing = _viewModel.JobListing;

            // Act
            await _viewModel.GoToNextJobCommand.ExecuteAsync(null);

            // Assert
            _viewModel.JobListing.Should().Be(initialJobListing);
        }

        [Fact]
        public async Task GoToNextJobAsync_GoesToNextJob_WhenValid()
        {
            // Arrange
            Singletons.Contacts = _singletonContacts;
            Singletons.JobListings = _singletonJobListings;
            _viewModel.JobListing = Singletons.JobListings.First();
            var initialJobListing = _viewModel.JobListing;

            var newJob = _fixture.Create<JobListing>();
            _dbContext.GetJobListingByIdAsync(Arg.Any<int>(), false).Returns(newJob);

            // Act
            await _viewModel.GoToNextJobCommand.ExecuteAsync(null);

            // Assert
            _viewModel.JobListing.Should().NotBe(initialJobListing);
        }

        [Fact]
        public async Task GoToPreviousJobAsync_DoesNotGoToPreviousJob_WhenInvalid()
        {
            // Arrange
            Singletons.JobListings = _singletonJobListings;
            _viewModel.JobListing = Singletons.JobListings.First();
            var initialJobListing = _viewModel.JobListing;

            // Act
            await _viewModel.GoToPreviousJobCommand.ExecuteAsync(null);

            // Assert
            _viewModel.JobListing.Should().Be(initialJobListing);
        }

        [Fact]
        public async Task GoToPreviousJobAsync_GoesToPreviousJob_WhenValid()
        {
            // Arrange
            Singletons.Contacts = _singletonContacts;
            Singletons.JobListings = _singletonJobListings;
            _viewModel.JobListing = Singletons.JobListings.Last();
            var initialJobListing = _viewModel.JobListing;

            var newJob = _fixture.Create<JobListing>();
            _dbContext.GetJobListingByIdAsync(Arg.Any<int>(), false).Returns(newJob);

            // Act
            await _viewModel.GoToPreviousJobCommand.ExecuteAsync(null);

            // Assert
            _viewModel.JobListing.Should().NotBe(initialJobListing);
        }

        [Fact]
        public async Task OpenJobListing_DetailsNotPopulated_UpdatesPropertiesCorrectly()
        {
            // Arrange
            var jobListingWithoutDetails = _fixture.Create<JobListingModel>();
            jobListingWithoutDetails.DetailsPopulated = false;

            var jobListingWithDetails = _fixture.Create<JobListing>();

            Singletons.Contacts = _singletonContacts;

            _dbContext.GetJobListingByIdAsync(jobListingWithoutDetails.Id, false).Returns(jobListingWithDetails);

            // Act
            await _viewModel.OpenJobListingCommand.ExecuteAsync(jobListingWithoutDetails);

            // Assert
            await _dbContext.Received().GetJobListingByIdAsync(jobListingWithoutDetails.Id, false);
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
        public async Task OpenJobListing_DetailsPopulated_UpdatesPropertiesCorrectly()
        {
            // Arrange
            var jobListingWithDetails = _fixture.Create<JobListingModel>();
            Singletons.Contacts = _singletonContacts;
            jobListingWithDetails.DetailsPopulated = true;
            jobListingWithDetails.EnableEvents = false;

            // Act
            await _viewModel.OpenJobListingCommand.ExecuteAsync(jobListingWithDetails);

            // Assert
            await _dbContext.DidNotReceive().GetJobListingByIdAsync(Arg.Any<int>(), false);
            _viewModel.JobListing.Should().BeEquivalentTo(jobListingWithDetails);
            _viewModel.JobListing.DetailsPopulated.Should().BeTrue();
            _viewModel.JobListing.EnableEvents.Should().BeTrue();
        }

        [Fact]
        public async Task OpenJobListingAsync_OpensCorrectJobListing_WhenDetails_AreNotPopulated()
        {
            // Arrange
            Singletons.Contacts = _singletonContacts;
            Singletons.JobListings = _singletonJobListings;

            var jobId = _fixture.Create<int>();
            var jobListingModel = new JobListingModel() { Id = jobId, DetailsPopulated = true };
            Singletons.JobListings.Add(jobListingModel);

            var jobListing = _fixture.Create<JobListing>();
            _dbContext.GetJobListingByIdAsync(Arg.Any<int>(), false).Returns(jobListing);

            // Act
            await _viewModel.OpenJobListingCommand.ExecuteAsync(jobListingModel);

            // Assert
            _viewModel.JobListing.Id.Should().Be(jobId);
        }

        [Fact]
        public async Task OpenJobListingAsync_OpensCorrectJobListing_WhenDetails_ArePopulated()
        {
            // Arrange
            Singletons.Contacts = _singletonContacts;
            Singletons.JobListings = _singletonJobListings;

            var jobId = _fixture.Create<int>();
            var jobListingModel = new JobListingModel() { Id = jobId, DetailsPopulated = true };
            Singletons.JobListings.Add(jobListingModel);

            // Act
            await _viewModel.OpenJobListingCommand.ExecuteAsync(jobListingModel);

            // Assert
            _viewModel.JobListing.Id.Should().Be(jobId);
        }

        [Fact]
        public async Task OpenJobListingByIdAsync_OpensJobListingWithCorrectId()
        {
            // Arrange
            Singletons.Contacts = _singletonContacts;
            Singletons.JobListings = _singletonJobListings;

            var jobId = _fixture.Create<int>();
            var jobIdModel = new JobListingModel() { Id = jobId, DetailsPopulated = true };
            Singletons.JobListings.Add(jobIdModel);

            // Act
            await _viewModel.OpenJobListingByIdCommand.ExecuteAsync(jobId);

            // Assert
            _viewModel.JobListing.Id.Should().Be(jobId);
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
        public void ViewContact_InvalidContactId_DoesNotInvokeChangeViewEvent()
        {
            // Arrange
            _viewModel.SelectedContactId = 0;
            bool wasCalled = false;
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            bool previousOrForwardButton = false;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
            _viewModel.ChangeViewToContactRequest += (id, previousOrForwardButton) => wasCalled = true;

            // Act
            _viewModel.ViewContactCommand.Execute(null);

            // Assert
            _viewModel.JobListing.EnableEvents.Should().Be(false);
            wasCalled.Should().Be(false);
        }

        [Fact]
        public void ViewContact_ValidContactId_InvokesChangeViewEvent()
        {
            // Arrange
            _viewModel.SelectedContactId = Math.Abs(_fixture.Create<int>());
            bool wasCalled = false;
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            bool previousOrForwardButton = false;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
            _viewModel.ChangeViewToContactRequest += (id, previousOrForwardButton) => wasCalled = true;

            // Act
            _viewModel.ViewContactCommand.Execute(null);

            // Assert
            _viewModel.JobListing.EnableEvents.Should().Be(false);
            wasCalled.Should().Be(true);
        }
    }
}