﻿using AutoJobSearchShared;
using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Data
{
    public class SQLiteDbContext : IDbContext
    {
        private readonly SQLiteDb _sqliteDb;

        public SQLiteDbContext()
        {
            _sqliteDb = new SQLiteDb(); // TODO: disposing
        }

        public async Task<JobSearchProfile> CreateJobSearchProfileAsync(JobSearchProfile profile)
        {
            return await _sqliteDb.CreateJobSearchProfileAsync(profile);
        }

        public async Task DeleteJobSearchProfileAsync(int id)
        {
            // TODO: throw exception inside these methods to see how it reacts
            await _sqliteDb.DeleteJobSearchProfileAsync(id);
        }

        public async Task<IQueryable<JobListing>> ExecuteJobListingQueryAsync(
            bool isAppliedTo, 
            bool isInterviewing, 
            bool isRejected, 
            bool isFavourite)
        {
            return await _sqliteDb.ExecuteJobListingQueryAsync(isAppliedTo, isInterviewing, isRejected, isFavourite);
        }

        public async Task<IEnumerable<JobListing>> GetAllJobListingsAsync()
        {
            return await _sqliteDb.GetAllJobListingsAsync();
        }

        public async Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync()
        {
            return await _sqliteDb.GetAllJobSearchProfilesAsync();
        }

        public async Task<IEnumerable<JobListing>> GetFavouriteJobListingsAsync()
        {
            return await _sqliteDb.GetFavouriteJobListingsAsync();
        }

        public async Task<IEnumerable<JobListing>> GetHiddenJobListingsAsync()
        {
            return await _sqliteDb.GetHiddenJobListingsAsync();
        }

        public async Task<JobListing> GetJobListingDetailsByIdAsync(int id) 
        {
            return await _sqliteDb.GetJobListingDetailsByIdAsync(id);
        }

        public async Task UpdateJobListingBoolPropertyAsync(JobListingsBoolField columnName, bool value, int id)
        {
            await _sqliteDb.UpdateJobListingBoolPropertyAsync(columnName, value, id);
        }

        public async Task UpdateJobListingStringPropertyAsync(JobListingsStringField columnName, string value, int id)
        {
            await _sqliteDb.UpdateJobListingStringPropertyAsync(columnName, value, id);
        }

        public async Task UpdateJobSearchProfileStringPropertyAsync(JobSearchProfilesStringField columnName, string value, int id)
        {
            await _sqliteDb.UpdateJobSearchProfileStringPropertyAsync(columnName, value, id);
        }
    }
}
