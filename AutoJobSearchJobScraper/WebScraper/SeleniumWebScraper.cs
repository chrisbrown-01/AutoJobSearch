using AutoJobSearchShared.Helpers;
using AutoJobSearchShared.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoJobSearchJobScraper.WebScraper
{
    internal class SeleniumWebScraper : IWebScraper
    {
        private const string REGEX_URL_PATTERN = @"https?://[^\s""]+";

        private readonly int MAX_PAGE_INDEX;
        private readonly string STARTING_INDEX_KEY;
        private readonly string ENDING_INDEX_KEY;

        private readonly ILogger<SeleniumWebScraper> _logger;

        public SeleniumWebScraper(ILogger<SeleniumWebScraper> logger)
        {
            _logger = logger;
            _logger.LogDebug("Initializing SeleniumWebScraper logger.");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            var config = builder.Build();

            MAX_PAGE_INDEX = config.GetValue<int>("MAX_PAGE_INDEX");
            if (MAX_PAGE_INDEX < 1) throw new ArgumentException(); // TODO: custom arguments

            STARTING_INDEX_KEY = config.GetValue<string>("STARTING_INDEX_KEY") ?? throw new NullReferenceException();
            ENDING_INDEX_KEY = config.GetValue<string>("ENDING_INDEX_KEY") ?? throw new NullReferenceException();
        }

        // TODO: surround in try-catch so that results are still saved even if captcha kills selenium
        public async Task<List<JobListing>> ScrapeJobs(IEnumerable<string> searchTerms) 
        {
            _logger.LogInformation("Starting ScrapeJobs method. Number of members in searchTerms argument: {@searchTerms.Count}", searchTerms.Count());

            var jobListings = new List<JobListing>();
            var doc = new HtmlDocument();
            var driver = new ChromeDriver();

            try
            {
                foreach (var searchTerm in searchTerms)
                {
                    // TODO: place try block inside this loop instead?

                    for (int i = 0; i < MAX_PAGE_INDEX + 1; i += 10)
                    {
                        driver.Navigate().GoToUrl($"https://www.google.com/search?q={WebUtility.UrlEncode(searchTerm)}&sourceid=chrome&ie=UTF-8&ibp=htl;jobs&start={i}");
                        doc.LoadHtml(driver.PageSource);

                        var checkForCaptcha = doc?.DocumentNode?.InnerText;

                        if (checkForCaptcha != null && checkForCaptcha.Contains("detected unusual traffic", StringComparison.OrdinalIgnoreCase))
                        {
                            var input = "";

                            while (input != "CONTINUE")
                            {
                                Console.WriteLine("Solve the Google Chrome captcha then type in 'CONTINUE' to continue scraping: ");
                                input = Console.ReadLine();
                            }

                            doc!.LoadHtml(driver.PageSource);
                        }

                        var liElements = doc!.DocumentNode?.SelectNodes("//li")?.ToList();

                        if (liElements == null) break;

                        // TODO: comment and document what the code is doing
                        jobListings.AddRange(ExtractJobListingsFromLiElements(liElements, searchTerm));
                    }
                }
            }
            catch (Exception) // TODO: implement exception handling
            {
                throw;
            }
            finally
            {
                driver.Close();
                driver.Quit();
            }

            await Task.CompletedTask;
            return jobListings;
        }

        /// <summary>
        /// Extracts the job listing description and application links from the raw HTML list item element.
        /// </summary>
        /// <param name="liElements"></param>
        /// <param name="searchTerm">The search term that the web scraper used to find the HTML list item.</param>
        /// <returns></returns>
        private List<JobListing> ExtractJobListingsFromLiElements(IEnumerable<HtmlNode> liElements, string searchTerm) // TODO: accessibility
        {
            var jobList = new List<JobListing>();

            foreach (var li in liElements)
            {
                var listing = new JobListing();
                listing.SearchTerm = searchTerm;

                var links = li.Descendants("a").Where(a => a.InnerText.Contains("apply", StringComparison.OrdinalIgnoreCase));

                // TODO: refactor into methods
                foreach (var link in links)
                {
                    var applicationLink = new ApplicationLink
                    {
                        Link_RawHTML = link.OuterHtml
                    };

                    MatchCollection matches = Regex.Matches(link.OuterHtml, REGEX_URL_PATTERN);

                    if (matches.FirstOrDefault() != null) // TODO: exception handling?
                    {
                        applicationLink.Link = matches.First().Value;
                    }

                    // TODO: test this, create hashset instead?
                    // Prevent duplicate links from being saved to the object.
                    if (listing.ApplicationLinks.Where(x => x.Link.Equals(applicationLink.Link)).Any())
                        continue;

                    listing.ApplicationLinks.Add(applicationLink);
                }

                listing.Description_Raw = WebUtility.HtmlDecode(li.InnerText);

                // TODO: extract to method
                var startingIndex = listing.Description_Raw.IndexOf(STARTING_INDEX_KEY);
                var endingIndex = listing.Description_Raw.IndexOf(ENDING_INDEX_KEY);

                if (startingIndex != -1 && endingIndex != -1 && (endingIndex > startingIndex))
                {
                    try
                    {
                        listing.Description = listing.Description_Raw.Substring(startingIndex + STARTING_INDEX_KEY.Length, endingIndex - (startingIndex + STARTING_INDEX_KEY.Length));
                        listing.Description = StringHelpers.AddNewLinesToMisformedString(listing.Description);
                    }
                    catch
                    {
                        Console.WriteLine("Substring error"); // TODO: implement logger and replace
                        listing.Description = StringHelpers.AddNewLinesToMisformedString(listing.Description_Raw); 
                    }
                }
                else
                {
                    listing.Description = StringHelpers.AddNewLinesToMisformedString(listing.Description_Raw);
                }
                
                jobList.Add(listing);
            }

            return jobList;
        }
    }
}
