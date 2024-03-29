﻿using AutoJobSearchGUI.Events.CustomEventArgs;
using AutoJobSearchShared.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace AutoJobSearchGUI.Models
{
    public partial class ContactModel : ObservableObject // Needs to be public for delegates to work
    {
        [ObservableProperty]
        private string _company = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private List<int> _jobListingIds = new();

        [ObservableProperty]
        private string _linkedIn = string.Empty;

        [ObservableProperty]
        private string _location = string.Empty;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _notes = string.Empty;

        [ObservableProperty]
        private string _phone = string.Empty;

        [ObservableProperty]
        private string _title = string.Empty;

        public static event EventHandler<ContactStringFieldChangedEventArgs>? StringFieldChanged;

        public DateTime CreatedAt { get; set; }

        // This property prevents events from unnecessarily firing when the view model is simply instantiating new models.
        public bool EnableEvents { get; set; } = false;

        public int Id { get; set; }

        // Note that these methods technically cause an excessive amount of database calls but since there is only a single user
        // interacting with the database, the technical debt is justified to ensure that no data loss occurs if the application
        // unexpectedly crashes before the user can request for the changes to be saved to the database.
        partial void OnCompanyChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new ContactStringFieldChangedEventArgs { Field = ContactStringField.Company, Value = value, Id = this.Id });
        }

        partial void OnEmailChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new ContactStringFieldChangedEventArgs { Field = ContactStringField.Email, Value = value, Id = this.Id });
        }

        partial void OnLinkedInChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new ContactStringFieldChangedEventArgs { Field = ContactStringField.LinkedIn, Value = value, Id = this.Id });
        }

        partial void OnLocationChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new ContactStringFieldChangedEventArgs { Field = ContactStringField.Location, Value = value, Id = this.Id });
        }

        partial void OnNameChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new ContactStringFieldChangedEventArgs { Field = ContactStringField.Name, Value = value, Id = this.Id });
        }

        partial void OnNotesChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new ContactStringFieldChangedEventArgs { Field = ContactStringField.Notes, Value = value, Id = this.Id });
        }

        partial void OnPhoneChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new ContactStringFieldChangedEventArgs { Field = ContactStringField.Phone, Value = value, Id = this.Id });
        }

        partial void OnTitleChanged(string value)
        {
            if (!this.EnableEvents) return;
            StringFieldChanged?.Invoke(this, new ContactStringFieldChangedEventArgs { Field = ContactStringField.Title, Value = value, Id = this.Id });
        }
    }
}