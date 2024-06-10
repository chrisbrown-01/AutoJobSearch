using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Data
{
    public interface IDbContext : IDisposable
    {
        Task<ContactAssociatedJobId> CreateContactAssociatedJobIdAsync(int contactId, int jobId);

        Task<Contact> CreateContactAsync(Contact contact);

        Task CreateJobListingAssociatedFilesAsync(JobListingAssociatedFiles jobListingAssociatedFiles);

        Task<JobListing> CreateJobListingAsync();

        Task<JobSearchProfile> CreateJobSearchProfileAsync(JobSearchProfile profile);

        Task DeleteAllContactsAsync();

        Task DeleteAllJobListingsAsync();

        Task DeleteContactAssociatedJobIdAsync(int contactId, int jobId);

        Task DeleteContactAsync(int id);

        Task DeleteJobListingAsync(int jobId);

        Task DeleteJobSearchProfileAsync(int id);

        Task<IQueryable<JobListing>> ExecuteJobListingQueryAsync(
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
            bool isFavourite);

        Task<IEnumerable<ContactAssociatedJobId>> GetAllContactsAssociatedJobIdsAsync();

        Task<IEnumerable<Contact>> GetAllContactsAsync();

        Task<IEnumerable<JobListing>> GetAllJobListingsAsync();

        Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync();

        Task<Contact> GetContactByIdAsync(int id);

        Task<IEnumerable<JobListing>> GetFavouriteJobListingsAsync();

        Task<IEnumerable<JobListing>> GetHiddenJobListingsAsync();

        Task<JobListing> GetJobListingByIdAsync(int id, bool isRetrievingAllDetails);

        Task UpdateContactStringPropertyAsync(ContactStringField columnName, string value, int id);

        Task UpdateJobListingAssociatedFilesAsync(JobListingAssociatedFiles jobListingAssociatedFiles);

        Task UpdateJobListingBoolPropertyAsync(JobListingsBoolField columnName, bool value, int id, DateTime statusModifiedAt);

        Task UpdateJobListingIntPropertyAsync(JobListingsIntField columnName, int value, int id);

        Task UpdateJobListingStringPropertyAsync(JobListingsStringField columnName, string value, int id);

        Task UpdateJobSearchProfileIntPropertyAsync(JobSearchProfilesIntField columnName, int value, int id);

        Task UpdateJobSearchProfileStringPropertyAsync(JobSearchProfilesStringField columnName, string value, int id);
    }
}