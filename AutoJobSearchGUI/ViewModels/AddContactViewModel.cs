using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        public delegate void UpdateContactsHandler(IEnumerable<ContactModel> contacts);
        public event UpdateContactsHandler? UpdateContactsRequest;

        public delegate void OpenContactsViewHandler();
        public event OpenContactsViewHandler? OpenContactsViewRequest;

        private List<ContactModel> Contacts { get; set; } = default!;

        [ObservableProperty]
        private ContactModel _contact = default!;

        [ObservableProperty]
        private IEnumerable<string> _contacts_Companies = default!;

        [ObservableProperty]
        private IEnumerable<string> _contacts_Locations = default!;

        [ObservableProperty]
        private IEnumerable<string> _contacts_Titles = default!;

        public AddContactViewModel(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [RelayCommand]
        private async Task PopulateContacts(IEnumerable<ContactModel>? contacts)
        {
            if(contacts is not null)
            {
                Contacts = contacts.ToList();
            }
            else
            {
                Contacts = await GetAllContactModelsAsync();
            }            

            Contacts_Companies = Contacts.Select(x => x.Company).Distinct();
            Contacts_Locations = Contacts.Select(x => x.Location).Distinct();
            Contacts_Titles = Contacts.Select(x => x.Title).Distinct();
        }

        // TODO: need to be able to navigate back to the job listing after it has been created
        [RelayCommand]
        private async Task CreateNewContactAsync(int? jobId)
        {
            var newContact = await _dbContext.CreateNewContactAsync(new Contact());
            List<int> jobIds = new();

            if (jobId is not null)
            {
                var associatedJobIdRecord = await _dbContext.CreateContactAssociatedJobId(newContact.Id, jobId.Value);
                jobIds.Add(associatedJobIdRecord.JobListingId);
            }

            var newContactModel = ConvertContactToContactModel(newContact, jobIds);
            Contacts.Add(newContactModel);
            UpdateContactsRequest?.Invoke(Contacts);
            OpenContact(newContactModel);
        }

        [RelayCommand]
        private void OpenJobListing(int jobId)
        {
            var test = jobId;
            // TODO: implement
        }

        [RelayCommand]
        private void OpenContact(ContactModel contact)
        {
            Contact = contact;
            EnableOnChangedEvents(Contact);
        }

        [RelayCommand]
        private void GoToPreviousContact()
        {
            var currentIndex = Contacts.IndexOf(Contact);
            if (currentIndex < 0) return;

            var previousIndex = currentIndex - 1;
            if (previousIndex < 0) return;

            DisableOnChangedEvents(Contact);

            OpenContact(Contacts[previousIndex]);
        }

        [RelayCommand]
        private void GoToNextContact()
        {
            var currentIndex = Contacts.IndexOf(Contact);
            if (currentIndex < 0) return;

            var nextIndex = currentIndex + 1;
            if (nextIndex >= Contacts.Count) return;

            DisableOnChangedEvents(Contact);

            OpenContact(Contacts[nextIndex]);
        }

        [RelayCommand]
        private async Task DeleteContactAsync()
        {            
            ContactModel? nextContactToDisplay;

            var currentIndex = Contacts.IndexOf(Contact);

            var nextContact = Contacts.ElementAtOrDefault(currentIndex + 1);
            var previousContact = Contacts.ElementAtOrDefault(currentIndex - 1);

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

            Contacts.Remove(Contact); 

            UpdateContactsRequest?.Invoke(Contacts);

            if (nextContactToDisplay != null)
            {
                OpenContact(nextContactToDisplay);
            }
            else
            {
                OpenContactsViewRequest?.Invoke(); // Return to Contacts view if no contacts are available to display.
            }
        }

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

        private async Task<List<ContactModel>> GetAllContactModelsAsync()
        {
            var contacts = await _dbContext.GetAllContactsAsync();
            var contactsAssociatedJobIds = await _dbContext.GetAllContactsAssociatedJobIdsAsync();
            return ContactsHelpers.ConvertContactsToContactModels(contacts, contactsAssociatedJobIds);
        }
    }
}
