using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private JobBoardViewModel jobBoardViewModel;
        private JobSearchViewModel jobSearchViewModel;
        private JobListingViewModel jobListingViewModel;
        private DbContext dbContext;

        [ObservableProperty]
        private ViewModelBase _contentViewModel;

        public MainWindowViewModel()
        {
            dbContext = new DbContext();
            jobBoardViewModel = new JobBoardViewModel(dbContext);
            jobSearchViewModel = new JobSearchViewModel();
            jobListingViewModel = new JobListingViewModel(dbContext);
            ContentViewModel = jobBoardViewModel;

            jobBoardViewModel.OpenJobListingViewRequest += ChangeViewToJobListing;
        }

        public void ChangeViewToJobBoard()
        {
            ContentViewModel = jobBoardViewModel;
        }

        public void ChangeViewToJobSearch()
        {
            ContentViewModel = jobSearchViewModel;
        }

        public async Task ChangeViewToJobListing(JobListingModel jobListing, IEnumerable<JobListingModel> jobListings)
        {
            jobListingViewModel.PopulateJobListings(jobListings);
            await jobListingViewModel.OpenJobListing(jobListing);
            ContentViewModel = jobListingViewModel;
        }
    }
}