using AutoJobSearchShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Events
{
    internal class StringFieldChangedEventArgs : EventArgs
    {
        public string Value { get; set; } = string.Empty;
        public int Id { get; set; }
        public DbStringField Field { get; set; }
    }
}
