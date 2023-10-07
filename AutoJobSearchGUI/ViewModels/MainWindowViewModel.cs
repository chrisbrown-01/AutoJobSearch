﻿using AutoJobSearchGUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private JobBoardViewModel jobBoardViewModel;
        private JobSearchViewModel jobSearchViewModel;
        private JobListingViewModel jobListingViewModel;

        [ObservableProperty]
        private ViewModelBase _contentViewModel;

        public MainWindowViewModel()
        {
            jobBoardViewModel = new JobBoardViewModel();
            jobSearchViewModel = new JobSearchViewModel();
            jobListingViewModel = new JobListingViewModel();
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

        public void ChangeViewToJobListing(JobListingModel jobListing)
        {
            jobListingViewModel.ChangeListing(jobListing);
            ContentViewModel = jobListingViewModel;
        }
    }
}