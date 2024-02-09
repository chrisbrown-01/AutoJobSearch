using AutoJobSearchGUI.Data;
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
    // TODO: autocomplete box for new entries
    public partial class AddContactViewModel : ViewModelBase
    {
        private readonly IDbContext _dbContext;

        public delegate void UpdateContactsHandler(IEnumerable<ContactModel> contacts);
        public event UpdateContactsHandler? UpdateContactsRequest;

        public delegate void OpenContactsViewHandler();
        public event OpenContactsViewHandler? OpenContactsViewRequest;

        [ObservableProperty]
        private ContactModel _contact = default!;

        private List<ContactModel> Contacts { get; set; } = default!;

        public AddContactViewModel(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [RelayCommand]
        private void PopulateContacts(IEnumerable<ContactModel> contacts)
        {
            Contacts = contacts.ToList();
        }

        // TODO: need to be able to call this from within a job listing
        // TODO: need to be able to navigate back to the job listing after it has been created
        [RelayCommand]
        private async Task CreateNewContactAsync() 
        {
            var newContact = await _dbContext.CreateNewContactAsync(new Contact());
            var newContactModel = ConvertContactToContactModel(newContact);

            Contacts.Add(newContactModel);

            UpdateContactsRequest?.Invoke(Contacts);

            OpenContact(newContactModel);
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

        private static ContactModel ConvertContactToContactModel(Contact contact)
        {
            return new ContactModel()
            {
                Id = contact.Id,
                JobListingId = contact.JobListingId,
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

        // TODO: ensure that undo action in text boxes reflects in the SQLite database events and writing
        // TODO: create tests

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
