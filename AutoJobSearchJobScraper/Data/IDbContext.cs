using AutoJobSearchShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchJobScraper.Data
{
    internal interface IDbContext 
    {
        Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync();
        Task<JobSearchProfile?> GetJobSearchProfileByIdAsync(int id);

        Task<IEnumerable<string>> GetAllApplicationLinks(); // TODO: rename all methods to have async in name

        Task SaveJobListings(IEnumerable<JobListing> jobListings);
    }
}
