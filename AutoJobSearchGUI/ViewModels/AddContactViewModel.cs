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
    public partial class AddContactViewModel : ViewModelBase
    {
        private readonly IDbContext _dbContext;

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

        [RelayCommand]
        private async Task CreateNewContactAsync() 
        {
            var newContact = await _dbContext.CreateNewContactAsync(new Contact());
            var newContactModel = ConvertContactToContactModel(newContact);

            Contacts.Add(newContactModel);
            // TODO: create event that notifies that a new contact has been created and added. might be able to just broadcast the id of new object

            Contact = newContactModel;
        }

        [RelayCommand]
        private void OpenContact(ContactModel contact)
        {
            Contact = contact;
            //EnableOnChangedEvents(Contact);
        }

        [RelayCommand]
        private void GoToPreviousContact()
        {
            // TODO: remove complexity?
            //var currentIndex = Contacts.IndexOf(Contacts.Single(contact => contact.Id == this.Contact.Id));
            var currentIndex = Contacts.IndexOf(Contact);
            if (currentIndex < 0) return;

            var previousIndex = currentIndex - 1;
            if (previousIndex < 0) return;

            // DisableOnChangedEvents(JobListing);

            Contact = Contacts[previousIndex];
        }

        // TODO: test empty list
        [RelayCommand]
        private void GoToNextContact()
        {
            // TODO: remove complexity?
            //var currentIndex = Contacts.IndexOf(Contacts.Single(contact => contact.Id == this.Contact.Id));
            var currentIndex = Contacts.IndexOf(Contact);
            if (currentIndex < 0) return;

            var nextIndex = currentIndex + 1;
            if (nextIndex >= Contacts.Count) return;

            // DisableOnChangedEvents(JobListing);

            Contact = Contacts[nextIndex];
        }

        [RelayCommand]
        private async Task DeleteContactAsync()
        {
            ContactModel? nextContactToDisplay;

            // TODO: remove complexity?
            //var currentIndex = Contacts.IndexOf(Contacts.Single(contact => contact.Id == this.Contact.Id));
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

            await _dbContext.DeleteContactAsync(Contact.Id);

            Contacts.Remove(Contact); // TODO: is this propogated to ContactsViewModel?

            if (nextContactToDisplay != null)
            {
                Contact = nextContactToDisplay;
            }
            else
            {
                OpenContactsViewRequest?.Invoke(); // Return to Contacts view if no contacts are available to display.
            }
        }

        // TODO: delete?, can consolidate with the ContactsViewModel
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
    }
}
