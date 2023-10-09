using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Data
{
    internal interface IDbContext
    {
        Task UpdateJobListingBoolProperty(DbBoolField columnName, bool value, int id);
        Task UpdateJobListingStringProperty(DbStringField columnName, string value, int id);

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