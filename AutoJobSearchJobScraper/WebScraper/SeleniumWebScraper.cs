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

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            var config = builder.Build();

            MAX_PAGE_INDEX = config.GetValue<int>("MAX_PAGE_INDEX");
            if (MAX_PAGE_INDEX < 1) throw new ArgumentException($"MAX_PAGE_INDEX must be greater than 0. Current value is {MAX_PAGE_INDEX}."); 

            STARTING_INDEX_KEY = config.GetValue<string>("STARTING_INDEX_KEY") ?? throw new NullReferenceException(); // TODO: custom exception for json config file arguments
            ENDING_INDEX_KEY = config.GetValue<string>("ENDING_INDEX_KEY") ?? throw new NullReferenceException();
        }

        // TODO: surround in try-catch so that results are still saved even if captcha kills selenium
        public async Task<IEnumerable<JobListing>> ScrapeJobsAsync(IEnumerable<string> searchTerms) 
        {
            _logger.LogInformation("Begin scraping jobs. Number of members in searchTerms argument: {@searchTerms.Count}", searchTerms.Count());

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

            _logger.LogInformation("Finished scraping {@jobListings.Count} jobs.", jobListings.Count);
            return jobListings;
        }

        /// <summary>
        /// Extracts the job listing description and application links from each raw HTML list item element.
        /// </summary>
        /// <param name="liElements">HTML <li> elements.</param>
        /// <param name="searchTerm">The search term that the web scraper used to find the HTML list item.</param>
        /// <returns></returns>
        private IEnumerable<JobListing> ExtractJobListingsFromLiElements(IEnumerable<HtmlNode> liElements, string searchTerm) 
        {
            var jobList = new List<JobListing>();

            foreach (var li in liElements)
            {
                var listing = new JobListing
                {
                    SearchTerm = searchTerm
                };

                // Get all <a> html elements that contain the word "apply". These will contain the direct web links to the job application.
                var anchorElements = li.Descendants("a").Where(a => a.InnerText.Contains("apply", StringComparison.OrdinalIgnoreCase));

                if (!anchorElements.Any()) continue;

                var existingLinks = new HashSet<string>();

                foreach (var anchor in anchorElements)
                {
                    MatchCollection hyperlinks = Regex.Matches(anchor.OuterHtml, REGEX_URL_PATTERN);
                    
                    if (!hyperlinks.Any()) continue; // If no weblinks found, skip and continue to the next link.

                    var hyperlink = hyperlinks.First().Value;

                    // Only add application link to object if it doesn't already exist.
                    if (!existingLinks.Contains(hyperlink))
                    {
                        existingLinks.Add(hyperlink);

                        var applicationLink = new ApplicationLink
                        {
                            Link_RawHTML = anchor.OuterHtml,
                            Link = hyperlink
                        };

                        listing.ApplicationLinks.Add(applicationLink);                      
                    }
                }

                listing.Description_Raw = WebUtility.HtmlDecode(li.InnerText);

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
                        _logger.LogError("Error extracting Description from Description_Raw. " +
                            "Variable values: " +
                            "{@startingIndex}, " +
                            "{@endingIndex}, " +
                            "@{STARTING_INDEX_KEY}, " +
                            "@{STARTING_INDEX_KEY.Length}",
                            startingIndex,
                            endingIndex,
                            STARTING_INDEX_KEY,
                            STARTING_INDEX_KEY.Length);

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
