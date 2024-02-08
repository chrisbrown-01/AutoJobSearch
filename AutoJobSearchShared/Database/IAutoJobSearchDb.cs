using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;

namespace AutoJobSearchShared.Database
{
    public interface IAutoJobSearchDb : IDisposable
    {
        Task<JobSearchProfile> CreateJobSearchProfileAsync(JobSearchProfile profile);
        Task DeleteAllJobListingsAsync();
        Task DeleteJobSearchProfileAsync(int id);
        Task<IQueryable<JobListing>> ExecuteJobListingQueryAsync(bool columnFiltersEnabled, bool isAppliedTo, bool isInterviewing, bool isRejected, bool isFavourite);
        Task<IEnumerable<string>> GetAllApplicationLinksAsync();
        Task<IEnumerable<JobListing>> GetAllJobListingsAsync();
        Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync();
        Task<IEnumerable<JobListing>> GetFavouriteJobListingsAsync();
        Task<IEnumerable<JobListing>> GetHiddenJobListingsAsync();
        Task<JobListing> GetJobListingDetailsByIdAsync(int id);
        Task<JobSearchProfile?> GetJobSearchProfileByIdAsync(int id);
        Task SaveJobListingsAsync(IEnumerable<JobListing> jobListings);
        Task UpdateJobListingBoolPropertyAsync(JobListingsBoolField columnName, bool value, int id);
        Task UpdateJobListingStringPropertyAsync(JobListingsStringField columnName, string value, int id);
        Task UpdateJobSearchProfileStringPropertyAsync(JobSearchProfilesStringField columnName, string value, int id);
        Task<IEnumerable<Contact>> GetAllContactsAsync();
        Task<Contact> CreateNewContactAsync(Contact contact);
        Task DeleteContactAsync(int id);
        Task UpdateContactStringPropertyAsync(ContactStringField columnName, string value, int id);
    }
}