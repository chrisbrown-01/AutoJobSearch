using AutoJobSearchShared.Enums;
using System;

namespace AutoJobSearchGUI.Events.CustomEventArgs
{
    public class JobListingsBoolFieldChangedEventArgs : EventArgs // Needs to be public for delegates to work
    {
        public JobListingsBoolField Field { get; set; }
        public int Id { get; set; }
        public DateTime StatusModifiedAt { get; set; }
        public bool Value { get; set; }
    }
}