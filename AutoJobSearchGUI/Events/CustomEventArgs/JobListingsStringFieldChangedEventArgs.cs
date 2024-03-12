using AutoJobSearchShared.Enums;
using System;

namespace AutoJobSearchGUI.Events.CustomEventArgs
{
    public class JobListingsStringFieldChangedEventArgs : EventArgs // Needs to be public for delegates to work
    {
        public JobListingsStringField Field { get; set; }
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}