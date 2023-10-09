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
    public class SQLiteDbContext : IDbContext
    {
        public async Task<JobSearchProfile> CreateNewJobSearchProfile(JobSearchProfile profile)
        {
            return await SQLiteDb.CreateNewJobSearchProfile(profile);
        }

        public async Task DeleteJobSearchProfile(int id)
        {
            await SQLiteDb.DeleteJobSearchProfile(id);
        }

        public async Task<IQueryable<JobListing>> ExecuteJobBoardAdvancedQuery(
            bool isAppliedTo, 
            bool isInterviewing, 
            bool isRejected, 
            bool isFavourite)
        {
            return await SQLiteDb.ExecuteJobBoardAdvancedQuery(isAppliedTo, isInterviewing, isRejected, isFavourite);
        }

        public async Task<IEnumerable<JobListing>> GetAllJobListings()
        {
            return await SQLiteDb.GetAllJobListings();
        }

        public async Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfiles()
        {
            return await SQLiteDb.GetAllJobSearchProfiles();
        }

        public async Task<IEnumerable<JobListing>> GetFavouriteJobListings()
        {
            return await SQLiteDb.GetFavouriteJobListings();
        }

        public async Task<IEnumerable<JobListing>> GetHiddenJobListings()
        {
            return await SQLiteDb.GetHiddenJobListings();
        }

        public async Task<JobListing> GetJobListingDetails(int id)
        {
            return await SQLiteDb.GetJobListingDetails(id);
        }

        public async Task UpdateJobListingBoolProperty(JobListingsBoolField columnName, bool value, int id)
        {
            await SQLiteDb.UpdateJobListingBoolProperty(columnName, value, id);
        }

        public async Task UpdateJobListingStringProperty(JobListingsStringField columnName, string value, int id)
        {
            await SQLiteDb.UpdateJobListingStringProperty(columnName, value, id);
        }

        public async Task UpdateJobSearchProfileStringProperty(JobSearchProfilesStringField columnName, string value, int id)
        {
            await SQLiteDb.UpdateJobSearchProfileStringProperty(columnName, value, id);
        }
    }
}
