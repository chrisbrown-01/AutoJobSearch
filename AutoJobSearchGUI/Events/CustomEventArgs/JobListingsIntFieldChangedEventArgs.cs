using AutoJobSearchShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Events.CustomEventArgs
{
    public class JobListingsIntFieldChangedEventArgs : EventArgs // Needs to be public for delegates to work
    {
        public int Value { get; set; }
        public int Id { get; set; }
        public JobListingsIntField Field { get; set; }
    }
}
