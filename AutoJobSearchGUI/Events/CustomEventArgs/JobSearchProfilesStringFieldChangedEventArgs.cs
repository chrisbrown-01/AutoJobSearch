using AutoJobSearchShared.Enums;
using System;

namespace AutoJobSearchGUI.Events.CustomEventArgs
{
    public class JobSearchProfilesStringFieldChangedEventArgs : EventArgs
    {
        public JobSearchProfilesStringField Field { get; set; }
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}