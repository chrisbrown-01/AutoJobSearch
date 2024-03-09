using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Data;
using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Tests.Data
{
    public class IDbContext_Tests
    {
        private readonly IFixture _fixture;
        private readonly IDbContext _dbContext;

        public IDbContext_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _dbContext = Substitute.For<IDbContext>();
        }

        [Fact]
        public async Task DeleteAllJobListingsAsync_Should_BeCalled()
        {
            // Act
            await _dbContext.DeleteAllJobListingsAsync();

            // Assert
            await _dbContext.Received().DeleteAllJobListingsAsync();
        }

        [Fact]
        public async Task DeleteJobSearchProfileAsync_Should_BeCalled()
        {
            // Arrange
            var testInt = _fixture.Create<int>();

            // Act
            await _dbContext.DeleteJobSearchProfileAsync(testInt);

            // Assert
            await _dbContext.Received().DeleteJobSearchProfileAsync(testInt);
        }

        [Fact]
        public async Task GetAllJobSearchProfilesAsync_Should_ReturnNonEmptyResult()
        {
            // Arrange
            var jobSearchProfiles = _fixture.CreateMany<JobSearchProfile>();
            _dbContext.GetAllJobSearchProfilesAsync().Returns(jobSearchProfiles);

            // Act
            var result = await _dbContext.GetAllJobSearchProfilesAsync();

            // Assert
            await _dbContext.Received().GetAllJobSearchProfilesAsync();
            result.Should().BeEquivalentTo(jobSearchProfiles);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetAllJobSearchProfilesAsync_Should_ReturnEmptyResult()
        {
            // Arrange
            IEnumerable<JobSearchProfile> jobSearchProfiles = new List<JobSearchProfile>();
            _dbContext.GetAllJobSearchProfilesAsync().Returns(jobSearchProfiles);

            // Act
            var result = await _dbContext.GetAllJobSearchProfilesAsync();

            // Assert
            await _dbContext.Received().GetAllJobSearchProfilesAsync();
            result.Should().BeEquivalentTo(jobSearchProfiles);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateJobSearchProfileAsync_Should_ReturnNonEmptyResult()
        {
            // Arrange
            var argument = _fixture.Create<JobSearchProfile>();
            var expectedResult = _fixture.Create<JobSearchProfile>();
            _dbContext.CreateJobSearchProfileAsync(argument).Returns(expectedResult);

            // Act
            var result = await _dbContext.CreateJobSearchProfileAsync(argument);

            await _dbContext.Received().CreateJobSearchProfileAsync(Arg.Any<JobSearchProfile>());
            result.Should().BeEquivalentTo(expectedResult);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateJobListingBoolPropertyAsync_Should_BeCalled()
        {
            // Arrange
            var columnName = _fixture.Create<JobListingsBoolField>();
            var value = _fixture.Create<bool>();
            var id = _fixture.Create<int>();
            var statusModifiedAt = _fixture.Create<DateTime>();

            // Act
            await _dbContext.UpdateJobListingBoolPropertyAsync(columnName, value, id, statusModifiedAt);

            // Assert
            await _dbContext.Received().UpdateJobListingBoolPropertyAsync(columnName, value, id, statusModifiedAt);
        }

        [Fact]
        public async Task UpdateJobListingStringPropertyAsync_Should_BeCalled()
        {
            // Arrange
            var columnName = _fixture.Create<JobListingsStringField>();
            var value = _fixture.Create<string>();
            var id = _fixture.Create<int>();

            // Act
            await _dbContext.UpdateJobListingStringPropertyAsync(columnName, value, id);

            // Assert
            await _dbContext.Received().UpdateJobListingStringPropertyAsync(columnName, value, id);
        }

        [Fact]
        public async Task UpdateJobSearchProfileStringPropertyAsync_Should_BeCalled()
        {
            // Arrange
            var columnName = _fixture.Create<JobSearchProfilesStringField>();
            var value = _fixture.Create<string>();
            var id = _fixture.Create<int>();

            // Act
            await _dbContext.UpdateJobSearchProfileStringPropertyAsync(columnName, value, id);

            // Assert
            await _dbContext.Received().UpdateJobSearchProfileStringPropertyAsync(columnName, value, id);
        }

        [Fact]
        public async Task ExecuteJobListingQueryAsync_Should_ReturnIQueryable()
        {
            // Arrange
            bool descriptionFilterEnabled = _fixture.Create<bool>();
            bool notesFilterEnabled = _fixture.Create<bool>();
            bool columnFilterEnabled = _fixture.Create<bool>();
            bool isToBeAppliedTo = _fixture.Create<bool>();
            bool isAppliedTo = _fixture.Create<bool>();
            bool isInterviewing = _fixture.Create<bool>();
            bool isNegotiating = _fixture.Create<bool>();
            bool isRejected = _fixture.Create<bool>();
            bool isDeclinedOffer = _fixture.Create<bool>();
            bool isAcceptedOffer = _fixture.Create<bool>();
            bool isFavourite = _fixture.Create<bool>();
            var expectedResult = _fixture.Create<IQueryable<JobListing>>();

            _dbContext.ExecuteJobListingQueryAsync(
                 descriptionFilterEnabled,
                 notesFilterEnabled,
                 columnFilterEnabled,
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
            var actualResult = await _dbContext.ExecuteJobListingQueryAsync(
                 descriptionFilterEnabled,
                 notesFilterEnabled,
                 columnFilterEnabled,
                 isToBeAppliedTo,
                 isAppliedTo,
                 isInterviewing,
                 isNegotiating,
                 isRejected,
                 isDeclinedOffer,
                 isAcceptedOffer,
                 isFavourite);

            // Assert
            await _dbContext.Received().ExecuteJobListingQueryAsync(
                descriptionFilterEnabled,
                notesFilterEnabled,
                columnFilterEnabled,
                isToBeAppliedTo,
                isAppliedTo,
                isInterviewing,
                isNegotiating,
                isRejected,
                isDeclinedOffer,
                isAcceptedOffer,
                isFavourite);

            actualResult.Should().BeEquivalentTo(expectedResult);
            actualResult.Should().NotBeNull();
        }

        [Fact]
        public async Task GetFavouriteJobListingsAsync_Should_BeCalled()
        {
            await _dbContext.GetFavouriteJobListingsAsync();

            await _dbContext.Received().GetFavouriteJobListingsAsync();
        }

        [Fact]
        public async Task GetFavouriteJobListingsAsync_Should_Return_NonEmptyIEnumerable()
        {
            // Arrange
            var favouriteListings = _fixture.CreateMany<JobListing>();
            _dbContext.GetFavouriteJobListingsAsync().Returns(favouriteListings);

            // Act
            var result = await _dbContext.GetFavouriteJobListingsAsync();

            // Assert
            result.Should().BeEquivalentTo(favouriteListings);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetFavouriteJobListingsAsync_Should_Return_EmptyIEnumerable()
        {
            // Arrange
            IEnumerable<JobListing> favouriteListings = new List<JobListing>();
            _dbContext.GetFavouriteJobListingsAsync().Returns(favouriteListings);

            // Act
            var result = await _dbContext.GetFavouriteJobListingsAsync();

            // Assert
            result.Should().BeEquivalentTo(favouriteListings);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetHiddenJobListingsAsync_Should_BeCalled()
        {
            await _dbContext.GetHiddenJobListingsAsync();

            await _dbContext.Received().GetHiddenJobListingsAsync();
        }

        [Fact]
        public async Task GetHiddenJobListingsAsync_Should_Return_NonEmptyIEnumerable()
        {
            // Arrange
            var hiddenListings = _fixture.CreateMany<JobListing>();
            _dbContext.GetHiddenJobListingsAsync().Returns(hiddenListings);

            // Act
            var result = await _dbContext.GetHiddenJobListingsAsync();

            // Assert
            result.Should().BeEquivalentTo(hiddenListings);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetHiddenJobListingsAsync_Should_Return_EmptyIEnumerable()
        {
            // Arrange
            IEnumerable<JobListing> hiddenListings = new List<JobListing>();
            _dbContext.GetHiddenJobListingsAsync().Returns(hiddenListings);

            // Act
            var result = await _dbContext.GetHiddenJobListingsAsync();

            // Assert
            result.Should().BeEquivalentTo(hiddenListings);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllJobListingsAsync_Should_BeCalled()
        {
            await _dbContext.GetAllJobListingsAsync();

            await _dbContext.Received().GetAllJobListingsAsync();
        }

        [Fact]
        public async Task GetAllJobListingsAsync_Should_Return_NonEmptyIEnumerable()
        {
            // Arrange
            var allListings = _fixture.CreateMany<JobListing>();
            _dbContext.GetAllJobListingsAsync().Returns(allListings);

            // Act
            var result = await _dbContext.GetAllJobListingsAsync();

            // Assert
            result.Should().BeEquivalentTo(allListings);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetAllJobListingsAsync_Should_Return_EmptyIEnumerable()
        {
            // Arrange
            IEnumerable<JobListing> allListings = new List<JobListing>();
            _dbContext.GetAllJobListingsAsync().Returns(allListings);

            // Act
            var result = await _dbContext.GetAllJobListingsAsync();

            // Assert
            result.Should().BeEquivalentTo(allListings);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetJobListingDetailsByIdAsync_Should_ReturnJobListing()
        {
            // Arrange
            var id = Arg.Any<int>();
            var expectedJobListing = _fixture.Create<JobListing>();
            _dbContext.GetJobListingDetailsByIdAsync(id).Returns(expectedJobListing);

            // Act
            var actualJobListing = await _dbContext.GetJobListingDetailsByIdAsync(id);

            // Assert
            actualJobListing.Should().BeEquivalentTo(expectedJobListing);
            actualJobListing.Should().NotBeNull();
        }

        [Fact]
        public async Task GetJobListingDetailsByIdAsync_Should_ThrowException_When_NoRecordsFound()
        {
            // Arrange
            var id = Arg.Any<int>();
            _dbContext.GetJobListingDetailsByIdAsync(id).Returns(Task.FromException<JobListing>(new Exception()));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _dbContext.GetJobListingDetailsByIdAsync(id));
        }
    }
}
