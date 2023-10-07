using AutoJobSearchGUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class JobListingViewModel : ViewModelBase
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private JobListingModel _jobListing;

        public JobListingViewModel()
        {
            JobListing = new JobListingModel();
        }

        public void ChangeListing(JobListingModel jobListing)
        {
            JobListing = jobListing;
        }
    }
}
