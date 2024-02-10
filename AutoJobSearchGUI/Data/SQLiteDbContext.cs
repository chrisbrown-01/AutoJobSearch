using AutoJobSearchShared.Database;
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
            _sqliteDb = new SQLiteDb();
        }

        public async Task<ContactAssociatedJobId> CreateContactAssociatedJobId(int contactId, int jobId)
        {
            return await _sqliteDb.CreateContactAssociatedJobId(contactId, jobId);
        }

        public async Task<JobSearchProfile> CreateJobSearchProfileAsync(JobSearchProfile profile)
        {
            return await _sqliteDb.CreateJobSearchProfileAsync(profile);
        }

        public async Task<Contact> CreateNewContactAsync(Contact contact)
        {
            return await _sqliteDb.CreateNewContactAsync(contact);
        }

        public async Task DeleteAllContactsAsync()
        {
            await _sqliteDb.DeleteAllContactsAsync();
        }

        public async Task DeleteAllJobListingsAsync()
        {
            await _sqliteDb.DeleteAllJobListingsAsync();
        }

        public async Task DeleteContactAsync(int id)
        {
            await _sqliteDb.DeleteContactAsync(id);
        }

        public async Task DeleteJobSearchProfileAsync(int id)
        {
            await _sqliteDb.DeleteJobSearchProfileAsync(id);
        }

        public void Dispose()
        {
            _sqliteDb.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<IQueryable<JobListing>> ExecuteJobListingQueryAsync(
            bool columnFiltersEnabled,
            bool isAppliedTo,
            bool isInterviewing,
            bool isRejected,
            bool isFavourite)
        {
            return await _sqliteDb.ExecuteJobListingQueryAsync(columnFiltersEnabled, isAppliedTo, isInterviewing, isRejected, isFavourite);
        }

        public async Task<IEnumerable<ContactAssociatedJobId>> GetAllContactsAssociatedJobIdsAsync()
        {
            return await _sqliteDb.GetAllContactsAssociatedJobIdsAsync();
        }

        public async Task<IEnumerable<Contact>> GetAllContactsAsync()
        {
            return await _sqliteDb.GetAllContactsAsync();
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

        public async Task UpdateContactStringPropertyAsync(ContactStringField columnName, string value, int id)
        {
            await _sqliteDb.UpdateContactStringPropertyAsync(columnName, value, id);
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
