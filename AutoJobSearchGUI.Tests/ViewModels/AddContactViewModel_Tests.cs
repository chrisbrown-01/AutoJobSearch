using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.ViewModels;
using AutoJobSearchShared.Models;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Tests.ViewModels
{
    public class AddContactViewModel_Tests
    {
        private readonly IFixture _fixture;
        private readonly AddContactViewModel _viewModel;
        private readonly IDbContext _dbContext;
        private readonly List<JobListingModel> _singletonJobListings;
        private readonly List<ContactModel> _singletonContacts;

        public AddContactViewModel_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _dbContext = Substitute.For<IDbContext>();
            _viewModel = new AddContactViewModel(_dbContext);
            _singletonJobListings = _fixture.CreateMany<JobListingModel>().ToList();
            _singletonContacts = _fixture.CreateMany<ContactModel>().ToList();
        }

        [Fact]
        public async Task CreateContactAssociatedJobIdAsync_CorrectlyUpdatesProperties()
        {
            // Arrange
            _viewModel.Contact = _fixture.Create<ContactModel>();
            var initialCount = _viewModel.Contact.JobListingIds.Count;

            Singletons.Contacts = _singletonContacts;

            Singletons.JobListings = _singletonJobListings;
            foreach(var jobListing in Singletons.JobListings)
            {
                jobListing.Id = 0;
            }

            int jobId = 0;
            while(_viewModel.Contact.JobListingIds.Contains(jobId))
            {
                jobId++;
            }

            var jobListings = _fixture.CreateMany<JobListing>();
            _dbContext.GetAllJobListingsAsync().Returns(jobListings);

            var associatedJobIdRecord = _fixture.Create<ContactAssociatedJobId>();
            _dbContext.CreateContactAssociatedJobIdAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(associatedJobIdRecord);

            // Act
            await _viewModel.CreateContactAssociatedJobIdCommand.ExecuteAsync(jobId.ToString());

            // Assert
            await _dbContext.Received().GetAllJobListingsAsync();
            await _dbContext.Received().CreateContactAssociatedJobIdAsync(Arg.Any<int>(), Arg.Any<int>());
            _viewModel.Contact.JobListingIds.Should().NotBeNullOrEmpty();
            _viewModel.Contact.JobListingIds.Count.Should().Be(initialCount + 1);
            _viewModel.Contact.JobListingIds.Should().Contain(associatedJobIdRecord.JobListingId);
        }

        [Fact]
        public async Task CreateNewContactAsync_NonNullIntArgument_CorrectlyUpdatesProperties()
        {
            // Arrange
            var jobId = _fixture.Create<int>();

            Singletons.Contacts = _singletonContacts;
            Singletons.JobListings = _singletonJobListings;

            var associatedJobIdRecord = new ContactAssociatedJobId();
            _dbContext.CreateContactAssociatedJobIdAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(associatedJobIdRecord);

            // Act

            // Assert
        }


        // Arrange

        // Act

        // Assert
    }
