using AutoJobSearchShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Events.CustomEventArgs
{
    public class JobSearchProfilesIntFieldChangedEventArgs : EventArgs
    {
        public int Value { get; set; } 
        public int Id { get; set; }
        public JobSearchProfilesIntField Field { get; set; }
    }
}
