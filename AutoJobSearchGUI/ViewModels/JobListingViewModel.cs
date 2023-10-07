﻿using AutoJobSearchGUI.Models;
using AutoJobSearchShared;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class JobListingViewModel : ViewModelBase
    {
        [ObservableProperty]
        private JobListingModel _jobListing;

        public JobListingViewModel()
        {
            JobListing = new JobListingModel();
        }

        public void ChangeListing(JobListingModel jobListing)
        {
            if (jobListing.Id == JobListing.Id) return;

            var applicationLinks = SQLiteDb.GetApplicationLinksById(jobListing.Id).Result;
            var notes = SQLiteDb.GetNotesById(jobListing.Id).Result;

            jobListing.ApplicationLinks = applicationLinks;
            jobListing.Notes = notes;

            JobListing = jobListing;
        }
    }
}
