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

        private List<Contact> Contacts { get; set; } = default!;

        public AddContactViewModel(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [RelayCommand]
        private void PopulateContacts(IEnumerable<Contact> contacts)
        {
            Contacts = contacts.ToList();
        }

        [RelayCommand]
        private async Task CreateNewContact()
        {
            var newContact = await _dbContext.CreateNewContactAsync(new Contact());
            this.Contact = ConvertContactToContactModel(newContact);
        }

        [RelayCommand]
        private void OpenContact(Contact contact)
        {
            this.Contact = ConvertContactToContactModel(contact);
            //EnableOnChangedEvents(Contact);
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
