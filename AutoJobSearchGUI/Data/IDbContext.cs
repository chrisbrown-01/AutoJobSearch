using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Data
{
    public interface IDbContext
    {
        Task DeleteJobSearchProfile(int id);

        Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync();

        Task<JobSearchProfile> CreateJobSearchProfile(JobSearchProfile profile);

        Task UpdateJobListingBoolProperty(JobListingsBoolField columnName, bool value, int id);
        Task UpdateJobListingStringProperty(JobListingsStringField columnName, string value, int id);

        Task UpdateJobSearchProfileStringProperty(JobSearchProfilesStringField columnName, string value, int id);

        Task<IQueryable<JobListing>> ExecuteJobBoardAdvancedQuery(
            bool isAppliedTo,
            bool isInterviewing,
            bool isRejected,
            bool isFavourite);

        Task<IEnumerable<JobListing>> GetFavouriteJobListings();

        Task<IEnumerable<JobListing>> GetHiddenJobListings();

        Task<IEnumerable<JobListing>> GetAllJobListings();

        Task<JobListing> GetJobListingDetails(int id);
    }
}