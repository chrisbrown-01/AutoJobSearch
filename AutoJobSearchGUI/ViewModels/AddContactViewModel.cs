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
    public partial class AddContactViewModel : ViewModelBase
    {
        private readonly IDbContext _dbContext;

        [ObservableProperty]
        private Contact _contact;

        private List<Contact> Contacts { get; set; } = default!;

        public AddContactViewModel(IDbContext dbContext)
        {
            _dbContext = dbContext;
            Contact = new Contact();
        }

        [RelayCommand]
        private void PopulateContacts(IEnumerable<Contact> contacts)
        {
            Contacts = contacts.ToList();
        }

        [RelayCommand]
        private void OpenContact(Contact? contact)
        {
            if (contact is not null)
            {
                this.Contact = contact;
            }

            //EnableOnChangedEvents(Contact);
        }

        // TODO: ensure that undo action in text boxes reflects in the SQLite database events and writing
        // TODO: create tests
    }
}
