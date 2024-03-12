using AutoJobSearchShared.Enums;
using System;

namespace AutoJobSearchGUI.Events.CustomEventArgs
{
    public class JobSearchProfilesIntFieldChangedEventArgs : EventArgs
    {
        public JobSearchProfilesIntField Field { get; set; }
        public int Id { get; set; }
        public int Value { get; set; }
    }
}