using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        private readonly DbContext dbContext;

        [ObservableProperty]
        private ViewModelBase _contentViewModel;

        private readonly AddContactViewModel addContactViewModel;
        private readonly ContactsViewModel contactsViewModel;
        private readonly HelpViewModel helpViewModel;
        private readonly JobBoardViewModel jobBoardViewModel;
        private readonly JobListingViewModel jobListingViewModel;
        private readonly JobSearchViewModel jobSearchViewModel;

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

#pragma warning disable CA1822 // Mark members as static

        // Cannot be static - it will throw an exception
        public void ToggleLightDarkMode()
#pragma warning restore CA1822 // Mark members as static
        {
            try
            {
                if (App.Current!.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Light)
                {
                    App.Current!.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Dark;
                }
                else
                {
                    App.Current!.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Light;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception caught when toggling light/dark mode: {@Exception}", ex);
            }
        }

        public void ChangeViewToAddContact(ContactModel? contact)
        {
            if (contact is not null)
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

        public void ChangeViewToContact(int contactId)
        {
            addContactViewModel.OpenContactCommand.Execute(Singletons.Contacts.Where(x => x.Id == contactId).Single());
            ContentViewModel = addContactViewModel;
        }

        public void ChangeViewToContacts()
        {
            ContentViewModel = contactsViewModel;
        }

        public void ChangeViewToHelp()
        {
            ContentViewModel = helpViewModel;
        }

        public void ChangeViewToJobBoard()
        {
            UpdateJobBoard();
            ContentViewModel = jobBoardViewModel;
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

        public void ChangeViewToJobSearch()
        {
            ContentViewModel = jobSearchViewModel;
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

        public void UpdateJobBoard()
        {
            jobBoardViewModel.UpdateJobBoard();
        }

        private static void ConfigureSerilog()
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

        private void InitializeSingletons()
        {
            Singletons.JobListings = GetAllJobListingsAsync().Result;
            Singletons.Contacts = GetAllContactsAsync().Result;
        }

        private void SubscribeToEvents()
        {
            jobListingViewModel.CreateNewContactWithAssociatedJobIdRequest += ChangeViewToAddContact;
            contactsViewModel.OpenAddContactViewRequest += ChangeViewToAddContact;

            jobBoardViewModel.OpenJobListingViewRequest += ChangeViewToJobListing;

            jobListingViewModel.OpenJobBoardViewRequest += ChangeViewToJobBoard;

            jobListingViewModel.ChangeViewToContactRequest += ChangeViewToContact;

            jobListingViewModel.UpdateJobBoardViewRequest += UpdateJobBoard;

            addContactViewModel.OpenContactsViewRequest += ChangeViewToContacts;

            addContactViewModel.OpenJobListingViewRequest += ChangeViewToJobListing;

            addContactViewModel.UpdateContactsViewRequest += UpdateContacts;
        }

        private void UnsubscribeFromEvents()
        {
            jobListingViewModel.CreateNewContactWithAssociatedJobIdRequest -= ChangeViewToAddContact;
            contactsViewModel.OpenAddContactViewRequest -= ChangeViewToAddContact;

            jobBoardViewModel.OpenJobListingViewRequest -= ChangeViewToJobListing;

            jobListingViewModel.OpenJobBoardViewRequest -= ChangeViewToJobBoard;

            jobListingViewModel.ChangeViewToContactRequest -= ChangeViewToContact;

            jobListingViewModel.UpdateJobBoardViewRequest -= UpdateJobBoard;

            addContactViewModel.OpenContactsViewRequest -= ChangeViewToContacts;

            addContactViewModel.UpdateContactsViewRequest -= UpdateContacts;
        }
    }
}