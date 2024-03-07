using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Models
{
    public partial class JobBoardQueryModel : ObservableObject // Needs to be public for delegates to work
    {
        [ObservableProperty]
        private bool _isToBeAppliedTo;

        [ObservableProperty]
        private bool _isAppliedTo;

        [ObservableProperty]
        private bool _isInterviewing;

        [ObservableProperty]
        private bool _isNegotiating;

        [ObservableProperty]
        private bool _isRejected;

        [ObservableProperty]
        private bool _isDeclinedOffer;

        [ObservableProperty]
        private bool _isAcceptedOffer;

        [ObservableProperty]
        private bool _isFavourite;

        [ObservableProperty]
        private bool _isHidden;

        [ObservableProperty]
        private bool _sortBySearchTerm;

        [ObservableProperty]
        private bool _sortByCreatedAt;

        [ObservableProperty]
        private bool _sortByModifiedAt; 

        [ObservableProperty]
        private bool _sortByScore;

        [ObservableProperty]
        private bool _orderByAscending;

        [ObservableProperty]
        private DateTimeOffset _createdAtDate = DateTime.Today;

        [ObservableProperty]
        private DateTimeOffset _modifiedAtDate = DateTime.Today; 

        [ObservableProperty]
        private bool _columnFiltersEnabled;

        [ObservableProperty]
        private bool _createdAtDateEnabled;

        [ObservableProperty]
        private bool _createdBetweenDatesEnabled;

        [ObservableProperty]
        private DateTimeOffset _createdBetweenDateStart = DateTime.Today.AddDays(-1);

        [ObservableProperty]
        private DateTimeOffset _createdBetweenDateEnd = DateTime.Today;

        [ObservableProperty]
        private bool _modifiedAtDateEnabled; 

        [ObservableProperty]
        private bool _modifiedBetweenDatesEnabled; 

        [ObservableProperty]
        private DateTimeOffset _modifiedBetweenDateStart = DateTime.Today.AddDays(-1); 

        [ObservableProperty]
        private DateTimeOffset _modifiedBetweenDateEnd = DateTime.Today; 

        [ObservableProperty]
        private bool _scoreEqualsEnabled;

        [ObservableProperty]
        private bool _scoreRangeEnabled;

        [ObservableProperty]
        private int _scoreEquals = 0;

        [ObservableProperty]
        private int _scoreRangeMin = -1;

        [ObservableProperty]
        private int _scoreRangeMax = 1;

        [ObservableProperty]
        private bool _searchTermQueryStringEnabled;

        [ObservableProperty]
        private string _searchTermQueryString = string.Empty;

        [ObservableProperty]
        private bool _jobDescriptionQueryStringEnabled;

        [ObservableProperty]
        private string _jobDescriptionQueryString = string.Empty;

        [ObservableProperty]
        private bool _notesQueryStringEnabled;

        [ObservableProperty]
        private string _notesQueryString = string.Empty;
    }
}
