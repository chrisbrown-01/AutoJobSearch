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
        public string Greeting => "listing";

        [ObservableProperty]
        private int _id;

        public JobListingViewModel()
        {
        }

        public void ChangeListing(int id)
        {
            Id = id;
        }

        //[ObservableProperty]
        //private JobListingModel _jobListingModel;

        //public JobListingViewModel(JobListingModel jobListing)
        //{
        //    var id = jobListing.Id;
        //    jobListing.ApplicationLinks.Add("link1");
        //    jobListing.ApplicationLinks.Add("link2");

        //    JobListingModel = jobListing;
        //}
    }
}
