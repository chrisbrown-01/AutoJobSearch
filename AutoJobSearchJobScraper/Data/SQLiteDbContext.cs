using AutoJobSearchShared;
using AutoJobSearchShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchJobScraper.Data
{
    internal class SQLiteDbContext : IDbContext
    {
        public async Task<IEnumerable<string>> GetAllApplicationLinks()
        {
            return await SQLiteDb.GetAllApplicationLinks();
        }

        public async Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync()
        {
            return await SQLiteDb.GetAllJobSearchProfilesAsync();
        }

        public async Task<JobSearchProfile?> GetJobSearchProfileByIdAsync(int id)
        {
            return await SQLiteDb.GetJobSearchProfileByIdAsync(id);
        }

        public async Task SaveJobListings(IEnumerable<JobListing> jobListings)
        {
            await SQLiteDb.SaveJobListings(jobListings);
        }
    }
}
