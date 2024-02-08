using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Models
{
    public class ContactsQueryModel
    {
        public ContactsQueryModel()
        {
            
        }

        public bool SortByJobId { get; set; }
        public bool SortByCompany { get; set; }
        public bool SortByLocation { get; set; }
        public bool SortByName { get; set; }
        public bool SortByTitle { get; set; }
        public bool SortByEmail { get; set; }
        public bool SortByPhone { get; set; }
        public bool SortByLinkedIn { get; set; }
        public bool orderByAscending { get; set; }
        public bool JobIdEqualsEnabled { get; set; }
        public int JobIdEquals { get; set; } = 1;
        public bool CompanyQueryStringEnabled { get; set; }
        public string CompanyQueryString { get; set; } = string.Empty;
        public bool LocationQueryStringEnabled { get; set; }
        public string LocationQueryString { get; set; } = string.Empty;
        public bool NameQueryStringEnabled { get; set; }
        public string NameQueryString { get; set; } = string.Empty;
        public bool TitleQueryStringEnabled { get; set; }
        public string TitleQueryString { get; set; } = string.Empty;
        public bool EmailQueryStringEnabled { get; set; }
        public string EmailQueryString { get; set; } = string.Empty;
        public bool PhoneQueryStringEnabled { get; set; }
        public string PhoneQueryString { get; set; } = string.Empty;
        public bool LinkedInQueryStringEnabled { get; set; }
        public string LinkedInQueryString { get; set; } = string.Empty;
        public bool NotesQueryStringEnabled { get; set; }
        public string NotesQueryString { get; set; } = string.Empty;
    }

    //public partial class ContactsQueryModel : ObservableObject // Needs to be public for delegates to work
    //{
    //    [ObservableProperty]
    //    private bool _sortByJobId = false;

    //    [ObservableProperty]
    //    private bool _sortByCompany = false;

    //    [ObservableProperty]
    //    private bool _sortByLocation = false;

    //    [ObservableProperty]
    //    private bool _sortByName = false;

    //    [ObservableProperty]
    //    private bool _sortByTitle = false;

    //    [ObservableProperty]
    //    private bool _sortByEmail = false;

    //    [ObservableProperty]
    //    private bool _sortByPhone = false;

    //    [ObservableProperty]
    //    private bool _sortByLinkedIn = false;

    //    [ObservableProperty]
    //    private bool _orderByAscending = false;

    //    [ObservableProperty]
    //    private bool _jobIdEqualsEnabled = false;

    //    [ObservableProperty]
    //    private int _jobIdEquals = 1;

    //    [ObservableProperty]
    //    private bool _companyQueryStringEnabled = false;

    //    [ObservableProperty]
    //    private string _companyQueryString = string.Empty;

    //    [ObservableProperty]
    //    private bool _locationQueryStringEnabled = false;

    //    [ObservableProperty]
    //    private string _locationQueryString = string.Empty;

    //    [ObservableProperty]
    //    private bool _nameQueryStringEnabled = false;

    //    [ObservableProperty]
    //    private string _nameQueryString = string.Empty;

    //    [ObservableProperty]
    //    private bool _titleQueryStringEnabled = false;

    //    [ObservableProperty]
    //    private string _titleQueryString = string.Empty;

    //    [ObservableProperty]
    //    private bool _emailQueryStringEnabled = false;

    //    [ObservableProperty]
    //    private string _emailQueryString = string.Empty;

    //    [ObservableProperty]
    //    private bool _phoneQueryStringEnabled = false;

    //    [ObservableProperty]
    //    private string _phoneQueryString = string.Empty;

    //    [ObservableProperty]
    //    private bool _linkedInQueryStringEnabled = false;

    //    [ObservableProperty]
    //    private string _linkedInQueryString = string.Empty;

    //    [ObservableProperty]
    //    private bool _notesStringEnabled = false;

    //    [ObservableProperty]
    //    private string _notesQueryString = string.Empty;
    //}
}
