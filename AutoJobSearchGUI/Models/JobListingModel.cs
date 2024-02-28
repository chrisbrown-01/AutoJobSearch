﻿using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Events.CustomEventArgs;
using AutoJobSearchShared;
using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
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

        public DateTime CreatedAt { get; set; }

        public string ApplicationLinks { get; set; } = string.Empty;

        // This property prevents re-fetching job listing details from the database if they have already been populated.
        public bool DetailsPopulated { get; set; } = false;

        // This property prevents events from unnecessarily firing when the view model is simply instantiating new models.
        public bool EnableEvents { get; set; } = false;

        [ObservableProperty]
        private int _score = 0;

        [ObservableProperty]
        // Keeps track of the most recent time a bool property was changed.
        private DateTime _statusModifiedAt;

        [ObservableProperty]
        private JobListingAssociatedFiles? _jobListingAssociatedFiles;

        [ObservableProperty]
        private string _searchTerm = string.Empty; 

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _notes = string.Empty;

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

        public static event EventHandler<JobListingsStringFieldChangedEventArgs>? StringFieldChanged;

        public static event EventHandler<JobListingsBoolFieldChangedEventArgs>? BoolFieldChanged;

        public static event EventHandler<JobListingsIntFieldChangedEventArgs>? IntFieldChanged;

        // Note that these methods technically cause an excessive amount of database calls but since there is only a single user
        // interacting with the database, the technical debt is justified to ensure that no data loss occurs if the application
        // unexpectedly crashes before the user can request for the changes to be saved to the database.

        partial void OnScoreChanged(int value)
        {
            if (!this.EnableEvents) return;
            IntFieldChanged?.Invoke(this, new JobListingsIntFieldChangedEventArgs { Field = JobListingsIntField.Score, Value = value, Id = this.Id });
        }

        partial void OnSearchTermChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new JobListingsStringFieldChangedEventArgs { Field = JobListingsStringField.SearchTerm, Value = value, Id = this.Id });
        }

        partial void OnDescriptionChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new JobListingsStringFieldChangedEventArgs { Field = JobListingsStringField.Description, Value = value, Id = this.Id });
        }

        partial void OnNotesChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new JobListingsStringFieldChangedEventArgs { Field = JobListingsStringField.Notes, Value = value, Id = this.Id });
        }

        partial void OnIsToBeAppliedToChanged(bool value)
        {
            if (!this.EnableEvents) return;

            StatusModifiedAt = DateTime.Now;

            BoolFieldChanged?.Invoke(this,
                new JobListingsBoolFieldChangedEventArgs
                {
                    Field = JobListingsBoolField.IsToBeAppliedTo,
                    Value = value,
                    Id =
                    this.Id,
                    StatusModifiedAt = this.StatusModifiedAt
                });
        }

        partial void OnIsAppliedToChanged(bool value)
        {
            if (!this.EnableEvents) return;

            StatusModifiedAt = DateTime.Now;

            BoolFieldChanged?.Invoke(this,
                new JobListingsBoolFieldChangedEventArgs
                {
                    Field = JobListingsBoolField.IsAppliedTo,
                    Value = value,
                    Id =
                    this.Id,
                    StatusModifiedAt = this.StatusModifiedAt
                });
        }

        partial void OnIsInterviewingChanged(bool value)
        {
            if (!this.EnableEvents) return;

            StatusModifiedAt = DateTime.Now;

            BoolFieldChanged?.Invoke(this,
                new JobListingsBoolFieldChangedEventArgs
                {
                    Field = JobListingsBoolField.IsInterviewing,
                    Value = value,
                    Id =
                    this.Id,
                    StatusModifiedAt = this.StatusModifiedAt
                });
        }

        partial void OnIsNegotiatingChanged(bool value)
        {
            if (!this.EnableEvents) return;

            StatusModifiedAt = DateTime.Now;

            BoolFieldChanged?.Invoke(this,
                new JobListingsBoolFieldChangedEventArgs
                {
                    Field = JobListingsBoolField.IsNegotiating,
                    Value = value,
                    Id =
                    this.Id,
                    StatusModifiedAt = this.StatusModifiedAt
                });
        }

        partial void OnIsRejectedChanged(bool value)
        {
            if (!this.EnableEvents) return;

            StatusModifiedAt = DateTime.Now;

            BoolFieldChanged?.Invoke(this,
                new JobListingsBoolFieldChangedEventArgs
                {
                    Field = JobListingsBoolField.IsRejected,
                    Value = value,
                    Id =
                    this.Id,
                    StatusModifiedAt = this.StatusModifiedAt
                });
        }

        partial void OnIsDeclinedOfferChanged(bool value)
        {
            if (!this.EnableEvents) return;

            StatusModifiedAt = DateTime.Now;

            BoolFieldChanged?.Invoke(this,
                new JobListingsBoolFieldChangedEventArgs
                {
                    Field = JobListingsBoolField.IsDeclinedOffer,
                    Value = value,
                    Id =
                    this.Id,
                    StatusModifiedAt = this.StatusModifiedAt
                });
        }

        partial void OnIsAcceptedOfferChanged(bool value)
        {
            if (!this.EnableEvents) return;

            StatusModifiedAt = DateTime.Now;

            BoolFieldChanged?.Invoke(this,
                new JobListingsBoolFieldChangedEventArgs
                {
                    Field = JobListingsBoolField.IsAcceptedOffer,
                    Value = value,
                    Id =
                    this.Id,
                    StatusModifiedAt = this.StatusModifiedAt
                });
        }

        partial void OnIsFavouriteChanged(bool value)
        {
            if (!this.EnableEvents) return;

            StatusModifiedAt = DateTime.Now;

            BoolFieldChanged?.Invoke(this,
                new JobListingsBoolFieldChangedEventArgs
                {
                    Field = JobListingsBoolField.IsFavourite,
                    Value = value,
                    Id =
                    this.Id,
                    StatusModifiedAt = this.StatusModifiedAt
                });
        }

        partial void OnIsHiddenChanged(bool value)
        {
            if (!this.EnableEvents) return;

            StatusModifiedAt = DateTime.Now;

            BoolFieldChanged?.Invoke(this,
                new JobListingsBoolFieldChangedEventArgs
                {
                    Field = JobListingsBoolField.IsHidden,
                    Value = value,
                    Id =
                    this.Id,
                    StatusModifiedAt = this.StatusModifiedAt
                });
        }
    }
}
