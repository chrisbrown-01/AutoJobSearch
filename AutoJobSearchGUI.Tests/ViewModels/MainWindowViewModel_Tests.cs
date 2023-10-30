using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchGUI.ViewModels;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Tests.ViewModels
{
    public class MainWindowViewModel_Tests
    {
        private readonly MainWindowViewModel _viewModel;
        private readonly IFixture _fixture;

        public MainWindowViewModel_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _viewModel = new MainWindowViewModel();
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
        public void ChangeViewToHelp_SwitchesToCorrectViewModel()
        {
            // Act
            _viewModel.ChangeViewToHelp();

            // Assert
            _viewModel.ContentViewModel.Should().BeOfType<HelpViewModel>();
        }

        [Fact]
        public void ChangeViewToJobSearch_SwitchesToCorrectViewModel()
        {
            // Act
            _viewModel.ChangeViewToJobSearch();

            // Assert
            _viewModel.ContentViewModel.Should().BeOfType<JobSearchViewModel>();
        }

        [Fact]
        public void ChangeViewToJobListing_SwitchesToCorrectViewModel()
        {
            var listing = _fixture.Create<JobListingModel>();
            var listings = _fixture.CreateMany<JobListingModel>();

            // Act
            //_viewModel.ChangeViewToJobListing(Arg.Any<JobListingModel>(), Arg.Any<IEnumerable<JobListingModel>>());
            _viewModel.ChangeViewToJobListing(listing, listings);

            // Assert
            _viewModel.ContentViewModel.Should().BeOfType<JobListingViewModel>();
        }
    }
}
