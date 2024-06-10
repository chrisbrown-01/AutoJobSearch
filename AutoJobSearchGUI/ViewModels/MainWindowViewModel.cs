using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Enums;
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
        private readonly AddContactViewModel addContactViewModel;
        private readonly ContactsViewModel contactsViewModel;
        private readonly DbContext dbContext;
        private readonly HelpViewModel helpViewModel;
        private readonly JobBoardViewModel jobBoardViewModel;
        private readonly JobListingViewModel jobListingViewModel;
        private readonly JobSearchViewModel jobSearchViewModel;

        [ObservableProperty]
        private ViewModelBase _contentViewModel;
        private List<ViewStackModel> viewHistory;
        private int viewHistoryIndex;

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

            viewHistory = new List<ViewStackModel>();
            viewHistoryIndex = -1;

            UpdateViewHistory(new ViewStackModel(ViewModel.JobBoardViewModel), false);
            UpdateJobBoard();
            SubscribeToEvents();
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

            ChangedContactView(addContactViewModel.Contact.Id, false);
            ContentViewModel = addContactViewModel;
        }

        public void ChangeViewToAddContact(int jobId)
        {
            addContactViewModel.CreateNewContactCommand.Execute(jobId);
            ContentViewModel = addContactViewModel;

            ChangedContactView(addContactViewModel.Contact.Id, false);
        }

        // This method must be "void", since "async Task" does not properly throw or log exceptions
        public void ChangeViewToContact(int contactId, bool changedViaPreviousOrForwardButton)
        {
            var contactModel = Singletons.Contacts.SingleOrDefault(x => x.Id == contactId);

            // Edge case handling if user has query filters enabled which prevents the Contacts singleton from containing a contact that does exist
            if (contactModel == null)
            {
                var contact = dbContext.GetContactByIdAsync(contactId).GetAwaiter().GetResult();
                var contactsAssociatedJobIds = dbContext.GetAllContactsAssociatedJobIdsAsync().GetAwaiter().GetResult();
                var contactAssociatedJobIds = contactsAssociatedJobIds.Select(x => x.JobListingId);
                contactModel = ContactsHelpers.ConvertContactToContactModel(contact, contactAssociatedJobIds);
            }

            addContactViewModel.OpenContactCommand.Execute(contactModel);
            ContentViewModel = addContactViewModel;

            ChangedContactView(contactId, changedViaPreviousOrForwardButton);
        }

        public void ChangeViewToContacts()
        {
            if (ContentViewModel == contactsViewModel)
                return;

            ContentViewModel = contactsViewModel;

            UpdateViewHistory(new ViewStackModel(ViewModel.ContactsViewModel), false);
        }

        public void ChangeViewToHelp()
        {
            if (ContentViewModel == helpViewModel)
                return;

            ContentViewModel = helpViewModel;

            UpdateViewHistory(new ViewStackModel(ViewModel.HelpViewModel), false);
        }

        public void ChangeViewToJobBoard()
        {
            if (ContentViewModel == jobBoardViewModel)
                return;

            UpdateJobBoard();
            ContentViewModel = jobBoardViewModel;

            UpdateViewHistory(new ViewStackModel(ViewModel.JobBoardViewModel), false);
        }

        // This method must be "void", since "async Task" does not properly throw or log exceptions
        public void ChangeViewToJobListing(int jobListingId, bool changedViaPreviousOrForwardButton)
        {
            jobListingViewModel.OpenJobListingByIdCommand.ExecuteAsync(jobListingId).GetAwaiter().GetResult();
            ContentViewModel = jobListingViewModel;

            ChangedJobListingView(jobListingId, changedViaPreviousOrForwardButton);
        }

        // This method must be "void", since "async Task" does not properly throw or log exceptions
        public void ChangeViewToJobListing(JobListingModel jobListing)
        {
            jobListingViewModel.OpenJobListingCommand.ExecuteAsync(jobListing).GetAwaiter().GetResult(); 
            ContentViewModel = jobListingViewModel;

            ChangedJobListingView(jobListing.Id, false);
        }

        public void ChangeViewToJobSearch()
        {
            if (ContentViewModel == jobSearchViewModel)
                return;

            ContentViewModel = jobSearchViewModel;

            UpdateViewHistory(new ViewStackModel(ViewModel.JobSearchViewModel), false);
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
            dbContext.Dispose();
        }

        public void GoToForwardView()
        {
            if (viewHistoryIndex + 1 >= viewHistory.Count)
                return;

            ++viewHistoryIndex;

            var forwardView = viewHistory[viewHistoryIndex];

            switch (forwardView.ViewModel)
            {
                case ViewModel.JobListingViewModel:
                    ChangeViewToJobListing(forwardView.ItemId, true);
                    break;

                case ViewModel.ContactViewModel:
                    ChangeViewToContact(forwardView.ItemId, true);
                    break;

                case ViewModel.ContactsViewModel:
                    ContentViewModel = contactsViewModel;
                    break;

                case ViewModel.JobBoardViewModel:
                    ContentViewModel = jobBoardViewModel;
                    break;

                case ViewModel.JobSearchViewModel:
                    ContentViewModel = jobSearchViewModel;
                    break;

                case ViewModel.HelpViewModel:
                    ContentViewModel = helpViewModel;
                    break;

                default:
                    ContentViewModel = jobBoardViewModel;
                    ResetViewHistory();
                    break;
            }
        }

        public void GoToPreviousView()
        {
            if (viewHistory.Count <= 1)
                return;

            if (viewHistoryIndex <= 0)
                return;

            --viewHistoryIndex;

            var previousView = viewHistory[viewHistoryIndex];

            switch (previousView.ViewModel)
            {
                case ViewModel.JobListingViewModel:
                    ChangeViewToJobListing(previousView.ItemId, true);
                    break;

                case ViewModel.ContactViewModel:
                    ChangeViewToContact(previousView.ItemId, true);
                    break;

                case ViewModel.ContactsViewModel:
                    ContentViewModel = contactsViewModel;
                    break;

                case ViewModel.JobBoardViewModel:
                    ContentViewModel = jobBoardViewModel;
                    break;

                case ViewModel.JobSearchViewModel:
                    ContentViewModel = jobSearchViewModel;
                    break;

                case ViewModel.HelpViewModel:
                    ContentViewModel = helpViewModel;
                    break;

                default:
                    ContentViewModel = jobBoardViewModel;
                    ResetViewHistory();
                    break;
            }
        }

        // Cannot be static - it will throw an exception
