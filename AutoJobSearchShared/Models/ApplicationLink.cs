using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchShared.Models
{
    public class ApplicationLink
    {
        public ApplicationLink()
        {

        }

        public int Id { get; }

        public int JobListingId { get; set; }

        public string Link { get; set; } = string.Empty;

        public string Link_RawHTML { get; set; } = string.Empty;
    }
}
