using AutoJobSearchGUI.Models;
using AutoJobSearchShared;
using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace AutoJobSearchGUI.Data
{
    // At the time of creating this project, there is no official documentation on how to perform dependency injection in Avalonia.
    // Therefore the purpose of this class is to act as the single point of change if the user wants to convert to using a 
    // different database.

    public class DbContext : IDbContext
    {
        private readonly IDbContext _dbContext;

        public DbContext()
        {
            Log.Information("Creating new DbContext object using SQLite provider.");
            _dbContext = new SQLiteDbContext();

            JobListingModel.BoolFieldChanged += async (sender, e) =>
            {
                await UpdateJobListingBoolPropertyAsync(e.Field, e.Value, e.Id);
            };

            JobListingModel.StringFieldChanged += async (sender, e) =>
            {
                await UpdateJobListingStringPropertyAsync(e.Field, e.Value, e.Id);
            };

            JobSearchProfileModel.StringFieldChanged += async (sender, e) =>
            {
                await UpdateJobSearchProfileStringPropertyAsync(e.Field, e.Value, e.Id);
            };
        }

        public async Task UpdateJobSearchProfileStringPropertyAsync(JobSearchProfilesStringField columnName, string value, int id)
        {
            await _dbContext.UpdateJobSearchProfileStringPropertyAsync(columnName, value, id);
        }

        public async Task<JobSearchProfile> CreateJobSearchProfileAsync(JobSearchProfile profile)
        {
            Log.Information("Creating new job search profile in database.");
            return await _dbContext.CreateJobSearchProfileAsync(profile);
        }

        public async Task<IQueryable<JobListing>> ExecuteJobListingQueryAsync(
            bool isAppliedTo,
            bool isInterviewing,
            bool isRejected,
            bool isFavourite)
        {
            Log.Information("Executing job board advanced query against database.");
            return await _dbContext.ExecuteJobListingQueryAsync(isAppliedTo, isInterviewing, isRejected, isFavourite);
        }

        public async Task<IEnumerable<JobListing>> GetAllJobListingsAsync()
        {
            Log.Information("Getting all job listings from database.");
            return await _dbContext.GetAllJobListingsAsync();
        }

        public async Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync()
        {
            Log.Information("Getting all job search profiles from database.");
            return await _dbContext.GetAllJobSearchProfilesAsync();
        }

        public async Task<IEnumerable<JobListing>> GetFavouriteJobListingsAsync()
        {
            Log.Information("Getting favourite job listings from database.");
            return await _dbContext.GetFavouriteJobListingsAsync();
        }

        public async Task<IEnumerable<JobListing>> GetHiddenJobListingsAsync()
        {
            Log.Information("Getting hidden job listings from database.");
            return await _dbContext.GetHiddenJobListingsAsync();
        }

        public async Task<JobListing> GetJobListingDetailsByIdAsync(int id)
        {
            Log.Information("Getting job listing details from database for {@id}", id);
            return await _dbContext.GetJobListingDetailsByIdAsync(id);
        }

        public async Task UpdateJobListingBoolPropertyAsync(JobListingsBoolField columnName, bool value, int id)
        {
            Log.Information(
                "Updating {@columnName} field for job listing {@id} to {@value}.",
                columnName, id, value);

            await _dbContext.UpdateJobListingBoolPropertyAsync(columnName, value, id);
        }

        public async Task UpdateJobListingStringPropertyAsync(JobListingsStringField columnName, string value, int id)
        {
            await _dbContext.UpdateJobListingStringPropertyAsync(columnName, value, id);
        }

        public async Task DeleteJobSearchProfileAsync(int id)
        {
            Log.Information("Deleting job search profile for {@id}", id);
            await _dbContext.DeleteJobSearchProfileAsync(id);
        }

        public async Task DeleteAllJobListingsAsync()
        {
            Log.Information("Deleting all job listings and application links.");
            await _dbContext.DeleteAllJobListingsAsync();
        }

        public void Dispose()
        {
            Log.Information("Disposing of _dbContext connection.");
            _dbContext.Dispose();
        }
    }
}
