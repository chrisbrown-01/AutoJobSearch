using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared.EventAggregator;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        private JobBoardViewModel jobBoardViewModel;
        private JobSearchViewModel jobSearchViewModel;
        private JobListingViewModel jobListingViewModel;
        private DbContext dbContext; // TODO: convert to readonly
        private readonly EventAggregator eventAggregator;

        [ObservableProperty]
        private ViewModelBase _contentViewModel;

        private void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console(new JsonFormatter())
                    .WriteTo.File(new JsonFormatter(), "AutoJobSearchGuiLogFile.json", rollingInterval: RollingInterval.Month)
                    .CreateLogger();

            // Attach event handler for unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                var exception = (Exception)eventArgs.ExceptionObject;
                Log.Fatal(exception, "An unhandled exception occurred.");
            };
        }

        public MainWindowViewModel()
        {
            ConfigureSerilog();

            eventAggregator = new EventAggregator();
            dbContext = new DbContext();
            jobBoardViewModel = new JobBoardViewModel(dbContext);
            jobSearchViewModel = new JobSearchViewModel(dbContext, eventAggregator);
            jobListingViewModel = new JobListingViewModel(dbContext);
            ContentViewModel = jobBoardViewModel;

            jobBoardViewModel.OpenJobListingViewRequest += ChangeViewToJobListing;
        }

        public void ChangeViewToJobBoard() // TODO: convert all to Tasks or RelayCommands (if exception handling improves?)
        {
            ContentViewModel = jobBoardViewModel;
            //await Task.CompletedTask;
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