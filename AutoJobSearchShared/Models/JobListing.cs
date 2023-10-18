using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchShared.Models
{
    public class JobListing
    {
        public JobListing()
        {
            
        }

        // TODO: delete
        //public JobListing(
        //    long Id, 
        //    string SearchTerm, 
        //    string CreatedAt, 
        //    byte[] Description, 
        //    long Score, 
        //    long IsAppliedTo, 
        //    long IsInterviewing, 
        //    long IsRejected, 
        //    long IsFavourite, 
        //    long IsHidden)
        //{
        //    this.Id = (int)Id;
        //    this.SearchTerm = SearchTerm;
        //    this.CreatedAt = DateTime.Parse(CreatedAt);
        //    this.Description = Encoding.UTF8.GetString(Description);
        //    this.Score = (int)Score;
        //    this.IsAppliedTo = IsAppliedTo == 1;
        //    this.IsInterviewing = IsInterviewing == 1;
        //    this.IsRejected = IsRejected == 1;
        //    this.IsFavourite = IsFavourite == 1;
        //    this.IsHidden = IsHidden == 1;
        //}

        public int Id { get; }
        public string SearchTerm { get; set; } = string.Empty;  

        public DateTime CreatedAt { get; } = DateTime.Now;

        public string Description_Raw { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<ApplicationLink> ApplicationLinks { get; set; } = new();

        public string ApplicationLinksString { get; set; } = string.Empty;

        public int Score { get; set; } = 0;

        public bool IsAppliedTo { get; set; } = false;

        public bool IsInterviewing { get; set; } = false;

        public bool IsRejected { get; set; } = false;

        public bool IsFavourite { get; set; } = false;

        public bool IsHidden { get; set; } = false;

        public string Notes { get; set; } = string.Empty;
    }
}
