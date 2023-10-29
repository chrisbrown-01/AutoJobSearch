using AutoJobSearchShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Events.CustomEventArgs
{
    public class JobListingsBoolFieldChangedEventArgs : EventArgs // Needs to be public for delegates to work
    {
        public bool Value { get; set; }
        public int Id { get; set; }
        public JobListingsBoolField Field { get; set; }
    }
}
