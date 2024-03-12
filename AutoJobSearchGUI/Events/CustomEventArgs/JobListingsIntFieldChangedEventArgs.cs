using AutoJobSearchShared.Enums;
using System;

namespace AutoJobSearchGUI.Events.CustomEventArgs
{
    public class JobListingsIntFieldChangedEventArgs : EventArgs // Needs to be public for delegates to work
    {
        public JobListingsIntField Field { get; set; }
        public int Id { get; set; }
        public int Value { get; set; }
    }
}