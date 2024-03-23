using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.ViewModels;
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

        // Arrange

        // Act

        // Assert
    }
}
