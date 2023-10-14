using AutoJobSearchShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchJobScraper.Data
{
    internal interface IDbContext : IDisposable
    {
        Task<JobSearchProfile?> GetJobSearchProfileByIdAsync(int id);

        Task<IEnumerable<string>> GetAllApplicationLinksAsync();

        Task SaveJobListingsAsync(IEnumerable<JobListing> jobListings);
    }
}
