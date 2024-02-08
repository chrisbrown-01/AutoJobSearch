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

        [ObservableProperty]
        private ContactModel _contact = default!;

        private List<ContactModel> Contacts { get; set; } = default!;

        public AddContactViewModel(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [RelayCommand]
        private void PopulateContacts(IEnumerable<Contact> contacts)
        {
            if (Contacts is not null) return;

            Contacts = new();

            foreach (var contact in contacts)
            {
                Contacts.Add(ConvertContactToContactModel(contact));
            }
        }

        [RelayCommand]
        private async Task CreateNewContact()
        {
            var newContact = await _dbContext.CreateNewContactAsync(new Contact());
            Contacts.Add(ConvertContactToContactModel(newContact));
            // TODO: create event that notifies that a new contact has been created and added. might be able to just broadcast the id of new object

            Contact = ConvertContactToContactModel(newContact);
        }

        [RelayCommand]
        private void OpenContact(Contact contact)
        {
            Contact = ConvertContactToContactModel(contact);
            //EnableOnChangedEvents(Contact);
        }

        [RelayCommand]
        private void GoToPreviousContact()
        {
            var currentIndex = Contacts.IndexOf(Contacts.Single(contact => contact.Id == this.Contact.Id));
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
            var currentIndex = Contacts.IndexOf(Contacts.Single(contact => contact.Id == this.Contact.Id));
            if (currentIndex < 0) return;

            var nextIndex = currentIndex + 1;
            if (nextIndex >= Contacts.Count) return;

            // DisableOnChangedEvents(JobListing);

            Contact = Contacts[nextIndex];
        }

        [RelayCommand]
        private void DeleteContact()
        {

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
    }
}
