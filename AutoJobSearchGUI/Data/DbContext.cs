using AutoJobSearchGUI.Models;
using AutoJobSearchShared;
using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Data
{
    public class DbContext : IDbContext
    {
        private readonly IDbContext _dbContext;

        public DbContext()
        {
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
            // TODO: proper logging
            await _dbContext.UpdateJobSearchProfileStringProperty(columnName, value, id);
        }

        public async Task<JobSearchProfile> CreateNewJobSearchProfile(JobSearchProfile profile)
        {
            return await _dbContext.CreateNewJobSearchProfile(profile);
        }

        public async Task<IQueryable<JobListing>> ExecuteJobBoardAdvancedQuery(
            bool isAppliedTo,
            bool isInterviewing,
            bool isRejected,
            bool isFavourite)
        {
            // TODO: proper logging
            return await _dbContext.ExecuteJobBoardAdvancedQuery(isAppliedTo, isInterviewing, isRejected, isFavourite);
        }

        public async Task<IEnumerable<JobListing>> GetAllJobListings()
        {
            return await _dbContext.GetAllJobListings();
        }

        public async Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfiles()
        {
            return await _dbContext.GetAllJobSearchProfiles();
        }

        public async Task<IEnumerable<JobListing>> GetFavouriteJobListings()
        {
            return await _dbContext.GetFavouriteJobListings();
        }

        public async Task<IEnumerable<JobListing>> GetHiddenJobListings()
        {
            return await _dbContext.GetHiddenJobListings();
        }

        public async Task<JobListing> GetJobListingDetails(int id)
        {
            return await _dbContext.GetJobListingDetails(id);
        }

        public async Task UpdateJobListingBoolProperty(JobListingsBoolField columnName, bool value, int id)
        {
            // TODO: proper logging
            await _dbContext.UpdateJobListingBoolProperty(columnName, value, id);
        }

        public async Task UpdateJobListingStringProperty(JobListingsStringField columnName, string value, int id)
        {
            // TODO: proper logging
            await _dbContext.UpdateJobListingStringProperty(columnName, value, id);
        }

        public async Task DeleteJobSearchProfile(int id)
        {
            await _dbContext.DeleteJobSearchProfile(id);
        }
    }
}
