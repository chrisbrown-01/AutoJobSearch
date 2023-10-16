using AutoJobSearchJobScraper.Exceptions;
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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoJobSearchJobScraper.WebScraper
{
    internal class SeleniumWebScraper : IWebScraper
    {
        private const string REGEX_URL_PATTERN = @"https?://[^\s""]+";

        private readonly int MAX_JOB_LISTING_INDEX;
        private readonly string STARTING_INDEX_KEYWORD;
        private readonly string ENDING_INDEX_KEYWORD;

        private readonly ILogger<SeleniumWebScraper> _logger;

        public SeleniumWebScraper(ILogger<SeleniumWebScraper> logger)
        {
            _logger = logger;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            var config = builder.Build();

            try
            {
                MAX_JOB_LISTING_INDEX = config.GetValue<int>(nameof(MAX_JOB_LISTING_INDEX));
            }
            catch (InvalidOperationException)
            {
                throw new AppSettingsFileArgumentException(
                    $"Failed to read {nameof(MAX_JOB_LISTING_INDEX)} from appsettings.json config file. " +
                    $"Ensure that {nameof(MAX_JOB_LISTING_INDEX)} is an integer.");
            }

            if (MAX_JOB_LISTING_INDEX < 1)
            {
                throw new AppSettingsFileArgumentException(
                    $"Failed to read {nameof(MAX_JOB_LISTING_INDEX)} from appsettings.json config file. " +
                    $"Ensure that {nameof(MAX_JOB_LISTING_INDEX)} has a value greater than 0.");
            }

            STARTING_INDEX_KEYWORD = config.GetValue<string>(nameof(STARTING_INDEX_KEYWORD)) ?? 
                throw new AppSettingsFileArgumentException($"Failed to read {nameof(STARTING_INDEX_KEYWORD)} from appsettings.json config file.");
            
            ENDING_INDEX_KEYWORD = config.GetValue<string>(nameof(ENDING_INDEX_KEYWORD)) ??
                throw new AppSettingsFileArgumentException($"Failed to read {nameof(ENDING_INDEX_KEYWORD)} from appsettings.json config file.");
        }

        public async Task<IEnumerable<JobListing>> ScrapeJobsAsync(IEnumerable<string> searchTerms)
        {
            _logger.LogInformation("Begin scraping jobs. Number of members in searchTerms argument: {@searchTerms.Count}", searchTerms.Count());

            var jobListings = new List<JobListing>();
            var doc = new HtmlDocument();
            var driver = new ChromeDriver();

            foreach (var searchTerm in searchTerms)
            {
                try
                {
                    // Parse through the amount of jobs specified by MAX_PAGE_INDEX. Parse for 10 jobs per iteration.
                    for (int i = 0; i < MAX_JOB_LISTING_INDEX + 1; i += 10)
                    {
                        driver.Navigate().GoToUrl($"https://www.google.com/search?q={WebUtility.UrlEncode(searchTerm)}&sourceid=chrome&ie=UTF-8&ibp=htl;jobs&start={i}");
                        doc!.LoadHtml(driver.PageSource);

                        CheckForCaptcha(doc, driver);

                        var liElements = doc?.DocumentNode?.SelectNodes("//li")?.AsEnumerable();

                        if (liElements == null)
                        {
                            _logger.LogWarning(
                                "No li elements detected during job scraping. " +
                                "{@searchTerm} {@iterationValue} {@MAX_JOB_LISTING_INDEX}",
                                searchTerm, i, MAX_JOB_LISTING_INDEX);

                            continue;
                        }

                        jobListings.AddRange(ExtractJobListingsFromLiElements(liElements, searchTerm));
                    }
                }
                catch (Exception ex) // TODO: try and see what exceptions would actually get thrown then handle them
                {
                    _logger.LogError("Exception thrown during job scraping. {@Exception}", ex);
                    throw; 
                }
            }

            driver.Close();
            driver.Quit();
            await Task.CompletedTask;

            _logger.LogInformation("Finished scraping jobs. {@jobListings.Count} job listings returned.", jobListings.Count);
            return jobListings;
        }

        /// <summary>
        /// If Google captcha is detected, pause the script until the user solves the captcha and manually continues the script.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="driver"></param>
        private void CheckForCaptcha(HtmlDocument doc, ChromeDriver driver)
        {
            var checkForCaptcha = doc?.DocumentNode?.InnerText;

            if (checkForCaptcha != null && checkForCaptcha.Contains("detected unusual traffic", StringComparison.OrdinalIgnoreCase))
            {
                var input = "";

                while (input != "CONTINUE")
                {
                    Console.WriteLine("Solve the Google Chrome captcha then type in 'CONTINUE' to continue scraping: ");
                    input = Console.ReadLine();
                }

                doc!.LoadHtml(driver.PageSource); // Reload the page now that captcha has been bypassed.
            }
        }

        /// <summary>
        /// Get the direct hyperlinks to the job listing.
        /// </summary>
        /// <param name="listing"></param>
        /// <param name="anchorElements"></param>
        private void GetApplicationLinksForJobListing(JobListing listing, IEnumerable<HtmlNode> anchorElements)
        {
            var existingLinks = new HashSet<string>();

            foreach (var anchor in anchorElements)
            {
                MatchCollection hyperlinks = Regex.Matches(anchor.OuterHtml, REGEX_URL_PATTERN);

                // If no weblinks found, skip and continue to the next link.
                if (!hyperlinks.Any()) continue;

                var hyperlink = hyperlinks.First().Value;

                // If application link is already in the list, skip it. This is to prevent duplicate links.
                if (existingLinks.Contains(hyperlink)) continue;

                existingLinks.Add(hyperlink);

                var applicationLink = new ApplicationLink
                {
                    Link_RawHTML = anchor.OuterHtml,
                    Link = hyperlink
                };

                listing.ApplicationLinks.Add(applicationLink);
            }
        }

        /// <summary>
        /// This method attempts to extract only the important portions of the scraped job listing description.
        /// </summary>
        /// <param name="listing"></param>
        private void GetDescriptionForJobListing(JobListing listing)
        {
            // The raw job descriptions contain a lot of unhelpful and boilerplate text that Google applies to present the job on their website.
            // Usually the main job description can be found as a substring between two keywords that Google generally applies at the beginning and
            // end of the raw HTML job description.

            var startingIndex = listing.Description_Raw.IndexOf(STARTING_INDEX_KEYWORD);
            var endingIndex = listing.Description_Raw.IndexOf(ENDING_INDEX_KEYWORD);

            // If the raw description doesn't contain the keywords in the correct order, don't attempt to extract the substring description.
            if (startingIndex != -1 && endingIndex != -1 && (endingIndex > startingIndex))
            {
                try
                {
                    listing.Description = listing.Description_Raw.Substring(startingIndex + STARTING_INDEX_KEYWORD.Length, endingIndex - (startingIndex + STARTING_INDEX_KEYWORD.Length));
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
                        STARTING_INDEX_KEYWORD,
                        STARTING_INDEX_KEYWORD.Length);

                    listing.Description = StringHelpers.AddNewLinesToMisformedString(listing.Description_Raw);
                }
            }
            else
            {
                listing.Description = StringHelpers.AddNewLinesToMisformedString(listing.Description_Raw);
            }
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
                // Get all <a> html elements that contain the word "apply". These will contain the direct web links to the job application.
                var anchorElements = li.Descendants("a").Where(a => a.InnerText.Contains("apply", StringComparison.OrdinalIgnoreCase));

                // Skip the job listing if no anchor elements found.
                if (!anchorElements.Any()) continue;

                var listing = new JobListing
                {
                    SearchTerm = searchTerm,
                    Description_Raw = WebUtility.HtmlDecode(li.InnerText)
                };

                GetApplicationLinksForJobListing(listing, anchorElements);
                GetDescriptionForJobListing(listing);

                jobList.Add(listing);
            }

            return jobList;
        }
    }
}
