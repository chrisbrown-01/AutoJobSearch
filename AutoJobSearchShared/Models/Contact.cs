namespace AutoJobSearchShared.Models
{
    public class Contact
    {
        public Contact()
        {
        }

        public string Company { get; set; } = string.Empty;
        public DateTime CreatedAt { get; } = DateTime.Now;
        public string Email { get; set; } = string.Empty;
        public int Id { get; }
        public string LinkedIn { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}