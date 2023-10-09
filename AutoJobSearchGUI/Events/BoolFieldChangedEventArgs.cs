using AutoJobSearchShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Events
{
    internal class BoolFieldChangedEventArgs : EventArgs
    {
        public bool Value { get; set; }
        public int Id { get; set; }
        public DbBoolField Field { get; set; }
    }
}
