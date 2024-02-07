using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private HelpViewModel helpViewModel;
        private AddContactViewModel addContactViewModel;
        private ContactsViewModel contactsViewModel;
        private readonly DbContext dbContext;

        [ObservableProperty]
        private ViewModelBase _contentViewModel;

        public MainWindowViewModel()
        {
            ConfigureSerilog();

            dbContext = new DbContext();
            jobBoardViewModel = new JobBoardViewModel(dbContext);
            jobSearchViewModel = new JobSearchViewModel(dbContext);
            jobListingViewModel = new JobListingViewModel(dbContext);
            addContactViewModel = new AddContactViewModel(dbContext);
            contactsViewModel = new ContactsViewModel(dbContext);
            helpViewModel = new HelpViewModel();
            ContentViewModel = jobBoardViewModel;

            jobBoardViewModel.OpenJobListingViewRequest += ChangeViewToJobListing;
            contactsViewModel.OpenAddContactViewRequest += ChangeViewToAddContact;
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        public void ChangeViewToContacts()
        {
            ContentViewModel = contactsViewModel;
        }

        public void ChangeViewToAddContact(Contact? contact, IEnumerable<Contact> contacts)
        {
            addContactViewModel.PopulateContactsCommand.Execute(contacts);
            addContactViewModel.OpenContactCommand.Execute(contact);
            ContentViewModel = addContactViewModel;
        }

        public void ChangeViewToJobBoard()
        {
            ContentViewModel = jobBoardViewModel;
        }

        public void ChangeViewToHelp()
        {
            ContentViewModel = helpViewModel;
        }

        public void ChangeViewToJobSearch()
        {
            ContentViewModel = jobSearchViewModel;
        }

        public void ChangeViewToJobListing(JobListingModel jobListing, IEnumerable<JobListingModel> jobListings)
        {
            jobListingViewModel.PopulateJobListingsCommand.Execute(jobListings);
            jobListingViewModel.OpenJobListingCommand.ExecuteAsync(jobListing).Wait();
            ContentViewModel = jobListingViewModel;
        }

        private void ConfigureSerilog()
        {
            // At the time of creating this project, the logging provider for Avalonia is pretty crude and not well documented.
            // Therefore I am choosing to make this project dependent on Serilog and configuring the logger in the MainWindowViewModel
            // to ensure I am not interfering with any of Avalonia's startup procedures.

            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console(new JsonFormatter())
                    .WriteTo.File(new JsonFormatter(), "AutoJobSearchGuiLogFile.json", rollingInterval: RollingInterval.Month)
                    .CreateLogger();

            Log.Information("Starting GUI application.");
        }
    }
}