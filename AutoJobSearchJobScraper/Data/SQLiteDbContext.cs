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
            _logger.LogDebug("Initializing SQLiteDbContext logger.");
        }

        public async Task<IEnumerable<string>> GetAllApplicationLinks()
        {
            //Log.Debug("testing logger in SQLite Db get all application links");
            return await SQLiteDb.GetAllApplicationLinks();
        }

        public async Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync()
        {
            //Log.Debug("testing logger in SQLite Db get all job search profiles");
            return await SQLiteDb.GetAllJobSearchProfilesAsync();
        }

        public async Task<JobSearchProfile?> GetJobSearchProfileByIdAsync(int id)
        {
            //Log.Debug("testing logger in SQLite Db get job search profile by id");
            return await SQLiteDb.GetJobSearchProfileByIdAsync(id);
        }

        public async Task SaveJobListings(IEnumerable<JobListing> jobListings)
        {
            //Log.Debug("testing logger in SQLite Db save job listings");
            await SQLiteDb.SaveJobListings(jobListings);
        }
    }
}
