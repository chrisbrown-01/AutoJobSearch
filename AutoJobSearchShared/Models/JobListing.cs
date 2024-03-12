namespace AutoJobSearchShared.Models
{
    public class JobListing
    {
        public JobListing()
        {
        }

        public List<ApplicationLink> ApplicationLinks { get; set; } = new();
        public string ApplicationLinksString { get; set; } = string.Empty;
        public DateTime CreatedAt { get; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public string Description_Raw { get; set; } = string.Empty;
        public int Id { get; }

        public bool IsAcceptedOffer { get; set; } = false;
        public bool IsAppliedTo { get; set; } = false;
        public bool IsDeclinedOffer { get; set; } = false;
        public bool IsFavourite { get; set; } = false;
        public bool IsHidden { get; set; } = false;
        public bool IsInterviewing { get; set; } = false;
        public bool IsNegotiating { get; set; } = false;
        public bool IsRejected { get; set; } = false;
        public bool IsToBeAppliedTo { get; set; } = false;
        public JobListingAssociatedFiles? JobListingAssociatedFiles { get; set; }
        public string Notes { get; set; } = string.Empty;
        public int Score { get; set; } = 0;
        public string SearchTerm { get; set; } = string.Empty;

        // Keeps track of the most recent time a bool property was changed.
        public DateTime StatusModifiedAt { get; } = DateTime.Now;
    }
}