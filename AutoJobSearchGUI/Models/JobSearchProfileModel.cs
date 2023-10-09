using AutoJobSearchGUI.Events;
using AutoJobSearchShared.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Models
{
    public partial class JobSearchProfileModel : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private string _profileName = string.Empty;

        [ObservableProperty]
        private string _searches = string.Empty;

        [ObservableProperty]
        private string _keywordsPositive = string.Empty;

        [ObservableProperty]
        private string _keywordsNegative = string.Empty;

        [ObservableProperty]
        private string _sentimentsPositive = string.Empty;

        [ObservableProperty]
        private string _sentimentsNegative = string.Empty;

        public static event EventHandler<JobSearchProfilesStringFieldChangedEventArgs>? StringFieldChanged;

        // Note that these methods technically cause an excessive amount of database calls but since there is only a single user
        // interacting with the database, the technical debt is justified to ensure that no data loss occurs if the application
        // unexpectedly crashes before the user can request for the changes to be saved to the database.

        // TODO: convert all of these to the 2 argument old/new method and compare. if comparison is equal, do not invoke the event
        partial void OnProfileNameChanged(string value)
        {
            StringFieldChanged?.Invoke(this, new JobSearchProfilesStringFieldChangedEventArgs { Field = JobSearchProfilesStringField.ProfileName, Value = value, Id = this.Id });
        }

        partial void OnSearchesChanged(string value)
        {
            StringFieldChanged?.Invoke(this, new JobSearchProfilesStringFieldChangedEventArgs { Field = JobSearchProfilesStringField.Searches, Value = value, Id = this.Id });
        }

        partial void OnKeywordsPositiveChanged(string value)
        {
            StringFieldChanged?.Invoke(this, new JobSearchProfilesStringFieldChangedEventArgs { Field = JobSearchProfilesStringField.KeywordsPositive, Value = value, Id = this.Id });
        }

        partial void OnKeywordsNegativeChanged(string value)
        {
            StringFieldChanged?.Invoke(this, new JobSearchProfilesStringFieldChangedEventArgs { Field = JobSearchProfilesStringField.KeywordsNegative, Value = value, Id = this.Id });
        }

        partial void OnSentimentsPositiveChanged(string value)
        {
            StringFieldChanged?.Invoke(this, new JobSearchProfilesStringFieldChangedEventArgs { Field = JobSearchProfilesStringField.SentimentsPositive, Value = value, Id = this.Id });
        }

        partial void OnSentimentsNegativeChanged(string value)
        {
            StringFieldChanged?.Invoke(this, new JobSearchProfilesStringFieldChangedEventArgs { Field = JobSearchProfilesStringField.SentimentsNegative, Value = value, Id = this.Id });
        }
    }
}
