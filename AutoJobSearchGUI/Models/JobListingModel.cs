using AutoJobSearchConsoleApp.Models;
using AutoJobSearchGUI.Data;
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

        //public string ApplicationLinksAsString()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach(var link in ApplicationLinks)
        //    {
        //        sb.AppendLine(link);
        //    }

        //    return sb.ToString();
        //}

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
        private string _notes = string.Empty;

        partial void OnIsAppliedToChanged(bool value)
        {
            Debug.WriteLine($"Updating IsAppliedTo for listing id {this.Id}"); // TODO: proper logging
            DbContextSQLite.UpdateDatabase("IsAppliedTo", value, this.Id); // TODO: convert to use DI
        }

        partial void OnIsInterviewingChanged(bool value)
        {
            Debug.WriteLine($"Updating IsInterviewing for listing id {this.Id}"); // TODO: proper logging
            DbContextSQLite.UpdateDatabase("IsInterviewing", value, this.Id);
        }

        partial void OnIsRejectedChanged(bool value)
        {
            Debug.WriteLine($"Updating IsRejected for listing id {this.Id}"); // TODO: proper logging
            DbContextSQLite.UpdateDatabase("IsRejected", value, this.Id);
        }

        partial void OnIsFavouriteChanged(bool value)
        {
            Debug.WriteLine($"Updating IsFavourite for listing id {this.Id}"); // TODO: proper logging
            DbContextSQLite.UpdateDatabase("IsFavourite", value, this.Id);
        }
    }
}
