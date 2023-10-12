using AutoJobSearchShared.Models;
using HtmlAgilityPack;

namespace AutoJobSearchJobScraper.WebScraper
{
    internal interface IWebScraper // TODO: create "DI" webScaper class
    {
        // List<JobListing> ExtractJobListingsFromLiElements(IEnumerable<HtmlNode> liElements, string searchTerm);
        Task<List<JobListing>> ScrapeJobs(IEnumerable<string> searchTerms);
    }
}