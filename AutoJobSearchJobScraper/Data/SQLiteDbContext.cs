using AutoJobSearchShared.Database;
using AutoJobSearchShared.Models;
using Microsoft.Extensions.Logging;


namespace AutoJobSearchJobScraper.Data
{
    internal class SQLiteDbContext : IDbContext
    {
        private readonly ILogger<SQLiteDbContext> _logger;
        private readonly SQLiteDb _sqliteDb;

        public SQLiteDbContext(ILogger<SQLiteDbContext> logger)
        {
            _logger = logger;
            _sqliteDb = new SQLiteDb();
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing of SQLite database connection.");
            _sqliteDb.Dispose();
        }

        public async Task<IEnumerable<string>> GetAllApplicationLinksAsync()
        {
            _logger.LogInformation("Getting all application links.");
            return await _sqliteDb.GetAllApplicationLinksAsync();
        }

        public async Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync()
        {
            _logger.LogInformation("Getting all job search profiles.");
            return await _sqliteDb.GetAllJobSearchProfilesAsync();
        }

        public async Task<JobSearchProfile?> GetJobSearchProfileByIdAsync(int id)
        {
            _logger.LogInformation("Getting job search profile for ID {@id}", id);
            return await _sqliteDb.GetJobSearchProfileByIdAsync(id);
        }

        public async Task SaveJobListingsAsync(IEnumerable<JobListing> jobListings)
        {
            _logger.LogInformation("Saving {@jobListings.Count} new job listings.", jobListings.Count());
            await _sqliteDb.SaveJobListingsAsync(jobListings);
        }
    }
}
