using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchJobScraper.Utility;
using AutoJobSearchShared.Models;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchJobScraper.Tests.Utility
{
    public class JobListingUtility_Tests
    {
        private readonly IFixture _fixture;
        private readonly ILogger<JobListingUtility> _logger;
        private readonly JobListingUtility _jobListingUtility;

        public JobListingUtility_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _logger = Substitute.For<ILogger<JobListingUtility>>();
            _jobListingUtility = new JobListingUtility(_logger);
        }

        [Fact]
        public async Task FilterDuplicatesAsync_Should_RemoveDuplicates_UsingExistingLinks()
        {
            // Arrange
            var jobListingsPossibleDuplicates = _fixture.CreateMany<JobListing>().ToList();
            var existingApplicationLinks = _fixture.CreateMany<string>();

            foreach (var test in jobListingsPossibleDuplicates)
            {
                test.ApplicationLinks = new List<ApplicationLink>();

                foreach (var existingLink in existingApplicationLinks)
                {
                    test.ApplicationLinks.Add(new ApplicationLink { Link = existingLink });
                }
            }

            // Act
            var result = await _jobListingUtility.FilterDuplicatesAsync(
                jobListingsPossibleDuplicates,
                existingApplicationLinks.ToHashSet());

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(0);
        }

        [Fact]
        public async Task FilterDuplicatesAsync_Should_RemoveDuplicates_FromArgumentList()
        {
            // Arrange
            var jobListingsPossibleDuplicates_half = _fixture.CreateMany<JobListing>().ToList();
            var jobListingsPossibleDuplicates = jobListingsPossibleDuplicates_half;
            jobListingsPossibleDuplicates.AddRange(jobListingsPossibleDuplicates_half);

            var existingApplicationLinks = _fixture.CreateMany<string>();

            // Act
            var result = await _jobListingUtility.FilterDuplicatesAsync(
                jobListingsPossibleDuplicates,
                existingApplicationLinks.ToHashSet());

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().NotBe(0);
            result.Count().Should().Be(jobListingsPossibleDuplicates.Count / 2);
            result.Count().Should().NotBe(jobListingsPossibleDuplicates.Count);
        }

        [Fact]
        public async Task ApplyScoringsAsync_Should_OnlyModifyScore()
        {
            // Arrange
            var jobListingsUnscored = _fixture.CreateMany<JobListing>().ToList();
            var keywordsPositive = _fixture.CreateMany<string>();
            var keywordsNegative = _fixture.CreateMany<string>();
            var sentimentsPositive = _fixture.CreateMany<string>();
            var sentimentsNegative = _fixture.CreateMany<string>();

            // Act
            var results = await _jobListingUtility.ApplyScoringsAsync(
                jobListingsUnscored,
                keywordsPositive,
                keywordsNegative,
                sentimentsPositive,
                sentimentsNegative);

            // Assert
            results.Should().NotBeNullOrEmpty();
            results.Count().Should().Be(jobListingsUnscored.Count);

            var resultsList = results.ToList();

            for (int i = 0; i < jobListingsUnscored.Count(); i++)
            {
                jobListingsUnscored[i].Id.Should().Be(resultsList[i].Id);
                jobListingsUnscored[i].SearchTerm.Should().Be(resultsList[i].SearchTerm);
                jobListingsUnscored[i].CreatedAt.Should().Be(resultsList[i].CreatedAt);
                jobListingsUnscored[i].Description_Raw.Should().Be(resultsList[i].Description_Raw);
                jobListingsUnscored[i].Description.Should().Be(resultsList[i].Description);
                jobListingsUnscored[i].ApplicationLinks.Should().BeEquivalentTo(resultsList[i].ApplicationLinks);
                jobListingsUnscored[i].ApplicationLinksString.Should().Be(resultsList[i].ApplicationLinksString);
                jobListingsUnscored[i].IsAppliedTo.Should().Be(resultsList[i].IsAppliedTo);
                jobListingsUnscored[i].IsInterviewing.Should().Be(resultsList[i].IsInterviewing);
                jobListingsUnscored[i].IsRejected.Should().Be(resultsList[i].IsRejected);
                jobListingsUnscored[i].IsFavourite.Should().Be(resultsList[i].IsFavourite);
                jobListingsUnscored[i].IsHidden.Should().Be(resultsList[i].IsHidden);
                jobListingsUnscored[i].Notes.Should().Be(resultsList[i].Notes);
            }

        }
    }
}
