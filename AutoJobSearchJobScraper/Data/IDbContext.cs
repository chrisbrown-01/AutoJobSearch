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
        Task AddJobListingsAndApplicationLinksToDb(IEnumerable<JobListing> jobListings);
    }
}
