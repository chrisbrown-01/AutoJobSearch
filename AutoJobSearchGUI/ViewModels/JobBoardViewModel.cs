using AutoJobSearchGUI.Models;
using AutoJobSearchShared;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class JobBoardViewModel : ViewModelBase
    {
        public delegate void OpenJobListingViewHandler(JobListingModel job);
        public event OpenJobListingViewHandler OpenJobListingViewRequest;

        public void TestClick()
        {
            Debug.WriteLine("test click");

            if (SelectedJobListing != null)
            {
                Debug.WriteLine("Selected id: " + SelectedJobListing.Id);

                OpenJobListingViewRequest?.Invoke(SelectedJobListing);
            }
        }

        public List<JobListingModel> JobListings { get; } // TODO: change to MVVM tookit observable

        [ObservableProperty]
        private JobListingModel? _selectedJobListing;

        public JobBoardViewModel()
        {
            TestClickCommand = new RelayCommand(TestClick);

            JobListings = new();
            var jobs = SQLiteDb.GetAllJobListings().Result.Take(25);

            foreach (var job in jobs)
            {
                var jobListing = new JobListingModel
                {
                    Id = job.Id,
                    SearchTerm = job.SearchTerm,
                    CreatedAt = job.CreatedAt,
                    Description = job.Description,
                    Score = job.Score,
                    IsAppliedTo = job.IsAppliedTo,
                    IsInterviewing = job.IsInterviewing,
                    IsRejected = job.IsRejected,
                    IsFavourite = job.IsFavourite
                };

                jobListing.PropertyChanged += JobListingMVVM_PropertyChanged; // TODO: unsubscibe somewhere or delete

                JobListings.Add(jobListing);
            }
        }

        private void JobListingMVVM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var jobBoardDataGridItem = (JobListingModel)sender!;
            Debug.WriteLine($"Property {e.PropertyName} of job listing {jobBoardDataGridItem.Id} has changed.");
        }

        public RelayCommand TestClickCommand { get; }

        [ObservableProperty]
        private List<string> _testStrings = new List<string>()
        {
            "test1",
            "test2"
        };


        // TODO: SQLite concurrency disabling?, database relative pathing best practices + keep all relative paths within shared folder?
        // TODO: Menu or tab controls and seperate views for modifiying specific row item, save changes to db features

        // TODO: SeleniumTesting.Execute();
    }
}
