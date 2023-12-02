using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchShared.Models
{
    public class JobSearchProfile
    {
        public JobSearchProfile()
        {

        }

        public int Id { get; }

        public string ProfileName { get; set; } = "New Profile";

        public string Searches { get; set; } = string.Empty;

        public string KeywordsPositive { get; set; } = string.Empty;

        public string KeywordsNegative { get; set; } = string.Empty;

        public string SentimentsPositive { get; set; } = string.Empty;

        public string SentimentsNegative { get; set; } = string.Empty;
    }
}
