﻿using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Data
{
    public interface IDbContext : IDisposable
    {
        Task DeleteAllJobListingsAsync();
        Task DeleteJobSearchProfileAsync(int id);
        Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync();
        Task<JobSearchProfile> CreateJobSearchProfileAsync(JobSearchProfile profile);
        Task UpdateJobListingBoolPropertyAsync(JobListingsBoolField columnName, bool value, int id, DateTime statusModifiedAt);
        Task UpdateJobListingStringPropertyAsync(JobListingsStringField columnName, string value, int id);
        Task UpdateJobSearchProfileStringPropertyAsync(JobSearchProfilesStringField columnName, string value, int id);
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
        Task<IEnumerable<JobListing>> GetFavouriteJobListingsAsync();
        Task<IEnumerable<JobListing>> GetHiddenJobListingsAsync();
        Task<IEnumerable<JobListing>> GetAllJobListingsAsync();
        Task<JobListing> GetJobListingDetailsByIdAsync(int id);
        Task<IEnumerable<Contact>> GetAllContactsAsync();
        Task<Contact> CreateNewContactAsync(Contact contact);
        Task DeleteContactAsync(int id);
        Task UpdateContactStringPropertyAsync(ContactStringField columnName, string value, int id);
        Task DeleteAllContactsAsync();
        Task<IEnumerable<ContactAssociatedJobId>> GetAllContactsAssociatedJobIdsAsync();
        Task<ContactAssociatedJobId> CreateContactAssociatedJobIdAsync(int contactId, int jobId);
        Task DeleteContactAssociatedJobIdAsync(int contactId, int jobId);
        Task DeleteJobAsync(int jobId);
        Task<JobListing> CreateJobAsync();
        Task UpdateJobListingIntPropertyAsync(JobListingsIntField columnName, int value, int id);
        Task UpdateJobSearchProfileIntPropertyAsync(JobSearchProfilesIntField columnName, int value, int id);
        Task CreateJobListingAssociatedFilesAsync(JobListingAssociatedFiles jobListingAssociatedFiles);
        Task UpdateJobListingAssociatedFilesAsync(JobListingAssociatedFiles jobListingAssociatedFiles);
    }
}