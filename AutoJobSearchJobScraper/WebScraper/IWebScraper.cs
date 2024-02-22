using AutoJobSearchShared.Models;
using HtmlAgilityPack;

namespace AutoJobSearchJobScraper.WebScraper
{
    internal interface IWebScraper
    {
        Task<IEnumerable<JobListing>> ScrapeJobsAsync(IEnumerable<string> searchTerms, int? maxJobListingIndex);
    }
}