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
        Task DeleteAllJobListingsAsync();

        Task DeleteJobSearchProfileAsync(int id);

        Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync();

        Task<JobSearchProfile> CreateJobSearchProfileAsync(JobSearchProfile profile);

        Task UpdateJobListingBoolPropertyAsync(JobListingsBoolField columnName, bool value, int id);
        Task UpdateJobListingStringPropertyAsync(JobListingsStringField columnName, string value, int id);

        Task UpdateJobSearchProfileStringPropertyAsync(JobSearchProfilesStringField columnName, string value, int id);

        Task<IQueryable<JobListing>> ExecuteJobListingQueryAsync(
            bool columnFiltersEnabled,
            bool isAppliedTo,
            bool isInterviewing,
            bool isRejected,
            bool isFavourite);

        Task<IEnumerable<JobListing>> GetFavouriteJobListingsAsync();

        Task<IEnumerable<JobListing>> GetHiddenJobListingsAsync();

        Task<IEnumerable<JobListing>> GetAllJobListingsAsync();

        Task<JobListing> GetJobListingDetailsByIdAsync(int id);

        Task<IEnumerable<Contact>> GetAllContactsAsync();

        Task<Contact> CreateNewContactAsync(Contact contact);
    }
}