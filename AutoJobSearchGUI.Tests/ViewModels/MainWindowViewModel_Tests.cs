using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.ViewModels;
using FluentAssertions;

namespace AutoJobSearchGUI.Tests.ViewModels
{
    public class MainWindowViewModel_Tests
    {
        private readonly IFixture _fixture;
        private readonly MainWindowViewModel _viewModel;

        public MainWindowViewModel_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _viewModel = new MainWindowViewModel();
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
        public void ChangeViewToJobListing_SwitchesToCorrectViewModel()
        {
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
    }
}