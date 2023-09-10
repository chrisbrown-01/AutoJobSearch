using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchConsoleApp
{
    public class JobListing
    {
        public int Id { get; set; }
        public string SearchTerm { get; set; } = "FOUND BY SEARCH TERM: ___"; // TODO: update

        public DateTime CreatedAt { get; private set; } = DateTime.Now;

        public string Description_Raw { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<string> ApplicationLinks_Raw { get; set; } = new();

        public List<string> ApplicationLinks { get; set; } = new();

        public int Score { get; set; } = 0;

        public bool IsAppliedTo { get; set; } = false;

        public bool IsInterviewing { get; set; } = false;

        public bool IsRejected { get; set; } = false;

        public string Notes { get; set; } = string.Empty;  
    }
}
