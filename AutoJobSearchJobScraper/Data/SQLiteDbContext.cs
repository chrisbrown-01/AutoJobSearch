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

        public async Task SaveJobListings(IEnumerable<JobListing> jobListings)
        {
            await SQLiteDb.AddJobListingsAndApplicationLinksToDb(jobListings);
        }
    }
}
