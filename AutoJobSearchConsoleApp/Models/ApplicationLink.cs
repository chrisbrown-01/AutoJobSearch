using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchConsoleApp.Models
{
    public class ApplicationLink
    {
        public int Id { get; set; }
        
        public int JobListingId { get; set; } // Foreign Key
        
        public string Link { get; set; } = string.Empty;

        public string Link_RawHTML { get; set; } = string.Empty;
    }
}
