namespace AutoJobSearchShared.Models
{
    public class JobSearchProfile
    {
        public JobSearchProfile()
        {
        }

        public int Id { get; }

        public string KeywordsNegative { get; set; } = string.Empty;
        public string KeywordsPositive { get; set; } = string.Empty;
        public int MaxJobListingIndex { get; set; } = 150;

        public string ProfileName { get; set; } = "New Profile";

        public string Searches { get; set; } = string.Empty;
        public string SentimentsNegative { get; set; } = string.Empty;
        public string SentimentsPositive { get; set; } = string.Empty;
    }
}