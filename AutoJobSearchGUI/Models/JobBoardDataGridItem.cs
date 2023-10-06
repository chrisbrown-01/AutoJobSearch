using AutoJobSearchShared;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Models
{
    public partial class JobBoardDataGridItem : ObservableObject
    {
        public int Id { get; set; }

        public string SearchTerm { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string Description { get; set; } = string.Empty;

        public int Score { get; set; }

        [ObservableProperty]
        private bool _isAppliedTo;

        [ObservableProperty]
        private bool _isInterviewing;

        [ObservableProperty]
        private bool _isRejected;

        [ObservableProperty]
        private bool _isFavourite;

        partial void OnIsAppliedToChanged(bool value)
        {
            UpdateDatabase("IsAppliedTo", value, this.Id);
        }

        partial void OnIsInterviewingChanged(bool value)
        {
            UpdateDatabase("IsInterviewing", value, this.Id);
        }

        partial void OnIsRejectedChanged(bool value)
        {
            UpdateDatabase("IsRejected", value, this.Id);
        }

        partial void OnIsFavouriteChanged(bool value)
        {
            UpdateDatabase("IsFavourite", value, this.Id);
        }

        private void UpdateDatabase(string columnName, bool value, int id) // TODO: async Task?
        {
            Debug.WriteLine("Updating database"); // TODO: proper logging
            SQLiteDb.UpdateDatabaseBoolPropertyById(columnName, value, id);
        }
    }
}
