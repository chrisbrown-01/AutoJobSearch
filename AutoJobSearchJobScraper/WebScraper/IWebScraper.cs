using AutoJobSearchShared.Models;
using HtmlAgilityPack;

namespace AutoJobSearchJobScraper.WebScraper
{
    internal interface IWebScraper
    {
        Task<List<JobListing>> ScrapeJobs(IEnumerable<string> searchTerms);
    }
}