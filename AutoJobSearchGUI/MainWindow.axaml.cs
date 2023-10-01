using AutoJobSearchConsoleApp.Models;
using AutoJobSearchGUI.ViewModels;
using AutoJobSearchShared;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AutoJobSearchGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var JobListings = SQLiteDb.GetAllJobListings().Result.Take(10);

            foreach (var job in JobListings)
            {
                var jobListingMVVM = new JobListingMVVM
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

                jobListingMVVM.PropertyChanged += JobListingMVVM_PropertyChanged;

                JobListingsMVVM.Add(jobListingMVVM);
            }

            dataGrid1.ItemsSource = JobListingsMVVM;
        }


        public List<JobListingMVVM> JobListingsMVVM { get; set; } = new();

        private void JobListingMVVM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var jobListingMVVM = (JobListingMVVM)sender!;
            Debug.WriteLine($"Property {e.PropertyName} of job listing {jobListingMVVM.Id} has changed.");
        }

        /*
        public List<TestModel> TestData = new()
        {
            new TestModel
            {
                Id = 1,
                Description = "desc1",
                IsTrue = true
            },
            new TestModel
            {
                Id = 2,
                Description = "desc2",
                IsTrue = true
            },
            new TestModel
            {
                Id = 3,
                Description = "desc3",
                IsTrue = false
            }
        };
        */

        public void ButtonClicked(object source, RoutedEventArgs args)
        {
            Debug.WriteLine("Click!");
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
        }
    }
}