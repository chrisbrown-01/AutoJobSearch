using AutoJobSearchShared;
using AutoJobSearchShared.Models;
using Microsoft.Extensions.Logging;


namespace AutoJobSearchJobScraper.Data
{
    internal class SQLiteDbContext : IDbContext
    {
        private readonly ILogger<SQLiteDbContext> _logger;   
        public SQLiteDbContext(ILogger<SQLiteDbContext> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<string>> GetAllApplicationLinks()
        {
            _logger.LogInformation("Getting all application links.");
            return await SQLiteDb.GetAllApplicationLinks();
        }

        public async Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync()
        {
            _logger.LogInformation("Getting all job search profiles.");
            return await SQLiteDb.GetAllJobSearchProfilesAsync();
        }

        public async Task<JobSearchProfile?> GetJobSearchProfileByIdAsync(int id)
        {
            _logger.LogInformation("Getting job search profile for ID {@id}", id);
            return await SQLiteDb.GetJobSearchProfileByIdAsync(id);
        }

        public async Task SaveJobListings(IEnumerable<JobListing> jobListings)
        {
            _logger.LogInformation("Saving {@jobListings.Count} new job listings.", jobListings.Count());
            await SQLiteDb.SaveJobListings(jobListings);
        }
    }
}
