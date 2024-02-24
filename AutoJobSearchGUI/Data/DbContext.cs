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
                await UpdateJobListingBoolPropertyAsync(e.Field, e.Value, e.Id, e.StatusModifiedAt);
            };

            JobListingModel.StringFieldChanged += async (sender, e) =>
            {
                await UpdateJobListingStringPropertyAsync(e.Field, e.Value, e.Id);
            };

            JobListingModel.IntFieldChanged += async (sender, e) =>
            {
                await UpdateJobListingIntPropertyAsync(e.Field, e.Value, e.Id);
            };

            JobSearchProfileModel.IntFieldChanged += async (sender, e) =>
            {
                await UpdateJobSearchProfileIntPropertyAsync(e.Field, e.Value, e.Id);
            };

            JobSearchProfileModel.StringFieldChanged += async (sender, e) =>
            {
                await UpdateJobSearchProfileStringPropertyAsync(e.Field, e.Value, e.Id);
            };

            ContactModel.StringFieldChanged += async (sender, e) =>
            {
                await UpdateContactStringPropertyAsync(e.Field, e.Value, e.Id);
            };
        }

        public async Task UpdateContactStringPropertyAsync(ContactStringField columnName, string value, int id)
        {
            await _dbContext.UpdateContactStringPropertyAsync(columnName, value, id);
        }

        public async Task UpdateJobSearchProfileIntPropertyAsync(JobSearchProfilesIntField columnName, int value, int id)
        {
            await _dbContext.UpdateJobSearchProfileIntPropertyAsync(columnName, value, id);
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
            bool columnFiltersEnabled,
            bool isToBeAppliedTo,
            bool isAppliedTo,
            bool isInterviewing,
            bool isNegotiating,
            bool isRejected,
            bool isDeclinedOffer,
            bool isAcceptedOffer,
            bool isFavourite)
        {
            Log.Information("Executing job board advanced query against database.");

            return await _dbContext.ExecuteJobListingQueryAsync(
                 columnFiltersEnabled,
                 isToBeAppliedTo,
                 isAppliedTo,
                 isInterviewing,
                 isNegotiating,
                 isRejected,
                 isDeclinedOffer,
                 isAcceptedOffer,
                 isFavourite);
        }

        public async Task<IEnumerable<JobListing>> GetAllJobListingsAsync()
        {
            Log.Information("Getting all job listings (shortened description) from database.");
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

        public async Task UpdateJobListingBoolPropertyAsync(JobListingsBoolField columnName, bool value, int id, DateTime statusModifiedAt)
        {
            Log.Information(
                "Updating {@columnName} field for job listing {@id} to {@value} at DateTime {@statusModifiedAt}.",
                columnName, id, value, statusModifiedAt);

            await _dbContext.UpdateJobListingBoolPropertyAsync(columnName, value, id, statusModifiedAt);
        }

        public async Task UpdateJobListingStringPropertyAsync(JobListingsStringField columnName, string value, int id)
        {
            await _dbContext.UpdateJobListingStringPropertyAsync(columnName, value, id);
        }

        public async Task UpdateJobListingIntPropertyAsync(JobListingsIntField columnName, int value, int id)
        {
            await _dbContext.UpdateJobListingIntPropertyAsync(columnName, value, id);
        }

        public async Task DeleteJobSearchProfileAsync(int id)
        {
            Log.Information("Deleting job search profile for {@id} from database.", id);
            await _dbContext.DeleteJobSearchProfileAsync(id);
        }

        public async Task DeleteAllJobListingsAsync()
        {
            Log.Information("Deleting all job listings and application links from database.");
            await _dbContext.DeleteAllJobListingsAsync();
        }

        public void Dispose()
        {
            Log.Information("Disposing of _dbContext connection.");
            _dbContext.Dispose();
        }

        public async Task<IEnumerable<Contact>> GetAllContactsAsync()
        {
            Log.Information("Getting all contacts from database.");
            return await _dbContext.GetAllContactsAsync();
        }

        public async Task<Contact> CreateNewContactAsync(Contact contact)
        {
            Log.Information("Creating new contact in database.");
            return await _dbContext.CreateNewContactAsync(contact);
        }

        public async Task DeleteContactAsync(int id)
        {
            Log.Information("Deleting contact for {@id} from database.", id);
            await _dbContext.DeleteContactAsync(id);
        }

        public async Task DeleteAllContactsAsync()
        {
            Log.Information("Deleting all contacts from database.");
            await _dbContext.DeleteAllContactsAsync();
        }

        public async Task<IEnumerable<ContactAssociatedJobId>> GetAllContactsAssociatedJobIdsAsync()
        {
            Log.Information("Getting all contacts associated job IDs from database.");
            return await _dbContext.GetAllContactsAssociatedJobIdsAsync();
        }

        public async Task<ContactAssociatedJobId> CreateContactAssociatedJobIdAsync(int contactId, int jobId)
        {
            Log.Information("Creating contact associated job ID record for contact ID {@contactId} and job ID {@jobId} in database.", contactId, jobId);
            return await _dbContext.CreateContactAssociatedJobIdAsync(contactId, jobId);
        }

        public async Task DeleteContactAssociatedJobIdAsync(int contactId, int jobId)
        {
            Log.Information("Deleting contact associated job ID record for contact ID {@contactId} and job ID {@jobId} in database.", contactId, jobId);
            await _dbContext.DeleteContactAssociatedJobIdAsync(contactId, jobId);
        }

        public async Task DeleteJobAsync(int id)
        {
            Log.Information("Deleting job listing for {@id} from database.", id);
            await _dbContext.DeleteJobAsync(id);
        }

        public async Task<JobListing> CreateJobAsync()
        {
            Log.Information("Creating new job listing in database.");
            return await _dbContext.CreateJobAsync();
        }
    }
}
