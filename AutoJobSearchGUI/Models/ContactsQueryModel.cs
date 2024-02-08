using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Models
{
    public partial class ContactsQueryModel : ObservableObject // Needs to be public for delegates to work
    {
        [ObservableProperty]
        private bool _sortByCompany = false;

        [ObservableProperty]
        private bool _sortByLocation = false;

        [ObservableProperty]
        private bool _sortByName = false;

        [ObservableProperty]
        private bool _sortByTitle = false;

        [ObservableProperty]
        private bool _sortByEmail = false;

        [ObservableProperty]
        private bool _sortByPhone = false;

        [ObservableProperty]
        private bool _sortByLinkedIn = false;

        [ObservableProperty]
        private bool _sortByJobId = false;

        [ObservableProperty]
        private bool _orderByDescending = false;

        [ObservableProperty]
        private bool _companyQueryStringEnabled = false;

        [ObservableProperty]
        private string _companyQueryString = string.Empty;

        [ObservableProperty]
        private bool _locationQueryStringEnabled = false;

        [ObservableProperty]
        private string _locationQueryString = string.Empty;

        [ObservableProperty]
        private bool _nameQueryStringEnabled = false;

        [ObservableProperty]
        private string _nameQueryString = string.Empty;

        [ObservableProperty]
        private bool _titleQueryStringEnabled = false;

        [ObservableProperty]
        private string _titleQueryString = string.Empty;

        [ObservableProperty]
        private bool _emailQueryStringEnabled = false;

        [ObservableProperty]
        private string _emailQueryString = string.Empty;

        [ObservableProperty]
        private bool _phoneQueryStringEnabled = false;

        [ObservableProperty]
        private string _phoneQueryString = string.Empty;

        [ObservableProperty]
        private bool _linkedInQueryStringEnabled = false;

        [ObservableProperty]
        private string _linkedInQueryString = string.Empty;

        [ObservableProperty]
        private bool _notesQueryStringEnabled = false;

        [ObservableProperty]
        private string _notesQueryString = string.Empty;

        [ObservableProperty]
        private bool _jobIdEqualsEnabled = false;

        [ObservableProperty]
        private int? _jobIdEquals = 1;
    }
}
