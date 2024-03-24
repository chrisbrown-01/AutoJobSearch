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
        public void UpdateContacts_SetsCorrectNumberOfRecords()
        {
            // Arrange
            var pageSize = _viewModel.PageSize;
            var pageIndex = _viewModel.PageIndex;
            Singletons.Contacts = _singletonContacts;

            // Act
            _viewModel.UpdateContacts();

            // Assert
            _viewModel.ContactsDisplayed.Count.Should().BeLessThanOrEqualTo(_viewModel.PageSize);
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

        [Fact]
        public void OpenContact_InvokesEvent()
        {
            // Arrange
            ContactModel? contactModel = _fixture.Create<ContactModel>();
            _viewModel.SelectedContact = contactModel;

            bool wasCalled = false;
            _viewModel.OpenAddContactViewRequest += (contactModel) => wasCalled = true;

            // Act
            _viewModel.OpenContactCommand.Execute(null);

            // Assert
            wasCalled.Should().BeTrue();
        }

        [Fact]
        public void AddNewContact_InvokesEvent()
        {
            // Arrange
            ContactModel? contactModel = _fixture.Create<ContactModel>();

            bool wasCalled = false;
            _viewModel.OpenAddContactViewRequest += (contactModel) => wasCalled = true;

            // Act
            _viewModel.AddNewContactCommand.Execute(null);

            // Assert
            wasCalled.Should().BeTrue();
        }

        [Fact]
        public void GoToNextPage_NoMorePages_DoesNotChangePageIndexOrJobListingsDisplayed()
        {
            // Arrange
            var initialPageIndex = _fixture.Create<int>();
            var initialContactsDisplayed = _fixture.CreateMany<ContactModel>().ToList();
            _viewModel.PageIndex = initialPageIndex;
            _viewModel.PageSize = 0;
            _viewModel.ContactsDisplayed = initialContactsDisplayed;

            // Act
            _viewModel.GoToNextPageCommand.Execute(null);

            // Assert
            _viewModel.PageIndex.Should().Be(initialPageIndex);
            _viewModel.ContactsDisplayed.Should().BeEquivalentTo(initialContactsDisplayed);
        }

        [Fact]
        public void GoToNextPage_HasMorePages_UpdatesPageIndexAndJobListingsDisplayed()
        {
            // Arrange
            var initialPageIndex = 0;
            _viewModel.PageIndex = initialPageIndex;

            _viewModel.PageSize = 50;
            var contacts = _fixture.CreateMany<ContactModel>(_viewModel.PageSize * 2).ToList();
            Singletons.Contacts = contacts;
            var initialContactsDisplayed = contacts.Take(_viewModel.PageSize).ToList();
            _viewModel.ContactsDisplayed = initialContactsDisplayed;

            // Act
            _viewModel.GoToNextPageCommand.Execute(null);

            // Assert
            _viewModel.PageIndex.Should().Be(initialPageIndex + 1);
            _viewModel.ContactsDisplayed.Should().NotBeEquivalentTo(initialContactsDisplayed);
        }

        [Fact]
        public void GoToPreviousPage_AtFirstPage_DoesNotChangePageIndexOrJobListingsDisplayed()
        {
            // Arrange
            var initialPageIndex = 0;
            var initialContactsDisplayed = _fixture.CreateMany<ContactModel>().ToList();
            _viewModel.PageIndex = initialPageIndex;
            _viewModel.PageSize = 50;
            _viewModel.ContactsDisplayed = initialContactsDisplayed;

            // Act
            _viewModel.GoToPreviousPageCommand.Execute(null);

            // Assert
            _viewModel.PageIndex.Should().Be(initialPageIndex);
            _viewModel.ContactsDisplayed.Should().BeEquivalentTo(initialContactsDisplayed);
        }

        [Fact]
        public void GoToPreviousPage_NotAtFirstPage_UpdatesPageIndexAndJobListingsDisplayed()
        {
            // Arrange
            var initialPageIndex = 10; // any positive integer
            var initialContactsDisplayed = _fixture.CreateMany<ContactModel>().ToList();
            _viewModel.PageIndex = initialPageIndex;
            _viewModel.PageSize = 1;
            _viewModel.ContactsDisplayed = initialContactsDisplayed;

            // Act
            _viewModel.GoToPreviousPageCommand.Execute(null);

            // Assert
            _viewModel.PageIndex.Should().Be(initialPageIndex - 1);
            _viewModel.ContactsDisplayed.Should().NotBeEquivalentTo(initialContactsDisplayed);
        }

        [Fact]
        public async Task RenderDefaultContactsViewAsync_CorrectlyUpdatesProperties()
        {
            // Arrange
            _dbContext.GetAllContactsAsync().Returns(_fixture.CreateMany<Contact>());
            _dbContext.GetAllContactsAssociatedJobIdsAsync().Returns(_fixture.CreateMany<ContactAssociatedJobId>());

            // Act
            await _viewModel.RenderDefaultContactsViewCommand.ExecuteAsync(null);

            // Assert
            _viewModel.PageIndex.Should().Be(0);
            _viewModel.ContactsQueryModel.Should().BeEquivalentTo(new ContactsQueryModel());
            _viewModel.ContactsDisplayed.Count.Should().BeLessThanOrEqualTo(_viewModel.PageSize);
        }
    }
}
