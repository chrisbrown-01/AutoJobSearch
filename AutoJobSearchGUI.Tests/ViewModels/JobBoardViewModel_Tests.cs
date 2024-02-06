using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.ViewModels;
using AutoJobSearchShared.Models;
using FluentAssertions;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace AutoJobSearchGUI.Tests.ViewModels
{
    public class JobBoardViewModel_Tests
    {
        private readonly JobBoardViewModel _viewModel;
        private readonly IDbContext _dbContext;
        private readonly IFixture _fixture;

        public JobBoardViewModel_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _dbContext = Substitute.For<IDbContext>();
            _viewModel = new JobBoardViewModel(_dbContext);
        }

        [Fact]
        public async void RenderDefaultJobBoardView_UpdatesPropertiesCorrectly()
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
        public async void RenderHiddenJobs_UpdatesPropertiesCorrectly()
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
        public async void RenderFavouriteJobs_UpdatesPropertiesCorrectly()
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
        public async void ExecuteQuery_UpdatesPropertiesCorrectly()
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
            jobBoardQueryModel.SearchedBetweenDatesEnabled = false;
            jobBoardQueryModel.SearchedOnDateEnabled = false;
            jobBoardQueryModel.ScoreEqualsEnabled = false;
            jobBoardQueryModel.ScoreRangeEnabled = false;

            _viewModel.JobBoardQueryModel = jobBoardQueryModel;

            _dbContext.ExecuteJobListingQueryAsync(
                _viewModel.JobBoardQueryModel.ColumnFiltersEnabled,
                _viewModel.JobBoardQueryModel.IsAppliedTo,
                _viewModel.JobBoardQueryModel.IsInterviewing,
                _viewModel.JobBoardQueryModel.IsRejected,
                _viewModel.JobBoardQueryModel.IsFavourite).
                Returns(jobListings.AsQueryable());

            // Act
            await _viewModel.ExecuteQueryCommand.ExecuteAsync(null);

            // Assert
            await _dbContext.Received().ExecuteJobListingQueryAsync(
                _viewModel.JobBoardQueryModel.ColumnFiltersEnabled,
                _viewModel.JobBoardQueryModel.IsAppliedTo,
                _viewModel.JobBoardQueryModel.IsInterviewing,
                _viewModel.JobBoardQueryModel.IsRejected,
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
            _viewModel.OpenJobListingViewRequest += (job, jobs) => wasCalled = true;

            // Act
            _viewModel.OpenJobListingCommand.Execute(null);

            // Assert
            wasCalled.Should().BeFalse();
            _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.EnableEvents.Should().BeTrue());
        }

        [Fact]
        public void OpenJobListing_SelectedJobListingIsNotNull_InvokesEvent()
        {
            // Arrange
            _viewModel.JobListingsDisplayed = _fixture.CreateMany<JobListingModel>().ToList();
            _viewModel.SelectedJobListing = _fixture.Create<JobListingModel>();
            bool wasCalled = false;
            _viewModel.OpenJobListingViewRequest += (job, jobs) => wasCalled = true;

            // Act
            _viewModel.OpenJobListingCommand.Execute(null);

            // Assert
            wasCalled.Should().BeTrue();
            _viewModel.JobListingsDisplayed.Should().AllSatisfy(x => x.EnableEvents.Should().BeFalse());
        }

        [Fact]
        public void HideJob_SelectedJobListingIsNull_DoesNotChangeCollections()
        {
            // Arrange
            _viewModel.SelectedJobListing = null;
            var initialJobListingsDisplayed = _fixture.CreateMany<JobListingModel>().ToList();
            _viewModel.JobListingsDisplayed = initialJobListingsDisplayed;

            // Act
            _viewModel.HideJob();

            // Assert
            _viewModel.JobListingsDisplayed.Should().BeEquivalentTo(initialJobListingsDisplayed);
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
            _viewModel.HideJob();

            // Assert
            selectedJobListing.IsHidden.Should().BeTrue();
            _viewModel.JobListingsDisplayed.Should().NotContain(selectedJobListing);
            wasCalled.Should().BeTrue();
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
            _viewModel.GoToNextPage();

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
            _viewModel.GoToPreviousPage();

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
            _viewModel.GoToPreviousPage();

            // Assert
            _viewModel.PageIndex.Should().Be(initialPageIndex - 1);
            _viewModel.JobListingsDisplayed.Should().NotBeEquivalentTo(initialJobListingsDisplayed);
        }

        //[Fact]
        //public async void DeleteAllRecords_UserConfirms_DeletesAllRecords()
        //{
        //    // Arrange
        //    var box = Substitute.For<IMsBox<MsBox.Avalonia.Enums.ButtonResult>>();
        //    box.ShowAsync().Returns(MsBox.Avalonia.Enums.ButtonResult.Ok);
        //    MessageBoxManager.GetMessageBoxStandard(
        //        Arg.Any<string>(),
        //        Arg.Any<string>(),
        //        Arg.Any<MsBox.Avalonia.Enums.ButtonEnum>(),
        //        Arg.Any<MsBox.Avalonia.Enums.Icon>())
        //        .Returns(box);

        //    // Act
        //    _viewModel.DeleteAllRecords();

        //    // Assert
        //    await _dbContext.Received().DeleteAllJobListingsAsync();
        //}

        //[Fact]
        //public void GoToNextPage_HasMorePages_UpdatesPageIndexAndJobListingsDisplayed()
        //{
        //    // Arrange
        //    var initialPageIndex = _fixture.Create<int>();
        //    var initialJobListingsDisplayed = _fixture.CreateMany<JobListingModel>().ToList();
        //    _viewModel.PageIndex = initialPageIndex;
        //    _viewModel.JobListingsDisplayed = initialJobListingsDisplayed;

        //    // Act
        //    _viewModel.GoToNextPage();

        //    // Assert
        //    _viewModel.PageIndex.Should().Be(initialPageIndex + 1);
        //    _viewModel.JobListingsDisplayed.Should().NotBeEquivalentTo(initialJobListingsDisplayed);
        //}
    }
}
