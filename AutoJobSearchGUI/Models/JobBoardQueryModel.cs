﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Models
{
    internal partial class JobBoardQueryModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isAppliedTo;

        [ObservableProperty]
        private bool _isInterviewing;

        [ObservableProperty]
        private bool _isRejected;

        [ObservableProperty]
        private bool _isFavourite;

        [ObservableProperty]
        private bool _isHidden;

        [ObservableProperty]
        private bool _sortBySearchTerm;

        [ObservableProperty]
        private bool _sortByCreatedAt;

        [ObservableProperty]
        private bool _sortByScore;

        [ObservableProperty]
        private bool _orderByAscending;

        [ObservableProperty]
        private DateTimeOffset _searchedOnDate = DateTime.Today;

        [ObservableProperty]
        private bool _searchedOnDateEnabled;

        [ObservableProperty]
        private bool _searchedBetweenDatesEnabled;

        [ObservableProperty]
        private DateTimeOffset _searchedOnDateStart = DateTime.Today.AddDays(-1);

        [ObservableProperty]
        private DateTimeOffset _searchedOnDateEnd = DateTime.Today;

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
