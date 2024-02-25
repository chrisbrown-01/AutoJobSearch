using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchShared.Database;
using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchShared.Tests.Database
{
    public class IAutoJobSearchDb_Tests
    {
        private readonly IAutoJobSearchDb _db;
        private readonly ApplicationLink _applicationLink;
        private readonly JobListing _jobListing;
        private readonly JobSearchProfile _jobSearchProfile;
        private readonly IFixture _fixture;

        public IAutoJobSearchDb_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _db = Substitute.For<IAutoJobSearchDb>();

            _applicationLink = _fixture.Create<ApplicationLink>();
            _jobListing = _fixture.Create<JobListing>();
            _jobSearchProfile = _fixture.Create<JobSearchProfile>();
        }

        [Fact]
        public async Task CreateJobSearchProfileAsync_Should_CreateProfile()
        {
            // Arrange
            var returnedJobSearchProfile = _fixture.Create<JobSearchProfile>();
            _db.CreateJobSearchProfileAsync(Arg.Any<JobSearchProfile>()).Returns(returnedJobSearchProfile);

            // Act
            var createdProfile = await _db.CreateJobSearchProfileAsync(_jobSearchProfile);

            // Assert
            createdProfile.Should().NotBeNull();
            createdProfile.Should().BeOfType<JobSearchProfile>();
            createdProfile.Should().NotBeEquivalentTo(_jobSearchProfile);
        }

        [Fact]
        public async Task DeleteAllJobListingsAsync_Should_BeCalled()
        {
            await _db.DeleteAllJobListingsAsync();

            await _db.Received().DeleteAllJobListingsAsync();
        }

        [Fact]
        public async Task DeleteJobSearchProfileAsync_Should_BeCalled()
        {
            var testInt = _fixture.Create<int>();

            await _db.DeleteJobSearchProfileAsync(testInt);

            await _db.Received().DeleteJobSearchProfileAsync(testInt);
        }

        [Fact]
        public async Task SaveJobListingsAsync_Should_BeCalled()
        {
            var jobListings = _fixture.CreateMany<JobListing>();

            await _db.SaveJobListingsAsync(jobListings);

            await _db.Received().SaveJobListingsAsync(jobListings);
        }

        [Fact]
        public async Task ExecuteJobListingQueryAsync_Should_BeCalled()
        {
            // Arrange
            bool columnFiltersEnabled = _fixture.Create<bool>();
            bool isToBeAppliedTo = _fixture.Create<bool>();
            bool isAppliedTo = _fixture.Create<bool>();
            bool isInterviewing = _fixture.Create<bool>();
            bool isNegotiating = _fixture.Create<bool>();
            bool isRejected = _fixture.Create<bool>();
            bool isDeclinedOffer = _fixture.Create<bool>();
            bool isAcceptedOffer = _fixture.Create<bool>();
            bool isFavourite = _fixture.Create<bool>();

            // Act
            await _db.ExecuteJobListingQueryAsync(
                columnFiltersEnabled, 
                isToBeAppliedTo,
                isAppliedTo, 
                isInterviewing, 
                isNegotiating,
                isRejected, 
                isDeclinedOffer,
                isAcceptedOffer,
                isFavourite);

            // Assert
            await _db.Received().ExecuteJobListingQueryAsync(
                columnFiltersEnabled,
                isToBeAppliedTo,
                isAppliedTo,
                isInterviewing,
                isNegotiating,
                isRejected,
                isDeclinedOffer,
                isAcceptedOffer,
                isFavourite);
        }

        [Fact]
        public async Task ExecuteJobListingQueryAsync_Should_ReturnIQueryable()
        {
            // Arrange
            bool columnFiltersEnabled = _fixture.Create<bool>();
            bool isToBeAppliedTo = _fixture.Create<bool>();
            bool isAppliedTo = _fixture.Create<bool>();
            bool isInterviewing = _fixture.Create<bool>();
            bool isNegotiating = _fixture.Create<bool>();
            bool isRejected = _fixture.Create<bool>();
            bool isDeclinedOffer = _fixture.Create<bool>();
            bool isAcceptedOffer = _fixture.Create<bool>();
            bool isFavourite = _fixture.Create<bool>();
            var expectedResult = _fixture.Create<IQueryable<JobListing>>();

            _db.ExecuteJobListingQueryAsync(
                columnFiltersEnabled,
                isToBeAppliedTo,
                isAppliedTo,
                isInterviewing,
                isNegotiating,
                isRejected,
                isDeclinedOffer,
                isAcceptedOffer,
                isFavourite)
                .Returns(expectedResult);

            // Act
            var actualResult = await _db.ExecuteJobListingQueryAsync(
                columnFiltersEnabled,
                isToBeAppliedTo,
                isAppliedTo,
                isInterviewing,
                isNegotiating,
                isRejected,
                isDeclinedOffer,
                isAcceptedOffer,
                isFavourite);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
            actualResult.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllApplicationLinksAsync_Should_BeCalled()
        {
            await _db.GetAllApplicationLinksAsync();

            await _db.Received().GetAllApplicationLinksAsync();
        }

        [Fact]
        public async Task GetAllApplicationLinksAsync_Should_Return_NonEmptyIEnumerable()
        {
            // Arrange
            var applicationLinks = _fixture.CreateMany<string>();
            _db.GetAllApplicationLinksAsync().Returns(applicationLinks);

            // Act
            var result = await _db.GetAllApplicationLinksAsync();

            // Assert
            result.Should().BeEquivalentTo(applicationLinks);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetAllApplicationLinksAsync_Should_Return_EmptyIEnumerable()
        {
            // Arrange
            IEnumerable<string> applicationLinks = new List<string>();
            _db.GetAllApplicationLinksAsync().Returns(applicationLinks);

            // Act
            var result = await _db.GetAllApplicationLinksAsync();

            // Assert
            result.Should().BeEquivalentTo(applicationLinks);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllJobListingsAsync_Should_BeCalled()
        {
            await _db.GetAllJobListingsAsync();

            await _db.Received().GetAllJobListingsAsync();
        }

        [Fact]
        public async Task GetAllJobListingsAsync_Should_Return_NonEmptyIEnumerable()
        {
            // Arrange
            var jobListings = _fixture.CreateMany<JobListing>();
            _db.GetAllJobListingsAsync().Returns(jobListings);

            // Act
            var result = await _db.GetAllJobListingsAsync();

            // Assert
            result.Should().BeEquivalentTo(jobListings);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetAllJobListingsAsync_Should_Return_EmptyIEnumerable()
        {
            // Arrange
            IEnumerable<JobListing> jobListings = new List<JobListing>();
            _db.GetAllJobListingsAsync().Returns(jobListings);

            // Act
            var result = await _db.GetAllJobListingsAsync();

            // Assert
            result.Should().BeEquivalentTo(jobListings);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllJobSearchProfilesAsync_Should_BeCalled()
        {
            await _db.GetAllJobSearchProfilesAsync();

            await _db.Received().GetAllJobSearchProfilesAsync();
        }

        [Fact]
        public async Task GetAllJobSearchProfilesAsync_Should_Return_NonEmptyIEnumerable()
        {
            // Arrange
            var jobSearchProfiles = _fixture.CreateMany<JobSearchProfile>();
            _db.GetAllJobSearchProfilesAsync().Returns(jobSearchProfiles);

            // Act
            var result = await _db.GetAllJobSearchProfilesAsync();

            // Assert
            result.Should().BeEquivalentTo(jobSearchProfiles);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetAllJobSearchProfilesAsync_Should_Return_EmptyIEnumerable()
        {
            // Arrange
            IEnumerable<JobSearchProfile> jobSearchProfiles = new List<JobSearchProfile>();
            _db.GetAllJobSearchProfilesAsync().Returns(jobSearchProfiles);

            // Act
            var result = await _db.GetAllJobSearchProfilesAsync();

            // Assert
            result.Should().BeEquivalentTo(jobSearchProfiles);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetFavouriteJobListingsAsync_Should_BeCalled()
        {
            await _db.GetFavouriteJobListingsAsync();

            await _db.Received().GetFavouriteJobListingsAsync();
        }

        [Fact]
        public async Task GetFavouriteJobListingsAsync_Should_Return_NonEmptyIEnumerable()
        {
            // Arrange
            var favouriteListings = _fixture.CreateMany<JobListing>();
            _db.GetFavouriteJobListingsAsync().Returns(favouriteListings);

            // Act
            var result = await _db.GetFavouriteJobListingsAsync();

            // Assert
            result.Should().BeEquivalentTo(favouriteListings);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetFavouriteJobListingsAsync_Should_Return_EmptyIEnumerable()
        {
            // Arrange
            IEnumerable<JobListing> favouriteListings = new List<JobListing>();
            _db.GetFavouriteJobListingsAsync().Returns(favouriteListings);

            // Act
            var result = await _db.GetFavouriteJobListingsAsync();

            // Assert
            result.Should().BeEquivalentTo(favouriteListings);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetHiddenJobListingsAsyn_Should_BeCalled()
        {
            await _db.GetHiddenJobListingsAsync();

            await _db.Received().GetHiddenJobListingsAsync();
        }

        [Fact]
        public async Task GetHiddenJobListingsAsync_Should_Return_NonEmptyIEnumerable()
        {
            // Arrange
            var hiddenListings = _fixture.CreateMany<JobListing>();
            _db.GetHiddenJobListingsAsync().Returns(hiddenListings);

            // Act
            var result = await _db.GetHiddenJobListingsAsync();

            // Assert
            result.Should().BeEquivalentTo(hiddenListings);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetHiddenJobListingsAsync_Should_Return_EmptyIEnumerable()
        {
            // Arrange
            IEnumerable<JobListing> hiddenListings = new List<JobListing>();
            _db.GetHiddenJobListingsAsync().Returns(hiddenListings);

            // Act
            var result = await _db.GetHiddenJobListingsAsync();

            // Assert
            result.Should().BeEquivalentTo(hiddenListings);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetJobListingDetailsByIdAsync_Should_ReturnJobListing()
        {
            // Arrange
            var id = Arg.Any<int>();
            var expectedJobListing = _fixture.Create<JobListing>();
            _db.GetJobListingDetailsByIdAsync(id).Returns(expectedJobListing);

            // Act
            var actualJobListing = await _db.GetJobListingDetailsByIdAsync(id);

            // Assert
            actualJobListing.Should().BeEquivalentTo(expectedJobListing);
            actualJobListing.Should().NotBeNull();
        }

        [Fact]
        public async Task GetJobListingDetailsByIdAsync_Should_ThrowException_When_NoRecordsFound()
        {
            // Arrange
            var id = Arg.Any<int>();
            _db.GetJobListingDetailsByIdAsync(id).Returns(Task.FromException<JobListing>(new Exception()));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _db.GetJobListingDetailsByIdAsync(id));
        }

        [Fact]
        public async Task GetJobSearchProfileByIdAsync_Should_ReturnNonNullResult_When_ProfileExists()
        {
            // Arrange
            var id = Arg.Any<int>();
            var expectedProfile = _fixture.Create<JobSearchProfile>();
            _db.GetJobSearchProfileByIdAsync(id).Returns(expectedProfile);

            // Act
            var actualProfile = await _db.GetJobSearchProfileByIdAsync(id);

            // Assert
            actualProfile.Should().NotBeNull();
            actualProfile.Should().BeEquivalentTo(expectedProfile);
        }

        [Fact]
        public async Task GetJobSearchProfileByIdAsync_Should_ReturnNull_When_ProfileDoesNotExist()
        {
            // Arrange
            var id = Arg.Any<int>();
            _db.GetJobSearchProfileByIdAsync(id).Returns((JobSearchProfile?)null);

            // Act
            var actualProfile = await _db.GetJobSearchProfileByIdAsync(id);

            // Assert
            actualProfile.Should().BeNull();
        }

        [Fact]
        public async Task UpdateJobListingBoolPropertyAsync_Should_BeCalled()
        {
            // Arrange
            var columnName = _fixture.Create<JobListingsBoolField>();
            bool value = _fixture.Create<bool>();
            int id = _fixture.Create<int>();
            var statusModifiedAt = _fixture.Create<DateTime>();

            // Act
            await _db.UpdateJobListingBoolPropertyAsync(columnName, value, id, statusModifiedAt);

            // Assert
            await _db.Received().UpdateJobListingBoolPropertyAsync(columnName, value, id, statusModifiedAt);
        }

        [Fact]
        public async Task UpdateJobListingStringPropertyAsync_Should_BeCalled()
        {
            // Arrange
            var columnName = _fixture.Create<JobListingsStringField>();
            string value = _fixture.Create<string>();
            int id = _fixture.Create<int>();

            // Act
            await _db.UpdateJobListingStringPropertyAsync(columnName, value, id);

            // Assert
            await _db.Received().UpdateJobListingStringPropertyAsync(columnName, value, id);
        }

        [Fact]
        public async Task UpdateJobSearchProfileStringPropertyAsync_Should_BeCalled()
        {
            // Arrange
            var columnName = _fixture.Create<JobSearchProfilesStringField>();
            string value = _fixture.Create<string>();
            int id = _fixture.Create<int>();

            // Act
            await _db.UpdateJobSearchProfileStringPropertyAsync(columnName, value, id);

            // Assert
            await _db.Received().UpdateJobSearchProfileStringPropertyAsync(columnName, value, id);
        }
    }
}
