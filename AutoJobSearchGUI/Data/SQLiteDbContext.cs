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

        public async Task<ContactAssociatedJobId> CreateContactAssociatedJobIdAsync(int contactId, int jobId)
        {
            return await _sqliteDb.CreateContactAssociatedJobIdAsync(contactId, jobId);
        }

        public async Task<JobListing> CreateJobAsync()
        {
            return await _sqliteDb.CreateJobAsync();
        }

        public async Task CreateJobListingAssociatedFilesAsync(JobListingAssociatedFiles jobListingAssociatedFiles)
        {
            await _sqliteDb.CreateJobListingAssociatedFilesAsync(jobListingAssociatedFiles);
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

        public async Task DeleteContactAssociatedJobIdAsync(int contactId, int jobId)
        {
            await _sqliteDb.DeleteContactAssociatedJobIdAsync(contactId, jobId);
        }

        public async Task DeleteContactAsync(int id)
        {
            await _sqliteDb.DeleteContactAsync(id);
        }

        public async Task DeleteJobAsync(int jobId)
        {
            await _sqliteDb.DeleteJobAsync(jobId);
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
            bool descriptionFilterEnabled,
            bool notesFilterEnabled,
            bool columnFilterEnabled,
            bool isToBeAppliedTo,
            bool isAppliedTo,
            bool isInterviewing,
            bool isNegotiating,
            bool isRejected,
            bool isDeclinedOffer,
            bool isAcceptedOffer,
            bool isFavourite)
        {
            return await _sqliteDb.ExecuteJobListingQueryAsync(
                descriptionFilterEnabled,
                notesFilterEnabled,
                columnFilterEnabled,
                isToBeAppliedTo,
                isAppliedTo,
                isInterviewing,
                isNegotiating,
                isRejected,
                isDeclinedOffer,
                isAcceptedOffer,
                isFavourite);
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

        public async Task UpdateJobListingAssociatedFilesAsync(JobListingAssociatedFiles jobListingAssociatedFiles)
        {
            await _sqliteDb.UpdateJobListingAssociatedFilesAsync(jobListingAssociatedFiles);
        }

        public async Task UpdateJobListingBoolPropertyAsync(JobListingsBoolField columnName, bool value, int id, DateTime statusModifiedAt)
        {
            await _sqliteDb.UpdateJobListingBoolPropertyAsync(columnName, value, id, statusModifiedAt);
        }

        public async Task UpdateJobListingIntPropertyAsync(JobListingsIntField columnName, int value, int id)
        {
            await _sqliteDb.UpdateJobListingIntPropertyAsync(columnName, value, id);
        }

        public async Task UpdateJobListingStringPropertyAsync(JobListingsStringField columnName, string value, int id)
        {
            await _sqliteDb.UpdateJobListingStringPropertyAsync(columnName, value, id);
        }

        public async Task UpdateJobSearchProfileIntPropertyAsync(JobSearchProfilesIntField columnName, int value, int id)
        {
            await _sqliteDb.UpdateJobSearchProfileIntPropertyAsync(columnName, value, id);
        }

        public async Task UpdateJobSearchProfileStringPropertyAsync(JobSearchProfilesStringField columnName, string value, int id)
        {
            await _sqliteDb.UpdateJobSearchProfileStringPropertyAsync(columnName, value, id);
        }
    }
}
