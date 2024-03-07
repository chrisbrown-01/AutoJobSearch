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
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class ContactsViewModel : ViewModelBase
    {
        private readonly IDbContext _dbContext;

        public delegate void OpenAddContactViewHandler(ContactModel? contact);
        public event OpenAddContactViewHandler? OpenAddContactViewRequest;

        [ObservableProperty]
        private ContactsQueryModel _contactsQueryModel;

        [ObservableProperty]
        private List<ContactModel> _contactsDisplayed = default!;

        [ObservableProperty]
        private ContactModel? _selectedContact;

        [ObservableProperty]
        private int _pageIndex;

        [ObservableProperty]
        private int _pageSize;

        [ObservableProperty]
        private IEnumerable<string> _contacts_Companies = default!;

        [ObservableProperty]
        private IEnumerable<string> _contacts_Locations = default!;

        [ObservableProperty]
        private IEnumerable<string> _contacts_Names = default!;

        [ObservableProperty]
        private IEnumerable<string> _contacts_Titles = default!;

        [ObservableProperty]
        private IEnumerable<string> _contacts_Emails = default!;

        [ObservableProperty]
        private IEnumerable<string> _contacts_Phones = default!;

        [ObservableProperty]
        private IEnumerable<string> _contacts_LinkedIns = default!;

        private void SetAutoCompleteBoxFields()
        {
            Contacts_Companies = Singletons.Contacts.Select(x => x.Company).Distinct();
            Contacts_Locations = Singletons.Contacts.Select(x => x.Location).Distinct();
            Contacts_Names = Singletons.Contacts.Select(x => x.Name).Distinct();
            Contacts_Titles = Singletons.Contacts.Select(x => x.Title).Distinct();
            Contacts_Emails = Singletons.Contacts.Select(x => x.Email).Distinct();
            Contacts_Phones = Singletons.Contacts.Select(x => x.Phone).Distinct();
            Contacts_LinkedIns = Singletons.Contacts.Select(x => x.LinkedIn).Distinct();
        }

        public ContactsViewModel(IDbContext dbContext)
        {
            _dbContext = dbContext;
            ContactsQueryModel = new();

            PageIndex = 0; 
            PageSize = 50; // TODO: allow customization

            RenderDefaultContactsViewCommand.Execute(null);
        }

        public void UpdateContacts()
        {
            ContactsDisplayed = Singletons.Contacts.Skip(PageIndex * PageSize).Take(PageSize).ToList();

            SetAutoCompleteBoxFields();
        }

        [RelayCommand]
        private async Task RenderDefaultContactsViewAsync()
        {
            PageIndex = 0;
            Singletons.Contacts = await GetAllContactModelsAsync();
            ContactsDisplayed = Singletons.Contacts.Skip(PageIndex * PageSize).Take(PageSize).ToList();

            ContactsQueryModel = new();

            SetAutoCompleteBoxFields();
        }

        [RelayCommand]
        private async Task ExecuteQueryAsync()
        {
            var contacts = await _dbContext.GetAllContactsAsync();
            var contactsAssociatedJobIds = await _dbContext.GetAllContactsAssociatedJobIdsAsync();

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

            if (ContactsQueryModel.SortByCompany)
            {
                if (ContactsQueryModel.OrderByDescending)
                {
                    contacts = contacts.OrderByDescending(x => x.Company);
                }
                else
                {
                    contacts = contacts.OrderBy(x => x.Company);
                }
            }
            else if (ContactsQueryModel.SortByLocation)
            {
                if (ContactsQueryModel.OrderByDescending)
                {
                    contacts = contacts.OrderByDescending(x => x.Location);
                }
                else
                {
                    contacts = contacts.OrderBy(x => x.Location);
                }
            }
            else if (ContactsQueryModel.SortByName)
            {
                if (ContactsQueryModel.OrderByDescending)
                {
                    contacts = contacts.OrderByDescending(x => x.Name);
                }
                else
                {
                    contacts = contacts.OrderBy(x => x.Name);
                }
            }
            else if (ContactsQueryModel.SortByTitle)
            {
                if (ContactsQueryModel.OrderByDescending)
                {
                    contacts = contacts.OrderByDescending(x => x.Title);
                }
                else
                {
                    contacts = contacts.OrderBy(x => x.Title);
                }
            }
            else if (ContactsQueryModel.SortByEmail)
            {
                if (ContactsQueryModel.OrderByDescending)
                {
                    contacts = contacts.OrderByDescending(x => x.Email);
                }
                else
                {
                    contacts = contacts.OrderBy(x => x.Email);
                }
            }
            else if (ContactsQueryModel.SortByPhone)
            {
                if (ContactsQueryModel.OrderByDescending)
                {
                    contacts = contacts.OrderByDescending(x => x.Phone);
                }
                else
                {
                    contacts = contacts.OrderBy(x => x.Phone);
                }
            }
            else if (ContactsQueryModel.SortByLinkedIn)
            {
                if (ContactsQueryModel.OrderByDescending)
                {
                    contacts = contacts.OrderByDescending(x => x.LinkedIn);
                }
                else
                {
                    contacts = contacts.OrderBy(x => x.LinkedIn);
                }
            }
            else
            {
                if (ContactsQueryModel.OrderByDescending)
                {
                    contacts = contacts.OrderByDescending(x => x.Id);
                }
                else
                {
                    contacts = contacts.OrderBy(x => x.Id);
                }
            }

            PageIndex = 0;
            Singletons.Contacts = ContactsHelpers.ConvertContactsToContactModels(contacts, contactsAssociatedJobIds);
            ContactsDisplayed = Singletons.Contacts.Skip(PageIndex * PageSize).Take(PageSize).ToList();
        }

        [RelayCommand]
        private void OpenContact()
        {
            if (SelectedContact == null) return;
            OpenAddContactViewRequest?.Invoke(SelectedContact);
        }

        [RelayCommand]
        private void AddNewContact()
        {
            OpenAddContactViewRequest?.Invoke(null);
        }

        [RelayCommand]
        private async Task DeleteContactAsync()
        {
            if (SelectedContact == null) return;

            await _dbContext.DeleteContactAsync(SelectedContact.Id);
            Singletons.Contacts.Remove(SelectedContact); 
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

            if (result != MsBox.Avalonia.Enums.ButtonResult.Ok) return;

            await _dbContext.DeleteAllContactsAsync();
            await RenderDefaultContactsViewAsync();        
        }

        [RelayCommand]
        private void GoToNextPage()
        {
            var contacts = Singletons.Contacts.Skip((PageIndex + 1) * PageSize).Take(PageSize);
            if (!contacts.Any()) return;

            PageIndex++;
            ContactsDisplayed = contacts.ToList();
        }

        [RelayCommand]
        private void GoToPreviousPage()
        {
            if (PageIndex - 1 < 0) return;

            PageIndex--;
            ContactsDisplayed = Singletons.Contacts.Skip(PageIndex * PageSize).Take(PageSize).ToList();
        }

        private async Task<List<ContactModel>> GetAllContactModelsAsync()
        {
            var contacts = await _dbContext.GetAllContactsAsync();
            var contactsAssociatedJobIds = await _dbContext.GetAllContactsAssociatedJobIdsAsync();
            return ContactsHelpers.ConvertContactsToContactModels(contacts, contactsAssociatedJobIds);
        }
    }
}