#pragma warning disable CA1822 // Mark members as static
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

        private void ChangedContactView(int contactId, bool changedViaPreviousOrForwardButton)
        {
            UpdateViewHistory(new ViewStackModel(ViewModel.ContactViewModel, contactId), changedViaPreviousOrForwardButton);
        }

        private void ChangedJobListingView(int jobListingId, bool changedViaPreviousOrForwardButton)
        {
            UpdateViewHistory(new ViewStackModel(ViewModel.JobListingViewModel, jobListingId), changedViaPreviousOrForwardButton);
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

        // This method is void since it gets called synchronously in the constructor
        private void InitializeSingletons()
        {
            Singletons.JobListings = GetAllJobListingsAsync().GetAwaiter().GetResult();
            Singletons.Contacts = GetAllContactsAsync().GetAwaiter().GetResult();
        }

        private void ResetViewHistory()
        {
            viewHistory.Clear();

            if (ContentViewModel == addContactViewModel)
            {
                viewHistory.Add(new ViewStackModel(ViewModel.ContactViewModel, addContactViewModel.Contact.Id));
            }
            else if (ContentViewModel == jobListingViewModel)
            {
                viewHistory.Add(new ViewStackModel(ViewModel.JobListingViewModel, jobListingViewModel.JobListing.Id));
            }
            else if (ContentViewModel == contactsViewModel)
            {
                viewHistory.Add(new ViewStackModel(ViewModel.ContactsViewModel));
            }
            else if (ContentViewModel == jobBoardViewModel)
            {
                viewHistory.Add(new ViewStackModel(ViewModel.JobBoardViewModel));
            }
            else if (ContentViewModel == helpViewModel)
            {
                viewHistory.Add(new ViewStackModel(ViewModel.HelpViewModel));
            }
            else if (ContentViewModel == jobSearchViewModel)
            {
                viewHistory.Add(new ViewStackModel(ViewModel.JobSearchViewModel));
            }

            viewHistoryIndex = 0;
        }
        private void SubscribeToEvents()
        {
            jobListingViewModel.CreateNewContactWithAssociatedJobIdRequest += ChangeViewToAddContact;
            contactsViewModel.OpenAddContactViewRequest += ChangeViewToAddContact;

            jobBoardViewModel.OpenJobListingViewRequest += ChangeViewToJobListing;
            jobBoardViewModel.OpenJobListingViewByIdRequest += ChangeViewToJobListing;

            jobListingViewModel.OpenJobBoardViewRequest += ChangeViewToJobBoard;

            jobListingViewModel.ChangeViewToContactRequest += ChangeViewToContact;

            jobListingViewModel.UpdateJobBoardViewRequest += UpdateJobBoard;

            jobListingViewModel.ChangedJobListingViewEvent += ChangedJobListingView;

            addContactViewModel.OpenContactsViewRequest += ChangeViewToContacts;

            addContactViewModel.OpenJobListingViewRequest += ChangeViewToJobListing;

            addContactViewModel.UpdateContactsViewRequest += UpdateContacts;

            addContactViewModel.ChangedContactViewEvent += ChangedContactView;

            addContactViewModel.ResetViewHistoryRequest += ResetViewHistory;
            jobListingViewModel.ResetViewHistoryRequest += ResetViewHistory;
            jobBoardViewModel.ResetViewHistoryRequest += ResetViewHistory;
            contactsViewModel.ResetViewHistoryRequest += ResetViewHistory;
        }

        private void UnsubscribeFromEvents()
        {
            jobListingViewModel.CreateNewContactWithAssociatedJobIdRequest -= ChangeViewToAddContact;
            contactsViewModel.OpenAddContactViewRequest -= ChangeViewToAddContact;

            jobBoardViewModel.OpenJobListingViewRequest -= ChangeViewToJobListing;
            jobBoardViewModel.OpenJobListingViewByIdRequest -= ChangeViewToJobListing;

            jobListingViewModel.OpenJobBoardViewRequest -= ChangeViewToJobBoard;

            jobListingViewModel.ChangeViewToContactRequest -= ChangeViewToContact;

            jobListingViewModel.UpdateJobBoardViewRequest -= UpdateJobBoard;

            jobListingViewModel.ChangedJobListingViewEvent -= ChangedJobListingView;

            addContactViewModel.OpenContactsViewRequest -= ChangeViewToContacts;

            addContactViewModel.OpenJobListingViewRequest -= ChangeViewToJobListing;

            addContactViewModel.UpdateContactsViewRequest -= UpdateContacts;

            addContactViewModel.ChangedContactViewEvent -= ChangedContactView;

            addContactViewModel.ResetViewHistoryRequest -= ResetViewHistory;
            jobListingViewModel.ResetViewHistoryRequest -= ResetViewHistory;
            jobBoardViewModel.ResetViewHistoryRequest -= ResetViewHistory;
            contactsViewModel.ResetViewHistoryRequest -= ResetViewHistory;
        }

        private void UpdateViewHistory(ViewStackModel model, bool changedViaPreviousOrForwardButton)
        {
            if (!changedViaPreviousOrForwardButton)
            {
                viewHistory = viewHistory.Take(viewHistoryIndex + 1).ToList();
                viewHistory.Add(model);
                ++viewHistoryIndex;
                return;
            }
        }
    }
}