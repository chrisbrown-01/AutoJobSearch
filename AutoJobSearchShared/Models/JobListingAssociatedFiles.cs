namespace AutoJobSearchShared.Models
{
    public class JobListingAssociatedFiles
    {
        public JobListingAssociatedFiles()
        {
        }

        public string CoverLetter { get; set; } = string.Empty;
        public string File1 { get; set; } = string.Empty;
        public string File2 { get; set; } = string.Empty;
        public string File3 { get; set; } = string.Empty;
        public int Id { get; set; }

        public string Resume { get; set; } = string.Empty;
    }
}