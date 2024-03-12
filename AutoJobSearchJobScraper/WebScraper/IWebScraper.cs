using AutoJobSearchShared.Models;

namespace AutoJobSearchJobScraper.WebScraper
{
    internal interface IWebScraper
    {
        Task<IEnumerable<JobListing>> ScrapeJobsAsync(IEnumerable<string> searchTerms, int? maxJobListingIndex);
    }
}