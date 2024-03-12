using AutoJobSearchGUI.Events.CustomEventArgs;
using AutoJobSearchShared.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace AutoJobSearchGUI.Models
{
    public partial class JobSearchProfileModel : ObservableObject
    {
        [ObservableProperty]
        private string _keywordsNegative = string.Empty;

        [ObservableProperty]
        private string _keywordsPositive = string.Empty;

        // TODO: add notes in Help section that describes what this is for
        [ObservableProperty]
        private int _maxJobListingIndex;

        [ObservableProperty]
        private string _profileName = string.Empty;

        [ObservableProperty]
        private string _searches = string.Empty;

        [ObservableProperty]
        private string _sentimentsNegative = string.Empty;

        [ObservableProperty]
        private string _sentimentsPositive = string.Empty;

        public static event EventHandler<JobSearchProfilesIntFieldChangedEventArgs>? IntFieldChanged;

        public static event EventHandler<JobSearchProfilesStringFieldChangedEventArgs>? StringFieldChanged;

        // This property prevents events from unnecessarily firing when the view model is simply instantiating new models.
        public bool EnableEvents { get; set; } = false;

        public int Id { get; set; }
        // Note that these methods technically cause an excessive amount of database calls but since there is only a single user
        // interacting with the database, the technical debt is justified to ensure that no data loss occurs if the application
        // unexpectedly crashes before the user can request for the changes to be saved to the database.

        partial void OnKeywordsNegativeChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new JobSearchProfilesStringFieldChangedEventArgs { Field = JobSearchProfilesStringField.KeywordsNegative, Value = value, Id = this.Id });
        }

        partial void OnKeywordsPositiveChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new JobSearchProfilesStringFieldChangedEventArgs { Field = JobSearchProfilesStringField.KeywordsPositive, Value = value, Id = this.Id });
        }

        partial void OnMaxJobListingIndexChanged(int value)
        {
            if (!this.EnableEvents) return;
            IntFieldChanged?.Invoke(this, new JobSearchProfilesIntFieldChangedEventArgs { Field = JobSearchProfilesIntField.MaxJobListingIndex, Value = value, Id = this.Id });
        }

        partial void OnProfileNameChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new JobSearchProfilesStringFieldChangedEventArgs { Field = JobSearchProfilesStringField.ProfileName, Value = value, Id = this.Id });
        }

        partial void OnSearchesChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new JobSearchProfilesStringFieldChangedEventArgs { Field = JobSearchProfilesStringField.Searches, Value = value, Id = this.Id });
        }

        partial void OnSentimentsNegativeChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new JobSearchProfilesStringFieldChangedEventArgs { Field = JobSearchProfilesStringField.SentimentsNegative, Value = value, Id = this.Id });
        }

        partial void OnSentimentsPositiveChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new JobSearchProfilesStringFieldChangedEventArgs { Field = JobSearchProfilesStringField.SentimentsPositive, Value = value, Id = this.Id });
        }
    }
}