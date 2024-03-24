using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.ViewModels;
using FluentAssertions;
using NSubstitute;

namespace AutoJobSearchGUI.Tests.ViewModels
{
    public class MainWindowViewModel_Tests
    {
        private readonly IDbContext _dbContext;
        private readonly IFixture _fixture;
        private readonly MainWindowViewModel _viewModel;

        public MainWindowViewModel_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _viewModel = new MainWindowViewModel();
            _dbContext = Substitute.For<IDbContext>();
        }

        [Fact]
        public void ChangeViewToAddContact_ContactModelArgument_NotNull_SwitchesToCorrectViewModel()
        {
            // Arrange
            var contact = _fixture.Create<ContactModel>();

            // Act
            _viewModel.ChangeViewToAddContact(contact);

            // Assert
            _viewModel.ContentViewModel.Should().BeOfType<AddContactViewModel>();
        }

        [Fact]
        public void ChangeViewToAddContact_ContactModelArgument_Null_SwitchesToCorrectViewModel()
        {
            // Act
            _viewModel.ChangeViewToAddContact(null);

            // Assert
            _viewModel.ContentViewModel.Should().BeOfType<AddContactViewModel>();
        }

        [Fact]
        public void ChangeViewToContact_SwitchesToCorrectViewModel()
        {
            // Arrange
            Singletons.Contacts = _fixture.CreateMany<ContactModel>().ToList();

            // Act
            _viewModel.ChangeViewToContact(Singletons.Contacts.First().Id);

            // Assert
            _viewModel.ContentViewModel.Should().BeOfType<AddContactViewModel>();
        }

        //    // Assert
        //    _viewModel.ContentViewModel.Should().BeOfType<AddContactViewModel>();
        //}
        [Fact]
        public void ChangeViewToContacts_SwitchesToCorrectViewModel()
        {
            // Act
            _viewModel.ChangeViewToContacts();

            // Assert
            _viewModel.ContentViewModel.Should().BeOfType<ContactsViewModel>();
        }

        [Fact]
        public void ChangeViewToHelp_SwitchesToCorrectViewModel()
        {
            // Act
            _viewModel.ChangeViewToHelp();

            // Assert
            _viewModel.ContentViewModel.Should().BeOfType<HelpViewModel>();
        }

        [Fact]
        public void ChangeViewToJobBoard_SwitchesToCorrectViewModel()
        {
            // Act
            _viewModel.ChangeViewToJobBoard();

            // Assert
            _viewModel.ContentViewModel.Should().BeOfType<JobBoardViewModel>();
        }

        [Fact]
        public void ChangeViewToJobListing_IntArgument_SwitchesToCorrectViewModel()
        {
            // Arrange
            Singletons.JobListings = new List<JobListingModel>()
            {
                new JobListingModel() { Id = 1, DetailsPopulated = true },
                new JobListingModel() { Id = 2, DetailsPopulated = true },
                new JobListingModel() { Id = 3, DetailsPopulated = true }
            };

            // Act
            _viewModel.ChangeViewToJobListing(Singletons.JobListings.First().Id);

            // Assert
            _viewModel.ContentViewModel.Should().BeOfType<JobListingViewModel>();
        }

        [Fact]
        public void ChangeViewToJobListing_JobListingModelArgument_SwitchesToCorrectViewModel()
        {
            // Arrange
            var listing = _fixture.Create<JobListingModel>();

            // Act
            //_viewModel.ChangeViewToJobListing(Arg.Any<JobListingModel>(), Arg.Any<IEnumerable<JobListingModel>>());
            _viewModel.ChangeViewToJobListing(listing);

            // Assert
            _viewModel.ContentViewModel.Should().BeOfType<JobListingViewModel>();
        }

        [Fact]
        public void ChangeViewToJobSearch_SwitchesToCorrectViewModel()
        {
            // Act
            _viewModel.ChangeViewToJobSearch();

            // Assert
            _viewModel.ContentViewModel.Should().BeOfType<JobSearchViewModel>();
        }

        // Cannot complete test since IDbContext is not using the mock.
        //[Fact]
        //public void ChangeViewToAddContact_IntArgument_SwitchesToCorrectViewModel()
        //{
        //    // Arrange
        //    var contactAssociatedJobId = _fixture.Create<ContactAssociatedJobId>();
        //    _dbContext.CreateContactAssociatedJobIdAsync(Arg.Any<int>(), Arg.Any<int>()).ReturnsForAnyArgs(contactAssociatedJobId);

        //    var contact = _fixture.Create<Contact>();
        //    _dbContext.CreateContactAsync(Arg.Any<Contact>()).ReturnsForAnyArgs(contact);

        //    Singletons.Contacts = _fixture.CreateMany<ContactModel>().ToList();

        //    var jobId = _fixture.Create<int>();

        //    // Act
        //    _viewModel.ChangeViewToAddContact(jobId);
    }
}