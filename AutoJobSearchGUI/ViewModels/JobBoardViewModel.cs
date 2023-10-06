using AutoJobSearchGUI.Models;
using AutoJobSearchShared;
using Avalonia.Interactivity;
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
        public List<JobBoardDataGridItem> JobListings { get; }

        public JobBoardViewModel()
        {
            JobListings = new();
            var jobs = SQLiteDb.GetAllJobListings().Result.Take(25);

            foreach (var job in jobs)
            {
                var jobListing = new JobBoardDataGridItem
                {
                    Id = job.Id,
                    SearchTerm = job.SearchTerm,
                    CreatedAt = job.CreatedAt,
                    Description = job.Description,
                    Score = job.Score,
                    IsAppliedTo = job.IsAppliedTo,
                    IsInterviewing = job.IsInterviewing,
                    IsRejected = job.IsRejected,
                };

                jobListing.PropertyChanged += JobListingMVVM_PropertyChanged;

                JobListings.Add(jobListing);
            }
        }

        private void JobListingMVVM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var jobBoardDataGridItem = (JobBoardDataGridItem)sender!;
            Debug.WriteLine($"Property {e.PropertyName} of job listing {jobBoardDataGridItem.Id} has changed.");
        }

        //public void ButtonClicked(object source, RoutedEventArgs args)
        //{
        //Debug.WriteLine("Click!");
        //txt1.Text = "Clicked";
        //listBox1.ItemsSource = new List<string>
        //{
        //    "test1",
        //    "test2",
        //    "test3"
        //};

        // dataGrid1.ItemsSource = SQLiteDb.GetAllJobListings().Result.Take(10);

        // TODO:
        // SQLite concurrency disabling?, database relative pathing best practices + keep all relative paths within shared folder?
        // Menu or tab controls and seperate views for modifiying specific row item, save changes to db features

        // SeleniumTesting.Execute();
        //}
    }
}
