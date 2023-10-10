using AutoJobSearchShared.Models;
using HtmlAgilityPack;

namespace AutoJobSearchJobScraper.WebScraper
{
    internal interface IWebScraper
    {
        // List<JobListing> ExtractJobListingsFromLiElements(IEnumerable<HtmlNode> liElements, string searchTerm);
        Task<List<JobListing>> ScrapeJobs(IEnumerable<string> searchTerms);
    }
}