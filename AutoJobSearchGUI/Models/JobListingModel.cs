using AutoJobSearchConsoleApp.Models;
using AutoJobSearchGUI.Data;
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
    public partial class JobListingModel : ObservableObject
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

        // Note that these methods technically cause an excessive amount of database calls but since there is only a single user
        // interacting with the database, the technical debt is justified to ensure that no data loss occurs if the application
        // unexpectedly crashes before the user can request for the changes to be saved to the database.

        partial void OnNotesChanged(string value)
        {
            Debug.WriteLine($"Updating notes for listing id {this.Id}"); // TODO: proper logging
            Task.Run(async () => await SQLiteDb.UpdateNotesById(this.Id, value));
        }

        partial void OnIsAppliedToChanged(bool value)
        {
            Debug.WriteLine($"Updating IsAppliedTo for listing id {this.Id}"); // TODO: proper logging
            Task.Run(async () => await DbContextSQLite.UpdateDatabase("IsAppliedTo", value, this.Id)); // TODO: convert to use DI, async
        }

        partial void OnIsInterviewingChanged(bool value)
        {
            Debug.WriteLine($"Updating IsInterviewing for listing id {this.Id}"); // TODO: proper logging
            Task.Run(async () => await DbContextSQLite.UpdateDatabase("IsInterviewing", value, this.Id)); // TODO: convert to use DI, async
        }

        partial void OnIsRejectedChanged(bool value)
        {
            Debug.WriteLine($"Updating IsRejected for listing id {this.Id}"); // TODO: proper logging
            Task.Run(async () => await DbContextSQLite.UpdateDatabase("IsRejected", value, this.Id)); // TODO: convert to use DI, async
        }

        partial void OnIsFavouriteChanged(bool value)
        {
            Debug.WriteLine($"Updating IsFavourite for listing id {this.Id}"); // TODO: proper logging
            Task.Run(async () => await DbContextSQLite.UpdateDatabase("IsFavourite", value, this.Id)); // TODO: convert to use DI, async
        }

        partial void OnIsHiddenChanged(bool value)
        {
            Debug.WriteLine($"Updating IsHidden for listing id {this.Id}"); // TODO: proper logging
            Task.Run(async () => await DbContextSQLite.UpdateDatabase("IsHidden", value, this.Id)); // TODO: convert to use DI, async
        }
    }
}
