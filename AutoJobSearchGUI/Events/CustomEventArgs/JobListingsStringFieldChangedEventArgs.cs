using AutoJobSearchShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Events.CustomEventArgs
{
    public class JobListingsStringFieldChangedEventArgs : EventArgs // Needs to be public for delegates to work
    {
        public string Value { get; set; } = string.Empty;
        public int Id { get; set; }
        public JobListingsStringField Field { get; set; }
    }
}
