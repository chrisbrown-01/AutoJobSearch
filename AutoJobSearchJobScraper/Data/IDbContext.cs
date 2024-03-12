using AutoJobSearchShared.Models;

namespace AutoJobSearchJobScraper.Data
{
    internal interface IDbContext : IDisposable
    {
        Task<IEnumerable<string>> GetAllApplicationLinksAsync();

        Task<JobSearchProfile?> GetJobSearchProfileByIdAsync(int id);

        Task SaveJobListingsAsync(IEnumerable<JobListing> jobListings);
    }
}