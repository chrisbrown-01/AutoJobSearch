using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutoJobSearchConsoleApp.Models
{
    public class JobListing
    {
        public int Id { get; }
        public string SearchTerm { get; set; } = "FOUND BY SEARCH TERM: ___"; // TODO: update value

        public DateTime CreatedAt { get; } = DateTime.Now;

        public string Description_Raw { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<ApplicationLink> ApplicationLinks { get; set; } = new();

        public int Score { get; set; } = 0;

        public bool IsAppliedTo { get; set; } = false;

        public bool IsInterviewing { get; set; } = false;

        public bool IsRejected { get; set; } = false;

        public bool IsFavourite { get; set; } = false;

        public string Notes { get; set; } = string.Empty;

        // TODO: add "pinned/favourited" property
    }
}
