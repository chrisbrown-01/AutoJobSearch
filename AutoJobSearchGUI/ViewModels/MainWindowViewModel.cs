using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Helpers;
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

            InitializeSingletons();
          
            jobBoardViewModel = new JobBoardViewModel(dbContext);
            jobSearchViewModel = new JobSearchViewModel(dbContext);
            jobListingViewModel = new JobListingViewModel(dbContext);
            addContactViewModel = new AddContactViewModel(dbContext);
            contactsViewModel = new ContactsViewModel(dbContext);
            helpViewModel = new HelpViewModel();

            ContentViewModel = jobBoardViewModel;

            SubscribeToEvents();
        }

        private void InitializeSingletons()
        {
            Singletons.JobListings = GetAllJobListingsAsync().Result;
            Singletons.Contacts = GetAllContactsAsync().Result;
        }

        private async Task<List<ContactModel>> GetAllContactsAsync()
        {
            var contacts = await dbContext.GetAllContactsAsync();
            var contactsAssociatedJobIds = await dbContext.GetAllContactsAssociatedJobIdsAsync();
            return ContactsHelpers.ConvertContactsToContactModels(contacts, contactsAssociatedJobIds);
        }

        private async Task<List<JobListingModel>> GetAllJobListingsAsync()
        {
            var jobs = await dbContext.GetAllJobListingsAsync();
            return JobListingHelpers.ConvertJobListingsToJobListingModels(jobs);
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
            dbContext.Dispose();
        }

        public void UpdateContacts()
        {
            contactsViewModel.UpdateContacts();
        }

        public void ChangeViewToContacts()
        {
            ContentViewModel = contactsViewModel;
        }

        public void ChangeViewToAddContact(ContactModel? contact)
        {
            if(contact is not null)
            {
                addContactViewModel.OpenContactCommand.Execute(contact);
            }
            else
            {
                addContactViewModel.CreateNewContactCommand.Execute(null);
            }

            ContentViewModel = addContactViewModel;
        }

        public void ChangeViewToAddContact(int jobId)
        {
            addContactViewModel.CreateNewContactCommand.Execute(jobId);
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

        public void ChangeViewToJobListing(int jobListingId)
        {
            jobListingViewModel.OpenJobListingByIdCommand.ExecuteAsync(jobListingId).Wait();
            ContentViewModel = jobListingViewModel;
        }

        public void ChangeViewToJobListing(JobListingModel jobListing)
        {
            jobListingViewModel.OpenJobListingCommand.ExecuteAsync(jobListing).Wait();
            ContentViewModel = jobListingViewModel;
        }

        private void SubscribeToEvents()
        {
            jobListingViewModel.OpenAddContactViewWithAssociatedJobIdRequest += ChangeViewToAddContact;
            contactsViewModel.OpenAddContactViewRequest += ChangeViewToAddContact;

            jobBoardViewModel.OpenJobListingViewRequest += ChangeViewToJobListing;

            addContactViewModel.OpenContactsViewRequest += ChangeViewToContacts;

            addContactViewModel.OpenJobListingViewRequest += ChangeViewToJobListing;

            addContactViewModel.UpdateContactsViewRequest += UpdateContacts;
        }

        private void UnsubscribeFromEvents()
        {
            jobListingViewModel.OpenAddContactViewWithAssociatedJobIdRequest -= ChangeViewToAddContact;
            contactsViewModel.OpenAddContactViewRequest -= ChangeViewToAddContact;

            jobBoardViewModel.OpenJobListingViewRequest -= ChangeViewToJobListing;

            addContactViewModel.OpenContactsViewRequest -= ChangeViewToContacts;

            addContactViewModel.UpdateContactsViewRequest -= UpdateContacts;
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