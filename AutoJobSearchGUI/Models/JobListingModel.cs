using AutoJobSearchConsoleApp.Models;
using AutoJobSearchGUI.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
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

        public List<string> ApplicationLinks { get; set; } = new();

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
            DbContextSQLite.UpdateDatabase("IsAppliedTo", value, this.Id); // TODO: convert to use DI
        }

        partial void OnIsInterviewingChanged(bool value)
        {
            DbContextSQLite.UpdateDatabase("IsInterviewing", value, this.Id);
        }

        partial void OnIsRejectedChanged(bool value)
        {
            DbContextSQLite.UpdateDatabase("IsRejected", value, this.Id);
        }

        partial void OnIsFavouriteChanged(bool value)
        {
            DbContextSQLite.UpdateDatabase("IsFavourite", value, this.Id);
        }
    }
}
