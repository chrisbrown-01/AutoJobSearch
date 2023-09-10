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
        public string SearchTerm { get; set; } = "FOUND BY SEARCH TERM: ___";

        public DateTime CreatedAt { get; private set; } = DateTime.Now;

        public string InnerText { get; set; } = string.Empty;

        public string InnerTextCleaned { get; set; } = string.Empty;

        public List<string> LinksOuterHtml { get; set; } = new();

        public List<string> Links { get; set; } = new();

        public int Score { get; set; } = 0;

        public bool IsAppliedTo { get; set; } = false;

        public bool IsInterviewing { get; set; } = false;

        public bool IsRejected { get; set; } = false;
    }
}
