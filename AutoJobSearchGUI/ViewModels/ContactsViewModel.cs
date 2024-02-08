using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class ContactsViewModel : ViewModelBase
    {
        private readonly IDbContext _dbContext;

        public delegate void OpenAddContactViewHandler(ContactModel? contact, IEnumerable<ContactModel> contacts);
        public event OpenAddContactViewHandler? OpenAddContactViewRequest;

        private List<ContactModel> Contacts { get; set; } = default!;

        [ObservableProperty]
        private List<ContactModel> _contactsDisplayed = default!;

        [ObservableProperty]
        private ContactModel? _selectedContact;

        [ObservableProperty]
        private int _pageIndex;

        [ObservableProperty]
        private int _pageSize;

        public ContactsViewModel(IDbContext dbContext)
        {
            _dbContext = dbContext;

            PageIndex = 0; // TODO: implement page switching methods
            PageSize = 50; // TODO: allow customization

            RenderDefaultContactsViewAsync().Wait();
        }

        public void UpdateContacts(IEnumerable<ContactModel> contacts)
        {
            Contacts = contacts.ToList();
            ContactsDisplayed = Contacts.Skip(PageIndex * PageSize).Take(PageSize).ToList();
        }

        [RelayCommand]
        private async Task RenderDefaultContactsViewAsync()
        {
            PageIndex = 0;
            Contacts = await GetAllContactsAsync();
            ContactsDisplayed = Contacts.Skip(PageIndex * PageSize).Take(PageSize).ToList();

            //JobBoardQueryModel = new();
        }

        [RelayCommand]
        private void OpenContact()
        {
            if (SelectedContact == null) return;
            OpenAddContactViewRequest?.Invoke(SelectedContact, Contacts);
        }

        [RelayCommand]
        private void AddNewContact()
        {
            OpenAddContactViewRequest?.Invoke(null, Contacts);
        }

        [RelayCommand]
        private async Task DeleteContactAsync()
        {
            if (SelectedContact == null) return;

            await _dbContext.DeleteContactAsync(SelectedContact.Id);
            Contacts.Remove(SelectedContact); 
            ContactsDisplayed.Remove(SelectedContact);
        }

        private async Task<List<ContactModel>> GetAllContactsAsync()
        {
            var allContacts = await _dbContext.GetAllContactsAsync();
            return ConvertContactsToContactModels(allContacts);
        }

        private static List<ContactModel> ConvertContactsToContactModels(IEnumerable<Contact> contacts)
        {
            var contactModels = new List<ContactModel>();

            foreach (var contact in contacts)
            {
                var contactModel = new ContactModel
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

                contactModels.Add(contactModel);
            }

            return contactModels;
        }
    }
}
