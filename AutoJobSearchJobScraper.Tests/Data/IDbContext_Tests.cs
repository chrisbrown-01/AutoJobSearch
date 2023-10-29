using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoJobSearchJobScraper;
using AutoJobSearchJobScraper.Data;
using AutoJobSearchShared.Models;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;

namespace AutoJobSearchJobScraper.Tests.Data
{
    public class IDbContext_Tests
    {
        private readonly IDbContext _dbContext;
        private readonly JobSearchProfile _jobSearchProfile;
        private readonly IEnumerable<JobListing> _jobListings;
        private readonly IEnumerable<string> _applicationLinks;
        private readonly IFixture _fixture;

        public IDbContext_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _dbContext = Substitute.For<IDbContext>();

            _jobSearchProfile = _fixture.Create<JobSearchProfile>();
            _jobListings = _fixture.CreateMany<JobListing>();
            _applicationLinks = _fixture.CreateMany<string>();
        }

        [Fact]
        public async Task GetJobSearchProfileByIdAsync_Should_BeCalled()
        {
            // Arrange
            var testInt = _fixture.Create<int>();

            // Act
            await _dbContext.GetJobSearchProfileByIdAsync(testInt);

            // Assert
            await _dbContext.Received().GetJobSearchProfileByIdAsync(testInt);
        }

        [Fact]
        public async Task GetJobSearchProfileByIdAsync_Should_ReturnProfile()
        {
            // Arrange
            int id = Arg.Any<int>();
            _dbContext.GetJobSearchProfileByIdAsync(id).Returns(_jobSearchProfile);

            // Act
            var result = await _dbContext.GetJobSearchProfileByIdAsync(id);

            // Assert
            result.Should().Be(_jobSearchProfile);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetJobSearchProfileByIdAsync_Should_ReturnNull()
        {
            // Arrange
            int id = Arg.Any<int>();
            _dbContext.GetJobSearchProfileByIdAsync(id).Returns((JobSearchProfile?)null);

            // Act
            var result = await _dbContext.GetJobSearchProfileByIdAsync(id);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllApplicationLinksAsync_Should_BeCalled()
        {
            // Act
            await _dbContext.GetAllApplicationLinksAsync();

            // Assert
            await _dbContext.Received().GetAllApplicationLinksAsync();
        }

        [Fact]
        public async Task GetAllApplicationLinksAsync_Should_ReturnNonEmptyResult()
        {
            // Arrange
            _dbContext.GetAllApplicationLinksAsync().Returns(_applicationLinks);

            // Act
            var result = await _dbContext.GetAllApplicationLinksAsync();

            // Assert
            result.Should().BeEquivalentTo(_applicationLinks);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetAllApplicationLinksAsync_Should_ReturnEmptyResult()
        {
            // Arrange
            IEnumerable<string> emptyResult = new List<string>();
            _dbContext.GetAllApplicationLinksAsync().Returns(emptyResult);

            // Act
            var result = await _dbContext.GetAllApplicationLinksAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task SaveJobListingsAsync_Should_BeCalled()
        {
            // Act
            await _dbContext.SaveJobListingsAsync(_jobListings);

            // Assert
            await _dbContext.Received().SaveJobListingsAsync(_jobListings);
        }
    }
}
