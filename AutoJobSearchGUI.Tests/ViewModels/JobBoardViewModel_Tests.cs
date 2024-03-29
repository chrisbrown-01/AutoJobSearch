﻿using AutoFixture;
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
    public class JobBoardViewModel_Tests
    {
        private readonly IDbContext _dbContext;
        private readonly IFixture _fixture;
        private readonly JobBoardViewModel _viewModel;

        public JobBoardViewModel_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _dbContext = Substitute.For<IDbContext>();
            _viewModel = new JobBoardViewModel(_dbContext);
        }

        [Fact]
        public void CreateJob_UpdatesViewModelCorrectly()
        {
            // Arrange
            var createdJob = _fixture.Create<JobListing>();
            _dbContext.CreateJobListingAsync().Returns(createdJob);

            Singletons.JobListings = _fixture.CreateMany<JobListingModel>().ToList();
            var initialNumOfJobListings = Singletons.JobListings.Count;
            var initialNumOfDisplayedJobListings = _viewModel.JobListingsDisplayed.Count;

            var createdJobModel = JobListingHelpers.ConvertJobListingToJobListingModel(createdJob);

            // Act
            _viewModel.CreateJobCommand.ExecuteAsync(null);
            _viewModel.OpenJobListingCommand.Execute(null);

            // Assert
            _viewModel.SelectedJobListing.Should().BeEquivalentTo(createdJobModel);
            var newCount = Singletons.JobListings.Count;
            newCount.Should().Be(initialNumOfJobListings + 1);

            var jobListingsDisplayedCount = _viewModel.JobListingsDisplayed.Count;
            jobListingsDisplayedCount.Should().Be(initialNumOfDisplayedJobListings + 1);
        }

        [Fact]
        public async Task ExecuteQuery_UpdatesPropertiesCorrectly()
        {
            // Arrange
            var pageSize = _viewModel.PageSize;
            var jobListings = _fixture.CreateMany<JobListing>(10000);
            foreach (var job in jobListings)
            {
                job.IsHidden = false;
            }

            var jobBoardQueryModel = _fixture.Create<JobBoardQueryModel>();
            jobBoardQueryModel.ColumnFiltersEnabled = true;
            jobBoardQueryModel.SearchTermQueryStringEnabled = false;
            jobBoardQueryModel.JobDescriptionQueryStringEnabled = false;
            jobBoardQueryModel.NotesQueryStringEnabled = false;
            jobBoardQueryModel.CreatedBetweenDatesEnabled = false;
            jobBoardQueryModel.CreatedAtDateEnabled = false;
            jobBoardQueryModel.ModifiedBetweenDatesEnabled = false;
            jobBoardQueryModel.ModifiedAtDateEnabled = false;
            jobBoardQueryModel.ScoreEqualsEnabled = false;
            jobBoardQueryModel.ScoreRangeEnabled = false;

            _viewModel.JobBoardQueryModel = jobBoardQueryModel;

            _dbContext.ExecuteJobListingQueryAsync(
                _viewModel.JobBoardQueryModel.JobDescriptionQueryStringEnabled,
                _viewModel.JobBoardQueryModel.NotesQueryStringEnabled,
                _viewModel.JobBoardQueryModel.ColumnFiltersEnabled,
                _viewModel.JobBoardQueryModel.IsToBeAppliedTo,
                _viewModel.JobBoardQueryModel.IsAppliedTo,
                _viewModel.JobBoardQueryModel.IsInterviewing,
                _viewModel.JobBoardQueryModel.IsNegotiating,
                _viewModel.JobBoardQueryModel.IsRejected,
                _viewModel.JobBoardQueryModel.IsDeclinedOffer,
                _viewModel.JobBoardQueryModel.IsAcceptedOffer,
                _viewModel.JobBoardQueryModel.IsFavourite).
                Returns(jobListings.AsQueryable());

            // Act
            await _viewModel.ExecuteQueryCommand.ExecuteAsync(null);

            // Assert
            await _dbContext.Received().ExecuteJobListingQueryAsync(
                _viewModel.JobBoardQueryModel.JobDescriptionQueryStringEnabled,
                _viewModel.JobBoardQueryModel.NotesQueryStringEnabled,
                _viewModel.JobBoardQueryModel.ColumnFiltersEnabled,
                _viewModel.JobBoardQueryModel.IsToBeAppliedTo,
                _viewModel.JobBoardQueryModel.IsAppliedTo,
                _viewModel.JobBoardQueryModel.IsInterviewing,
                _viewModel.JobBoardQueryModel.IsNegotiating,
                _viewModel.JobBoardQueryModel.IsRejected,
                _viewModel.JobBoardQueryModel.IsDeclinedOffer,
                _viewModel.JobBoardQueryModel.IsAcceptedOffer,
                _viewModel.JobBoardQueryModel.IsFavourite);

            _viewModel.PageIndex.Should().Be(0);
            _viewModel.JobListingsDisplayed.Should().NotBeNull();
            _viewModel.JobListingsDisplayed.Should().BeOfType<List<JobListingModel>>();
            _viewModel.JobListingsDisplayed.Count.Should().BeLessThanOrEqualTo(_viewModel.PageSize);

            if (_viewModel.JobListingsDisplayed.Any())
            {
                _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.EnableEvents.Should().BeTrue());
                _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.IsHidden.Should().BeFalse());
            }
        }

        [Fact]
        public void GoToNextPage_HasMorePages_UpdatesPageIndexAndJobListingsDisplayed()
        {
            // Arrange
            var initialPageIndex = 0;
            var jobListings = _fixture.CreateMany<JobListing>(_viewModel.PageSize * 2);
            _dbContext.GetAllJobListingsAsync().Returns(jobListings);
            _viewModel.RenderDefaultJobBoardViewCommand.ExecuteAsync(null);
            var initialJobListingsDisplayed = _viewModel.JobListingsDisplayed.ToList();

            // Act
            _viewModel.GoToNextPageCommand.Execute(null);

            // Assert
            _viewModel.PageIndex.Should().Be(initialPageIndex + 1);
            _viewModel.JobListingsDisplayed.Should().NotBeEquivalentTo(initialJobListingsDisplayed);
        }

        [Fact]
        public void GoToNextPage_NoMorePages_DoesNotChangePageIndexOrJobListingsDisplayed()
        {
            // Arrange
            var initialPageIndex = _fixture.Create<int>();
            var initialJobListingsDisplayed = _fixture.CreateMany<JobListingModel>().ToList();
            _viewModel.PageIndex = initialPageIndex;
            _viewModel.PageSize = 0;
            _viewModel.JobListingsDisplayed = initialJobListingsDisplayed;

            // Act
            _viewModel.GoToNextPageCommand.Execute(null);

            // Assert
            _viewModel.PageIndex.Should().Be(initialPageIndex);
            _viewModel.JobListingsDisplayed.Should().BeEquivalentTo(initialJobListingsDisplayed);
        }

        [Fact]
        public void GoToPreviousPage_AtFirstPage_DoesNotChangePageIndexOrJobListingsDisplayed()
        {
            // Arrange
            var initialPageIndex = 0;
            var initialJobListingsDisplayed = _fixture.CreateMany<JobListingModel>().ToList();
            _viewModel.PageIndex = initialPageIndex;
            _viewModel.JobListingsDisplayed = initialJobListingsDisplayed;

            // Act
            _viewModel.GoToPreviousPageCommand.Execute(null);

            // Assert
            _viewModel.PageIndex.Should().Be(initialPageIndex);
            _viewModel.JobListingsDisplayed.Should().BeEquivalentTo(initialJobListingsDisplayed);
        }

        [Fact]
        public void GoToPreviousPage_NotAtFirstPage_UpdatesPageIndexAndJobListingsDisplayed()
        {
            // Arrange
            var initialPageIndex = 10; // any positive integer
            var initialJobListingsDisplayed = _fixture.CreateMany<JobListingModel>().ToList();
            _viewModel.PageIndex = initialPageIndex;
            _viewModel.JobListingsDisplayed = initialJobListingsDisplayed;

            // Act
            _viewModel.GoToPreviousPageCommand.Execute(null);

            // Assert
            _viewModel.PageIndex.Should().Be(initialPageIndex - 1);
            _viewModel.JobListingsDisplayed.Should().NotBeEquivalentTo(initialJobListingsDisplayed);
        }

        [Fact]
        public void HideJob_SelectedJobListingIsNotNull_UpdatesPropertiesAndCollections()
        {
            // Arrange
            var selectedJobListing = _fixture.Create<JobListingModel>();
            selectedJobListing.IsHidden = false;
            selectedJobListing.EnableEvents = true;
            _viewModel.SelectedJobListing = selectedJobListing;

            bool wasCalled = false;
            JobListingModel.BoolFieldChanged += (sender, args) => wasCalled = true;

            // Act
            _viewModel.HideJobCommand.Execute(null);

            // Assert
            selectedJobListing.IsHidden.Should().BeTrue();
            _viewModel.JobListingsDisplayed.Should().NotContain(selectedJobListing);
            wasCalled.Should().BeTrue();
        }

        [Fact]
        public void HideJob_SelectedJobListingIsNull_DoesNotChangeCollections()
        {
            // Arrange
            _viewModel.SelectedJobListing = null;
            var initialJobListingsDisplayed = _fixture.CreateMany<JobListingModel>().ToList();
            _viewModel.JobListingsDisplayed = initialJobListingsDisplayed;

            // Act
            _viewModel.HideJobCommand.Execute(null);

            // Assert
            _viewModel.JobListingsDisplayed.Should().BeEquivalentTo(initialJobListingsDisplayed);
        }

        [Fact]
        public void OpenJobListing_SelectedJobListingIsNotNull_InvokesEvent()
        {
            // Arrange
            _viewModel.JobListingsDisplayed = _fixture.CreateMany<JobListingModel>().ToList();
            _viewModel.SelectedJobListing = _fixture.Create<JobListingModel>();
            bool wasCalled = false;
            _viewModel.OpenJobListingViewRequest += (job) => wasCalled = true;

            // Act
            _viewModel.OpenJobListingCommand.Execute(null);

            // Assert
            wasCalled.Should().BeTrue();
            _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.EnableEvents.Should().BeFalse());
        }

        [Fact]
        public void OpenJobListing_SelectedJobListingIsNull_DoesNotInvokeEvent()
        {
            // Arrange
            _viewModel.JobListingsDisplayed = _fixture.CreateMany<JobListingModel>().ToList();
            foreach (var job in _viewModel.JobListingsDisplayed)
            {
                job.EnableEvents = true;
            }

            _viewModel.SelectedJobListing = null;
            bool wasCalled = false;
            _viewModel.OpenJobListingViewRequest += (job) => wasCalled = true;

            // Act
            _viewModel.OpenJobListingCommand.Execute(null);

            // Assert
            wasCalled.Should().BeFalse();
            _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.EnableEvents.Should().BeTrue());
        }

        [Fact]
        public async Task RenderDefaultJobBoardView_UpdatesPropertiesCorrectly()
        {
            // Arrange
            var pageSize = _viewModel.PageSize;
            var jobListings = _fixture.CreateMany<JobListing>();

            foreach (var job in jobListings)
            {
                job.IsHidden = false;
            }

            _dbContext.GetAllJobListingsAsync().Returns(jobListings);

            // Act
            await _viewModel.RenderDefaultJobBoardViewCommand.ExecuteAsync(null);

            // Assert
            await _dbContext.Received().GetAllJobListingsAsync();
            _viewModel.PageIndex.Should().Be(0);
            _viewModel.JobListingsDisplayed.Should().NotBeNullOrEmpty();
            _viewModel.JobListingsDisplayed.Should().BeOfType<List<JobListingModel>>();
            _viewModel.JobListingsDisplayed.Count.Should().BeLessThanOrEqualTo(_viewModel.PageSize);
            _viewModel.JobListingsDisplayed.Count.Should().BeGreaterThan(0);
            _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.EnableEvents.Should().BeTrue());
            _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.IsHidden.Should().BeFalse());
        }

        [Fact]
        public async Task RenderFavouriteJobs_UpdatesPropertiesCorrectly()
        {
            // Arrange
            var pageSize = _viewModel.PageSize;
            var jobListings = _fixture.CreateMany<JobListing>();

            foreach (var jobListing in jobListings)
            {
                jobListing.IsFavourite = true;
                jobListing.IsHidden = false;
            }

            _dbContext.GetFavouriteJobListingsAsync().Returns(jobListings);

            // Act
            await _viewModel.RenderFavouriteJobsCommand.ExecuteAsync(null);

            // Assert
            await _dbContext.Received().GetFavouriteJobListingsAsync();
            _viewModel.PageIndex.Should().Be(0);
            _viewModel.JobListingsDisplayed.Should().NotBeNullOrEmpty();
            _viewModel.JobListingsDisplayed.Should().BeOfType<List<JobListingModel>>();
            _viewModel.JobListingsDisplayed.Count.Should().BeLessThanOrEqualTo(_viewModel.PageSize);
            _viewModel.JobListingsDisplayed.Count.Should().BeGreaterThan(0);
            _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.EnableEvents.Should().BeTrue());
            _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.IsHidden.Should().BeFalse());
            _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.IsFavourite.Should().BeTrue());
        }

        [Fact]
        public async Task RenderHiddenJobs_UpdatesPropertiesCorrectly()
        {
            // Arrange
            var pageSize = _viewModel.PageSize;
            var jobListings = _fixture.CreateMany<JobListing>();

            foreach (var jobListing in jobListings)
            {
                jobListing.IsHidden = true;
            }

            _dbContext.GetHiddenJobListingsAsync().Returns(jobListings);

            // Act
            await _viewModel.RenderHiddenJobsCommand.ExecuteAsync(null);

            // Assert
            await _dbContext.Received().GetHiddenJobListingsAsync();
            _viewModel.PageIndex.Should().Be(0);
            _viewModel.JobListingsDisplayed.Should().NotBeNullOrEmpty();
            _viewModel.JobListingsDisplayed.Should().BeOfType<List<JobListingModel>>();
            _viewModel.JobListingsDisplayed.Count.Should().BeLessThanOrEqualTo(_viewModel.PageSize);
            _viewModel.JobListingsDisplayed.Count.Should().BeGreaterThan(0);
            _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.EnableEvents.Should().BeTrue());
            _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.IsHidden.Should().BeTrue());
        }

        [Fact]
        public void UpdateJobBoard_UpdatesPropertiesCorrectly()
        {
            // Arrange
            Singletons.JobListings = _fixture.CreateMany<JobListingModel>().ToList();

            // Act
            _viewModel.UpdateJobBoard();

            // Assert
            _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.EnableEvents.Should().BeTrue());
        }
    }
}