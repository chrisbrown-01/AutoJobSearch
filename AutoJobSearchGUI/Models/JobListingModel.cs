using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Events.CustomEventArgs;
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
    public partial class JobListingModel : ObservableObject // Needs to be public for delegates to work
    {
        public int Id { get; set; }
        public string SearchTerm { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string Description { get; set; } = string.Empty;

        public string ApplicationLinks { get; set; } = string.Empty;

        public int Score { get; set; }

        // This property prevents re-fetching job listing details from the database if they have already been populated.
        public bool DetailsPopulated { get; set; } = false;

        // This property prevents events from unnecessarily firing when the view model is simply instantiating new models.
        public bool EnableEvents { get; set; } = false;

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

        public static event EventHandler<JobListingsStringFieldChangedEventArgs>? StringFieldChanged;

        public static event EventHandler<JobListingsBoolFieldChangedEventArgs>? BoolFieldChanged;

        // Note that these methods technically cause an excessive amount of database calls but since there is only a single user
        // interacting with the database, the technical debt is justified to ensure that no data loss occurs if the application
        // unexpectedly crashes before the user can request for the changes to be saved to the database.

        partial void OnNotesChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new JobListingsStringFieldChangedEventArgs { Field = JobListingsStringField.Notes, Value = value, Id = this.Id });
        }

        partial void OnIsAppliedToChanged(bool value)
        {
            if (!this.EnableEvents) return;
            BoolFieldChanged?.Invoke(this, new JobListingsBoolFieldChangedEventArgs { Field = JobListingsBoolField.IsAppliedTo, Value = value, Id = this.Id });
        }

        partial void OnIsInterviewingChanged(bool value)
        {
            if (!this.EnableEvents) return;
            BoolFieldChanged?.Invoke(this, new JobListingsBoolFieldChangedEventArgs { Field = JobListingsBoolField.IsInterviewing, Value = value, Id = this.Id });
        }

        partial void OnIsRejectedChanged(bool value)
        {
            if (!this.EnableEvents) return;
            BoolFieldChanged?.Invoke(this, new JobListingsBoolFieldChangedEventArgs { Field = JobListingsBoolField.IsRejected, Value = value, Id = this.Id });
        }

        partial void OnIsFavouriteChanged(bool value)
        {
            if (!this.EnableEvents) return;
            BoolFieldChanged?.Invoke(this, new JobListingsBoolFieldChangedEventArgs { Field = JobListingsBoolField.IsFavourite, Value = value, Id = this.Id });
        }

        partial void OnIsHiddenChanged(bool value)
        {
            if (!this.EnableEvents) return;
            BoolFieldChanged?.Invoke(this, new JobListingsBoolFieldChangedEventArgs { Field = JobListingsBoolField.IsHidden, Value = value, Id = this.Id });
        }
    }
}
