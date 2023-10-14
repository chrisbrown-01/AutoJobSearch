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
    public class DbContext : IDbContext
    {
        private readonly IDbContext _dbContext;

        public DbContext()
        {
            Log.Information("Creating new DbContext object using SQLite provider.");
            _dbContext = new SQLiteDbContext();

            JobListingModel.BoolFieldChanged += async (sender, e) =>
            {
                await UpdateJobListingBoolProperty(e.Field, e.Value, e.Id);
            };

            JobListingModel.StringFieldChanged += async (sender, e) =>
            {
                await UpdateJobListingStringProperty(e.Field, e.Value, e.Id);
            };

            JobSearchProfileModel.StringFieldChanged += async (sender, e) =>
            {
                await UpdateJobSearchProfileStringProperty(e.Field, e.Value, e.Id);
            };
        }

        public async Task UpdateJobSearchProfileStringProperty(JobSearchProfilesStringField columnName, string value, int id)
        {
            await _dbContext.UpdateJobSearchProfileStringProperty(columnName, value, id);
        }

        public async Task<JobSearchProfile> CreateJobSearchProfile(JobSearchProfile profile)
        {
            Log.Information("Creating new job search profile in database.");
            return await _dbContext.CreateJobSearchProfile(profile);
        }

        public async Task<IQueryable<JobListing>> ExecuteJobBoardAdvancedQuery(
            bool isAppliedTo,
            bool isInterviewing,
            bool isRejected,
            bool isFavourite)
        {
            Log.Information("Executing job board advanced query against database.");
            return await _dbContext.ExecuteJobBoardAdvancedQuery(isAppliedTo, isInterviewing, isRejected, isFavourite);
        }

        public async Task<IEnumerable<JobListing>> GetAllJobListings()
        {
            Log.Information("Getting all job listings from database.");
            return await _dbContext.GetAllJobListings();
        }

        public async Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync()
        {
            Log.Information("Getting all job search profiles from database.");
            return await _dbContext.GetAllJobSearchProfilesAsync();
        }

        public async Task<IEnumerable<JobListing>> GetFavouriteJobListings()
        {
            Log.Information("Getting favourite job listings from database.");
            return await _dbContext.GetFavouriteJobListings();
        }

        public async Task<IEnumerable<JobListing>> GetHiddenJobListings()
        {
            Log.Information("Getting hidden job listings from database.");
            return await _dbContext.GetHiddenJobListings();
        }

        public async Task<JobListing> GetJobListingDetails(int id)
        {
            Log.Information("Getting job listing details from database for {@id}", id);
            return await _dbContext.GetJobListingDetails(id);
        }

        public async Task UpdateJobListingBoolProperty(JobListingsBoolField columnName, bool value, int id)
        {
            Log.Information(
                "Updating {@columnName} field for job listing {@id} to {@value}.",
                columnName, id, value);

            await _dbContext.UpdateJobListingBoolProperty(columnName, value, id);
        }

        public async Task UpdateJobListingStringProperty(JobListingsStringField columnName, string value, int id)
        {
            await _dbContext.UpdateJobListingStringProperty(columnName, value, id);
        }

        public async Task DeleteJobSearchProfile(int id)
        {
            Log.Information("Deleting job search profile for {@id}", id, this);
            await _dbContext.DeleteJobSearchProfile(id);
        }
    }
}
