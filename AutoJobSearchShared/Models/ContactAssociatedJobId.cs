namespace AutoJobSearchShared.Models
{
    public class ContactAssociatedJobId
    {
        public ContactAssociatedJobId()
        {
        }

        public int ContactId { get; set; }
        public int Id { get; }
        public int JobListingId { get; set; }
    }
}