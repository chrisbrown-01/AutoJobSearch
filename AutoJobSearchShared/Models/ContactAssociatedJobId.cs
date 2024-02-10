using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchShared.Models
{
    public class ContactAssociatedJobId
    {
        public ContactAssociatedJobId()
        {
            
        }

        public int Id { get; }

        public int ContactId { get; set; }

        public int JobListingId { get; set; }
    }
}
