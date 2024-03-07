using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoJobSearchGUI.ViewModels
{
    // TODO: create tests
    public partial class AddContactViewModel : ViewModelBase
    {
        private readonly IDbContext _dbContext;

        public delegate void OpenJobListingViewHandler(int jobListingId);
        public event OpenJobListingViewHandler? OpenJobListingViewRequest;

        public delegate void UpdateContactsViewHandler();
        public event UpdateContactsViewHandler? UpdateContactsViewRequest;

        public delegate void OpenContactsViewHandler();
        public event OpenContactsViewHandler? OpenContactsViewRequest;

        [ObservableProperty]
        private ContactModel _contact = default!;

        [ObservableProperty]
        private IEnumerable<string> _contacts_Companies = default!;

        [ObservableProperty]
        private IEnumerable<string> _contacts_Locations = default!;

        [ObservableProperty]
        private IEnumerable<string> _contacts_Titles = default!;

        [ObservableProperty]
        private object _selectedJobListingId = new();

        [ObservableProperty]
        private bool _isNavigateToJobButtonEnabled;

        public AddContactViewModel(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [RelayCommand]
        private async Task CreateContactAssociatedJobIdAsync(string jobIdTextBoxInput)
        {
            if (Int32.TryParse(jobIdTextBoxInput, out int jobId))
            {
                if (Contact.JobListingIds.Contains(jobId)) return;

                // Make sure the job ID exists before allowing the user to add it to the database
                var allJobListings = await _dbContext.GetAllJobListingsAsync();
                var allJobIds = allJobListings.Select(x => x.Id);
                var singletonJobIdExists = Singletons.JobListings.Exists(x => x.Id == jobId);

                if (allJobIds is null || !allJobIds.Contains(jobId) || !singletonJobIdExists) return;

                var associatedJobIdRecord = await _dbContext.CreateContactAssociatedJobIdAsync(Contact.Id, jobId);
                Contact.JobListingIds.Add(associatedJobIdRecord.JobListingId);

                // Re-open contact to ensure that "Navigate" button in the view functions properly.
                // This still doesn't fix the bug where the user cannot immediately select a newly added item in the ListBox.
                // I cannot fix this bug, and it seems the only way to resolve is when the user navigates to a new contact,
                // which causes the JobIds in the list box to reset.
                DisableOnChangedEvents(Contact);
                OpenContact(Contact); 
            }
        }

        [RelayCommand]
        private async Task DeleteContactAssociatedJobIdAsync(string jobIdTextBoxInput)
        {
            if (Int32.TryParse(jobIdTextBoxInput, out int jobId))
            {
                if (!Contact.JobListingIds.Contains(jobId)) return;

                await _dbContext.DeleteContactAssociatedJobIdAsync(Contact.Id, jobId);
                Contact.JobListingIds.Remove(jobId);

                // Re-open contact to ensure that "Navigate" button in the view functions properly.
                // This still doesn't fix the bug where the user cannot immediately select a newly added item in the ListBox.
                // I cannot fix this bug, and it seems the only way to resolve is when the user navigates to a new contact,
                // which causes the JobIds in the list box to reset.
                DisableOnChangedEvents(Contact);
                OpenContact(Contact); 
            }
        }

        [RelayCommand]
        private async Task CreateNewContactAsync(int? jobId)
        {
            var newContact = await _dbContext.CreateNewContactAsync(new Contact());
            List<int> jobIds = new();

            if (jobId is not null)
            {
                var associatedJobIdRecord = await _dbContext.CreateContactAssociatedJobIdAsync(newContact.Id, jobId.Value);
                jobIds.Add(associatedJobIdRecord.JobListingId);
            }

            var newContactModel = ConvertContactToContactModel(newContact, jobIds);
            Singletons.Contacts.Add(newContactModel);
            UpdateContactsViewRequest?.Invoke();

            OpenContact(newContactModel);
        }

        [RelayCommand]
        private void OpenJobListing()
        {
            if (SelectedJobListingId is not int || SelectedJobListingId is null) return;
            if ((int)SelectedJobListingId < 1) return;

            DisableOnChangedEvents(Contact);
            OpenJobListingViewRequest?.Invoke((int)SelectedJobListingId);
            SelectedJobListingId = -1; // Set to invalid number so the currently selected integer does not persist by accident.
        }

        [RelayCommand]
        private void OpenContact(ContactModel contact)
        {
            Contact = contact;

            // Ensure controls in the view are up-to-date
            IsNavigateToJobButtonEnabled = Contact.JobListingIds.Any();
            Contacts_Companies = Singletons.Contacts.Select(x => x.Company).Distinct();
            Contacts_Locations = Singletons.Contacts.Select(x => x.Location).Distinct();
            Contacts_Titles = Singletons.Contacts.Select(x => x.Title).Distinct();

            EnableOnChangedEvents(Contact);
        }

        [RelayCommand]
        private void GoToPreviousContact()
        {
            var currentIndex = Singletons.Contacts.IndexOf(Contact);
            if (currentIndex < 0) return;

            var previousIndex = currentIndex - 1;
            if (previousIndex < 0) return;

            DisableOnChangedEvents(Contact);

            OpenContact(Singletons.Contacts[previousIndex]);
        }

        [RelayCommand]
        private void GoToNextContact()
        {
            var currentIndex = Singletons.Contacts.IndexOf(Contact);
            if (currentIndex < 0) return;

            var nextIndex = currentIndex + 1;
            if (nextIndex >= Singletons.Contacts.Count) return;

            DisableOnChangedEvents(Contact);

            OpenContact(Singletons.Contacts[nextIndex]);
        }

        [RelayCommand]
        private async Task DeleteContactAsync()
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Confirm Delete Contact",
                "Are you sure you want to delete this contact? This action cannot be reversed.",
                MsBox.Avalonia.Enums.ButtonEnum.OkAbort,
                MsBox.Avalonia.Enums.Icon.Warning);

            var result = await box.ShowAsync();

            if (result != MsBox.Avalonia.Enums.ButtonResult.Ok) return;

            ContactModel? nextContactToDisplay;

            var currentIndex = Singletons.Contacts.IndexOf(Contact);

            var nextContact = Singletons.Contacts.ElementAtOrDefault(currentIndex + 1);
            var previousContact = Singletons.Contacts.ElementAtOrDefault(currentIndex - 1);

            if (nextContact != null) // Try to choose the next contact as the new one to display.
            {
                nextContactToDisplay = nextContact;
            }
            else if (previousContact != null) // If next contact is not available, try to choose the previous one.
            {
                nextContactToDisplay = previousContact;
            }
            else // Otherwise we have no contacts at all to display.
            {
                nextContactToDisplay = null;
            }

            DisableOnChangedEvents(Contact);

            await _dbContext.DeleteContactAsync(Contact.Id);
            Singletons.Contacts.Remove(Contact);
            UpdateContactsViewRequest?.Invoke();

            if (nextContactToDisplay != null)
            {
                OpenContact(nextContactToDisplay);
            }
            else
            {
                OpenContactsViewRequest?.Invoke(); // Return to Contacts view if no contacts are available to display.
            }
        }

        // TODO: move to ContactHelpers class
        private static ContactModel ConvertContactToContactModel(Contact contact, IEnumerable<int> associatedJobIds)
        {
            return new ContactModel()
            {
                Id = contact.Id,
                JobListingIds = associatedJobIds.ToList(),
                CreatedAt = contact.CreatedAt,
                Company = contact.Company,
                Location = contact.Location,
                Name = contact.Name,
                Title = contact.Title,
                Email = contact.Email,
                Phone = contact.Phone,
                LinkedIn = contact.LinkedIn,
                Notes = contact.Notes
            };
        }

        /// <summary>
        /// Allows events to fire. This method should be called after the view model properties have been fully instantiated.
        /// </summary>
        /// <param name="contact"></param>
        private void EnableOnChangedEvents(ContactModel contact)
        {
            contact.EnableEvents = true;
        }

        /// <summary>
        /// Prevent events from firing. This method should be called in preparation of instantiating new view model properties.
        /// </summary>
        /// <param name="contact"></param>
        private void DisableOnChangedEvents(ContactModel contact)
        {
            contact.EnableEvents = false;
        }
    }
}
