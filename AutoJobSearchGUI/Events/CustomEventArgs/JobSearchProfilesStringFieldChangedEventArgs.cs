using AutoJobSearchShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Events.CustomEventArgs
{
    public class JobSearchProfilesStringFieldChangedEventArgs : EventArgs
    {
        public string Value { get; set; } = string.Empty;
        public int Id { get; set; }
        public JobSearchProfilesStringField Field { get; set; }
    }
}
