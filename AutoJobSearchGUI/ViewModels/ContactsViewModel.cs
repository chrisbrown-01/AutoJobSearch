using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
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

        [ObservableProperty]
        private ContactsQueryModel _contactsQueryModel;

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
            ContactsQueryModel = new();

            PageIndex = 0; 
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

            ContactsQueryModel = new();
        }

        [RelayCommand]
        private async Task ExecuteQueryAsync()
        {
            var contacts = await _dbContext.GetAllContactsAsync();

            if (ContactsQueryModel.JobIdEqualsEnabled)
            {
                contacts = contacts.Where(x => x.JobListingId == ContactsQueryModel.JobIdEquals);
            }

            if (ContactsQueryModel.EmailQueryStringEnabled)
            {
                contacts = contacts.Where(x => x.Email.Contains(ContactsQueryModel.EmailQueryString, StringComparison.OrdinalIgnoreCase));
            }

            if (ContactsQueryModel.PhoneQueryStringEnabled)
            {
                contacts = contacts.Where(x => x.Phone.Contains(ContactsQueryModel.PhoneQueryString, StringComparison.OrdinalIgnoreCase));
            }

            if (ContactsQueryModel.NameQueryStringEnabled)
            {
                contacts = contacts.Where(x => x.Name.Contains(ContactsQueryModel.NameQueryString, StringComparison.OrdinalIgnoreCase));
            }

            if (ContactsQueryModel.LinkedInQueryStringEnabled)
            {
                contacts = contacts.Where(x => x.LinkedIn.Contains(ContactsQueryModel.LinkedInQueryString, StringComparison.OrdinalIgnoreCase));
            }

            if (ContactsQueryModel.LocationQueryStringEnabled)
            {
                contacts = contacts.Where(x => x.Location.Contains(ContactsQueryModel.LocationQueryString, StringComparison.OrdinalIgnoreCase));
            }

            if (ContactsQueryModel.CompanyQueryStringEnabled)
            {
                contacts = contacts.Where(x => x.Company.Contains(ContactsQueryModel.CompanyQueryString, StringComparison.OrdinalIgnoreCase));
            }

            if (ContactsQueryModel.TitleQueryStringEnabled)
            {
                contacts = contacts.Where(x => x.Title.Contains(ContactsQueryModel.TitleQueryString, StringComparison.OrdinalIgnoreCase));
            }

            if (ContactsQueryModel.NotesQueryStringEnabled)
            {
                contacts = contacts.Where(x => x.Notes.Contains(ContactsQueryModel.NotesQueryString, StringComparison.OrdinalIgnoreCase));
            }

            Contacts = ConvertContactsToContactModels(contacts);
            ContactsDisplayed = Contacts.Skip(PageIndex * PageSize).Take(PageSize).ToList();
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

        [RelayCommand]
        private async Task DeleteAllContactsAsync()
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Confirm Delete All Contacts",
                "Are you sure you want to delete all contacts from the database? This action cannot be reversed.",
                MsBox.Avalonia.Enums.ButtonEnum.OkAbort,
                MsBox.Avalonia.Enums.Icon.Warning);

            var result = await box.ShowAsync();

            if (result == MsBox.Avalonia.Enums.ButtonResult.Ok)
            {
               await _dbContext.DeleteAllContactsAsync();
               await RenderDefaultContactsViewAsync();
            }          
        }

        [RelayCommand]
        private void GoToNextPage()
        {
            var contacts = Contacts.Skip((PageIndex + 1) * PageSize).Take(PageSize);
            if (!contacts.Any()) return;

            PageIndex++;
            ContactsDisplayed = contacts.ToList();
        }

        [RelayCommand]
        private void GoToPreviousPage()
        {
            if (PageIndex - 1 < 0) return;

            PageIndex--;
            ContactsDisplayed = Contacts.Skip(PageIndex * PageSize).Take(PageSize).ToList();
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
