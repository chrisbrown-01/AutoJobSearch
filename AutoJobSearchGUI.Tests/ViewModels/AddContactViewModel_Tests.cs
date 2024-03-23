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
            foreach (var jobListing in Singletons.JobListings)
            {
                jobListing.Id = 0;
            }

            int jobId = 0;
            while (_viewModel.Contact.JobListingIds.Contains(jobId))
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
            int? jobId = _fixture.Create<int>();

            Singletons.Contacts = _singletonContacts;
            Singletons.JobListings = _singletonJobListings;

            var initialContactsCount = Singletons.Contacts.Count;

            var associatedJobIdRecord = new ContactAssociatedJobId();
            _dbContext.CreateContactAssociatedJobIdAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(associatedJobIdRecord);

            var contact = new Contact();
            _dbContext.CreateContactAsync(Arg.Any<Contact>()).Returns(contact);

           // Act
           await _viewModel.CreateNewContactCommand.ExecuteAsync(jobId);

            // Assert
            var newCount = Singletons.Contacts.Count;
            newCount.Should().Be(initialContactsCount + 1);
            await _dbContext.Received().CreateContactAssociatedJobIdAsync(Arg.Any<int>(), Arg.Any<int>());
            await _dbContext.Received().CreateContactAsync(Arg.Any<Contact>());
        }

        [Fact]
        public async Task CreateNewContactAsync_nullIntArgument_CorrectlyUpdatesProperties()
        {
            // Arrange
            int? jobId = null;

            Singletons.Contacts = _singletonContacts;
            Singletons.JobListings = _singletonJobListings;

            var initialContactsCount = Singletons.Contacts.Count;

            var associatedJobIdRecord = new ContactAssociatedJobId();
            _dbContext.CreateContactAssociatedJobIdAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(associatedJobIdRecord);

            var contact = new Contact();
            _dbContext.CreateContactAsync(Arg.Any<Contact>()).Returns(contact);

            // Act
            await _viewModel.CreateNewContactCommand.ExecuteAsync(jobId);

            // Assert
            var newCount = Singletons.Contacts.Count;
            newCount.Should().Be(initialContactsCount + 1);
            await _dbContext.DidNotReceive().CreateContactAssociatedJobIdAsync(Arg.Any<int>(), Arg.Any<int>());
            await _dbContext.Received().CreateContactAsync(Arg.Any<Contact>());
        }

        [Fact]
        public async Task DeleteContactAssociatedJobIdAsync_ValidInputArgument_CorrectlyUpdatesProperties()
        {
            // Arrange
            Singletons.Contacts = _singletonContacts;

            var contact = _fixture.Create<ContactModel>();
            _viewModel.Contact = contact;

            var initialJobId = contact.JobListingIds.First();

            // Act
            await _viewModel.DeleteContactAssociatedJobIdCommand.ExecuteAsync(initialJobId.ToString());

            // Assert
            _viewModel.Contact.JobListingIds.Should().NotContain(initialJobId);
            await _dbContext.Received().DeleteContactAssociatedJobIdAsync(Arg.Any<int>(), Arg.Any<int>());
        }

        [Fact]
        public async Task DeleteContactAssociatedJobIdAsync_InvalidInputArgument_DoesNotUpdateProperties()
        {
            // Arrange
            Singletons.Contacts = _singletonContacts;

            var contact = _fixture.Create<ContactModel>();
            _viewModel.Contact = contact;

            var initialJobId = contact.JobListingIds.First();

            // Act
            await _viewModel.DeleteContactAssociatedJobIdCommand.ExecuteAsync("999999");

            // Assert
            _viewModel.Contact.JobListingIds.Should().Contain(initialJobId);
            await _dbContext.DidNotReceive().DeleteContactAssociatedJobIdAsync(Arg.Any<int>(), Arg.Any<int>());
        }

        [Fact]
        public void GoToPreviousContact_GoesToPreviousContact_WhenValid()
        {
            // Arrange
            Singletons.Contacts = _singletonContacts;
            _viewModel.Contact = Singletons.Contacts.Last();
            var initialJobListing = _viewModel.Contact;

            // Act
            _viewModel.GoToPreviousContactCommand.Execute(null);

            // Assert
            _viewModel.Contact.Should().NotBe(initialJobListing);
        }

        [Fact]
        public void GoToPreviousContact_DoesNotGoToPreviousContact_WhenInvalid()
        {
            // Arrange
            Singletons.Contacts = _singletonContacts;
            _viewModel.Contact = Singletons.Contacts.First();
            var initialJobListing = _viewModel.Contact;

            // Act
            _viewModel.GoToPreviousContactCommand.Execute(null);

            // Assert
            _viewModel.Contact.Should().Be(initialJobListing);
        }

        [Fact]
        public void GoToNextContact_GoesToNextContact_WhenValid()
        {
            // Arrange
            Singletons.Contacts = _singletonContacts;
            _viewModel.Contact = Singletons.Contacts.First();
            var initialJobListing = _viewModel.Contact;

            // Act
            _viewModel.GoToNextContactCommand.Execute(null);

            // Assert
            _viewModel.Contact.Should().NotBe(initialJobListing);
        }

        [Fact]
        public void GoToNextContact_DoesNotGoToNextContact_WhenInvalid()
        {
            // Arrange
            Singletons.Contacts = _singletonContacts;
            _viewModel.Contact = Singletons.Contacts.Last();
            var initialJobListing = _viewModel.Contact;

            // Act
            _viewModel.GoToNextContactCommand.Execute(null);

            // Assert
            _viewModel.Contact.Should().Be(initialJobListing);
        }
    }
}
