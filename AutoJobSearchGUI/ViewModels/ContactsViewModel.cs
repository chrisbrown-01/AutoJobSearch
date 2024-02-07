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

        // TODO: implement switching to add contact view
        //public delegate void OpenJobListingViewHandler(JobListingModel job, IEnumerable<JobListingModel> jobListings);
        //public event OpenJobListingViewHandler? OpenJobListingViewRequest;

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
            PageSize = 25;

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

        private async Task<List<Contact>> GetAllContactsAsync()
        {
            //var jobs = await _dbContext.GetAllJobListingsAsync();
            //return ConvertJobListingsToJobListingModels(jobs);

            var allContacts = await _dbContext.GetAllContactsAsync();

            return allContacts.ToList(); // TODO: will need to convert to local model with ObservableProperty attributes
        }
    }
}
