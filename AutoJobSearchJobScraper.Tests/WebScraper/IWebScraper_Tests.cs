using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchJobScraper.WebScraper;
using AutoJobSearchShared.Models;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchJobScraper.Tests.WebScraper
{
    public class IWebScraper_Tests
    {
        private readonly IFixture _fixture;
        private readonly IWebScraper _webScraper;
        private readonly IEnumerable<string> _searchTerms;
        private readonly int? _maxJobListingIndex;

        public IWebScraper_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _webScraper = Substitute.For<IWebScraper>();

            _searchTerms = _fixture.CreateMany<string>();
            _maxJobListingIndex = _fixture.Create<int>();
        }

        [Fact]
        public async Task ScrapeJobsAsync_Should_BeCalled()
        {
            await _webScraper.ScrapeJobsAsync(_searchTerms, _maxJobListingIndex);

            await _webScraper.Received().ScrapeJobsAsync(_searchTerms, _maxJobListingIndex);
        }

        [Fact]
        public async Task ScrapeJobsAsync_Should_Return_NonNullResult()
        {
            var result = await _webScraper.ScrapeJobsAsync(_searchTerms, _maxJobListingIndex);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ScrapeJobsAsync_Should_Return_NonEmptyResult()
        {
            // Arrange
            var expectedResult = _fixture.CreateMany<JobListing>();
            _webScraper.ScrapeJobsAsync(_searchTerms, _maxJobListingIndex).Returns(expectedResult);

            // Act
            var result = await _webScraper.ScrapeJobsAsync(_searchTerms, _maxJobListingIndex);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ScrapeJobsAsync_Should_Return_EmptyResult()
        {
            // Arrange
            IEnumerable<JobListing> expectedResult = new List<JobListing>();
            _webScraper.ScrapeJobsAsync(_searchTerms, _maxJobListingIndex).Returns(expectedResult);

            // Act
            var result = await _webScraper.ScrapeJobsAsync(_searchTerms, _maxJobListingIndex);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
