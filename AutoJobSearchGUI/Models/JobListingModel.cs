using AutoJobSearchConsoleApp.Models;
using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Events;
using AutoJobSearchShared;
using AutoJobSearchShared.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Models
{
    internal partial class JobListingModel : ObservableObject 
    {
        public int Id { get; set; }
        public string SearchTerm { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } 

        public string Description { get; set; } = string.Empty;

        public string ApplicationLinks { get; set; } = string.Empty;

        public int Score { get; set; }

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
        private string _notes = string.Empty;

        public static event EventHandler<StringFieldChangedEventArgs>? StringFieldChanged;

        public static event EventHandler<BoolFieldChangedEventArgs>? BoolFieldChanged;

        // Note that these methods technically cause an excessive amount of database calls but since there is only a single user
        // interacting with the database, the technical debt is justified to ensure that no data loss occurs if the application
        // unexpectedly crashes before the user can request for the changes to be saved to the database.

        partial void OnNotesChanged(string value)
        {
            StringFieldChanged?.Invoke(this, new StringFieldChangedEventArgs { Field = DbStringField.Notes, Value = value, Id = this.Id });
        }

        partial void OnIsAppliedToChanged(bool value)
        {
            BoolFieldChanged?.Invoke(this, new BoolFieldChangedEventArgs { Field = DbBoolField.IsAppliedTo, Value = value, Id = this.Id });
        }

        partial void OnIsInterviewingChanged(bool value)
        {
            BoolFieldChanged?.Invoke(this, new BoolFieldChangedEventArgs { Field = DbBoolField.IsInterviewing, Value = value, Id = this.Id });
        }

        partial void OnIsRejectedChanged(bool value)
        {
            BoolFieldChanged?.Invoke(this, new BoolFieldChangedEventArgs { Field = DbBoolField.IsRejected, Value = value, Id = this.Id });
        }

        partial void OnIsFavouriteChanged(bool value)
        {
            BoolFieldChanged?.Invoke(this, new BoolFieldChangedEventArgs { Field = DbBoolField.IsFavourite, Value = value, Id = this.Id });
        }

        partial void OnIsHiddenChanged(bool value)
        {
            BoolFieldChanged?.Invoke(this, new BoolFieldChangedEventArgs { Field = DbBoolField.IsHidden, Value = value, Id = this.Id });
        }
    }
}
