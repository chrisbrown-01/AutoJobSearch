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
    public class ContactsViewModel_Tests
    {
        private readonly IFixture _fixture;
        private readonly ContactsViewModel _viewModel;
        private readonly IDbContext _dbContext;
        private readonly List<JobListingModel> _singletonJobListings;
        private readonly List<ContactModel> _singletonContacts;

        public ContactsViewModel_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _dbContext = Substitute.For<IDbContext>();
            _viewModel = new ContactsViewModel(_dbContext);
            _singletonJobListings = _fixture.CreateMany<JobListingModel>().ToList();
            _singletonContacts = _fixture.CreateMany<ContactModel>().ToList();
        }

        [Fact]
        public async Task ExecuteQuery_UpdatesPropertiesCorrectly()
        {
            // Arrange
            var pageSize = _viewModel.PageSize;
            var contacts = _fixture.CreateMany<Contact>(10000);
            var contactsAssociatedJobIds = _fixture.CreateMany<ContactAssociatedJobId>(1000);

            _dbContext.GetAllContactsAsync().Returns(contacts);
            _dbContext.GetAllContactsAssociatedJobIdsAsync().Returns(contactsAssociatedJobIds);

            _viewModel.ContactsQueryModel = new ContactsQueryModel();

            // Act
            await _viewModel.ExecuteQueryCommand.ExecuteAsync(null);

            // Assert
            await _dbContext.Received().GetAllContactsAsync();
            await _dbContext.Received().GetAllContactsAssociatedJobIdsAsync();

            _viewModel.PageIndex.Should().Be(0);
            _viewModel.ContactsDisplayed.Should().NotBeNull();
            _viewModel.ContactsDisplayed.Should().BeOfType<List<ContactModel>>();
            _viewModel.ContactsDisplayed.Count.Should().BeLessThanOrEqualTo(_viewModel.PageSize);

            if (_viewModel.ContactsDisplayed.Any())
            {
                _viewModel.ContactsDisplayed.Should().AllSatisfy(x => x.EnableEvents.Should().BeFalse());
            }
        }

        // Arrange

        // Act

        // Assert
    }
}
