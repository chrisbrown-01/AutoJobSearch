using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Models
{
    public partial class ContactModel : ObservableObject // Needs to be public for delegates to work
    {
        public int Id { get; set; }

        public int? JobListingId { get; set; } // TODO: figure out multiple linked jobs and how to modify

        [ObservableProperty]
        private DateTime _createdAt;

        [ObservableProperty]
        private string _company = string.Empty;

        [ObservableProperty]
        private string _location = string.Empty;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _phone = string.Empty;

        [ObservableProperty]
        private string _linkedIn = string.Empty;

        [ObservableProperty]
        private string _notes = string.Empty;
    }
}
