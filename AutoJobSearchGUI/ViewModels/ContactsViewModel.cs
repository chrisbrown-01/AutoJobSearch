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

        public delegate void OpenAddContactViewHandler(Contact? contact, IEnumerable<Contact> contacts);
        public event OpenAddContactViewHandler? OpenAddContactViewRequest;

        private List<Contact> Contacts { get; set; } = default!;

        [ObservableProperty]
        private List<Contact> _contactsDisplayed = default!;

        [ObservableProperty]
        private Contact? _selectedContact;

        [ObservableProperty]
        private int _pageIndex;

        [ObservableProperty]
        private int _pageSize;

        public ContactsViewModel(IDbContext dbContext)
        {
            _dbContext = dbContext;

            PageIndex = 0;
            PageSize = 50; // TODO: allow customization

            RenderDefaultContactsViewAsync().Wait();
        }

        [RelayCommand]
        private async Task RenderDefaultContactsViewAsync()
        {
            PageIndex = 0;
            Contacts = await GetAllContactsAsync();
            ContactsDisplayed = Contacts.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            //EnableOnChangedEvents(JobListingsDisplayed);

            //JobBoardQueryModel = new();
        }

        [RelayCommand]
        private void OpenContact()
        {
            if (SelectedContact == null) return;
            // DisableOnChangedEvents(JobListingsDisplayed);
            OpenAddContactViewRequest?.Invoke(SelectedContact, Contacts);
        }

        [RelayCommand]
        private void AddNewContact()
        {
            // DisableOnChangedEvents(JobListingsDisplayed);
            OpenAddContactViewRequest?.Invoke(null, Contacts);
        }

        private async Task<List<Contact>> GetAllContactsAsync()
        {
            //var jobs = await _dbContext.GetAllJobListingsAsync();
            //return ConvertJobListingsToJobListingModels(jobs);

            var allContacts = await _dbContext.GetAllContactsAsync();

            return allContacts.ToList(); // TODO: will need to convert to local model with ObservableProperty attributes
        }
    }
}
