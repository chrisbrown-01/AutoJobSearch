using AutoJobSearchJobScraper.Exceptions;
using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Helpers;
using AutoJobSearchShared.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoJobSearchJobScraper.WebScraper
{
    internal class SeleniumWebScraper : IWebScraper
    {
        private const string REGEX_URL_PATTERN = @"https?://[^\s""]+";

        private readonly int MAX_JOB_LISTING_INDEX;
        private readonly string STARTING_INDEX_KEYWORD_GOOGLE;
        private readonly string ENDING_INDEX_KEYWORD_GOOGLE;
        private readonly string STARTING_INDEX_KEYWORD_INDEED;
        private readonly string ENDING_INDEX_KEYWORD_INDEED;
        private readonly string CAPTCHA_MESSAGE_GOOGLE;
        private readonly string CAPTCHA_MESSAGE_INDEED;
        private readonly string INDEED_JOB_DESCRIPTION_HTML_DIV_ID_ATTRIBUTE;

        private readonly ILogger<SeleniumWebScraper> _logger;

        public SeleniumWebScraper(ILogger<SeleniumWebScraper> logger)
        {
            _logger = logger;

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);

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

            STARTING_INDEX_KEYWORD_GOOGLE = config.GetValue<string>(nameof(STARTING_INDEX_KEYWORD_GOOGLE)) ??
                throw new AppSettingsFileArgumentException($"Failed to read {nameof(STARTING_INDEX_KEYWORD_GOOGLE)} from appsettings.json config file.");

            ENDING_INDEX_KEYWORD_GOOGLE = config.GetValue<string>(nameof(ENDING_INDEX_KEYWORD_GOOGLE)) ??
                throw new AppSettingsFileArgumentException($"Failed to read {nameof(ENDING_INDEX_KEYWORD_GOOGLE)} from appsettings.json config file.");

            STARTING_INDEX_KEYWORD_INDEED = config.GetValue<string>(nameof(STARTING_INDEX_KEYWORD_INDEED)) ??
                throw new AppSettingsFileArgumentException($"Failed to read {nameof(STARTING_INDEX_KEYWORD_INDEED)} from appsettings.json config file.");

            ENDING_INDEX_KEYWORD_INDEED = config.GetValue<string>(nameof(ENDING_INDEX_KEYWORD_INDEED)) ??
                throw new AppSettingsFileArgumentException($"Failed to read {nameof(ENDING_INDEX_KEYWORD_INDEED)} from appsettings.json config file.");

            CAPTCHA_MESSAGE_GOOGLE = config.GetValue<string>(nameof(CAPTCHA_MESSAGE_GOOGLE)) ??
                throw new AppSettingsFileArgumentException($"Failed to read {nameof(CAPTCHA_MESSAGE_GOOGLE)} from appsettings.json config file.");

            CAPTCHA_MESSAGE_INDEED = config.GetValue<string>(nameof(CAPTCHA_MESSAGE_INDEED)) ??
                throw new AppSettingsFileArgumentException($"Failed to read {nameof(CAPTCHA_MESSAGE_INDEED)} from appsettings.json config file.");

            INDEED_JOB_DESCRIPTION_HTML_DIV_ID_ATTRIBUTE = config.GetValue<string>(nameof(INDEED_JOB_DESCRIPTION_HTML_DIV_ID_ATTRIBUTE)) ??
                throw new AppSettingsFileArgumentException($"Failed to read {nameof(INDEED_JOB_DESCRIPTION_HTML_DIV_ID_ATTRIBUTE)} from appsettings.json config file.");
        }

        private void ProcessJobs_Google(List<JobListing> jobListings, IEnumerable<HtmlNode>? jobListingNodes, string searchTerm)
        {
            if (jobListingNodes is null) return;

            foreach (var node in jobListingNodes)
            {
                // Get all <a> html elements that contain the word "apply". These will contain the direct web links to the job application.
                var anchorElements = node.Descendants("a").Where(a => a.InnerText.Contains("apply", StringComparison.OrdinalIgnoreCase));

                // Skip the job listing if no anchor elements found.
                if (!anchorElements.Any()) continue;

                var jobListing = new JobListing
                {
                    SearchTerm = searchTerm,
                    Description_Raw = WebUtility.HtmlDecode(node.InnerText)
                };

                jobListing.ApplicationLinks = GetApplicationLinks(anchorElements);
                jobListing.Description = GetDescription(jobListing, JobBoards.GoogleJobSearch);

                jobListings.Add(jobListing);
            }
        }

        private void ProcessJobs_Indeed(
            List<JobListing> jobListings,
            IEnumerable<HtmlNode>? jobListingNodes,
            string searchTerm,
            ref ChromeDriver driver,
            ref HtmlDocument doc)
        {
            // TODO: need to account for US (www.indeed.com) and CA (ca.indeed.com) subdomains

            if (jobListingNodes is null) return;

            foreach (var node in jobListingNodes)
            {
                var jobId = node.GetAttributeValue(INDEED_JOB_DESCRIPTION_HTML_DIV_ID_ATTRIBUTE, null);

                if (String.IsNullOrWhiteSpace(jobId)) continue;

                var url = $"https://ca.indeed.com/viewjob?jk={jobId}";

                driver.Navigate().GoToUrl(url);
                doc.LoadHtml(driver.PageSource);

                CheckForCaptcha(ref doc, ref driver, url);

                var jobListing = new JobListing
                {
                    SearchTerm = searchTerm,
                    Description_Raw = WebUtility.HtmlDecode(doc.DocumentNode.InnerText)
                };

                jobListing.ApplicationLinks.Add(new ApplicationLink { Link = url });
                jobListing.Description = GetDescription(jobListing, JobBoards.Indeed);

                jobListings.Add(jobListing);
            }
        }

        public async Task<IEnumerable<JobListing>> ScrapeJobsAsync(IEnumerable<string> searchTerms)
        {
            _logger.LogInformation("Begin scraping jobs. Number of members in searchTerms argument: {@searchTerms.Count}", searchTerms.Count());

            var jobListings = new List<JobListing>();
            var htmlDocument_Google = new HtmlDocument();
            var htmlDocument_Indeed = new HtmlDocument();
            var driver = new ChromeDriver();

            try
            {
                foreach (var searchTerm in searchTerms)
                {
                    // Parse through the amount of jobs specified by MAX_PAGE_INDEX. Increment the start index by 10 every iteration.
                    for (int i = 0; i < MAX_JOB_LISTING_INDEX + 1; i += 10)
                    {
                        var googleJobsBoardURL = $"https://www.google.com/search?q={WebUtility.UrlEncode(searchTerm)}&sourceid=chrome&ie=UTF-8&ibp=htl;jobs&start={i}";
                        var indeedJobsBoardURL = $"https://ca.indeed.com/jobs?q={WebUtility.UrlEncode(searchTerm)}&start={i}";
                        // TODO: need to account for US (www.indeed.com) and CA (ca.indeed.com) subdomains

                        // Scrape Google jobs board
                        driver.Navigate().GoToUrl(googleJobsBoardURL);
                        htmlDocument_Google!.LoadHtml(driver.PageSource);
                        CheckForCaptcha(ref htmlDocument_Google, ref driver, googleJobsBoardURL);
                        var jobListingNodes_Google = htmlDocument_Google?.DocumentNode?.SelectNodes("//li")?.ToList();

                        // Scrape Indeed jobs board
                        driver.Navigate().GoToUrl(indeedJobsBoardURL);
                        htmlDocument_Indeed!.LoadHtml(driver.PageSource);
                        CheckForCaptcha(ref htmlDocument_Indeed, ref driver, indeedJobsBoardURL);
                        var jobListingNodes_Indeed = htmlDocument_Indeed?.DocumentNode?.SelectNodes("//a").Where(x => x.GetAttributeValue("data-jk", null) != null).ToList();

                        if ((jobListingNodes_Google is null || !jobListingNodes_Google.Any()) &&
                            (jobListingNodes_Indeed is null || !jobListingNodes_Indeed.Any()))
                        {
                            _logger.LogWarning(
                                "No jobs detected during job scraping. " +
                                "{@searchTerm} {@iterationValue} {@MAX_JOB_LISTING_INDEX}",
                                searchTerm, i, MAX_JOB_LISTING_INDEX);

                            break;
                        }

                        //ProcessJobs_Google(jobListings, jobListingNodes_Google, searchTerm);
                        ProcessJobs_Indeed(jobListings, jobListingNodes_Indeed, searchTerm, ref driver, ref htmlDocument_Indeed!);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception thrown during job scraping. {@Exception}", ex);
            }
            finally
            {
                driver.Quit();
            }

            await Task.CompletedTask;

            _logger.LogInformation("Finished scraping jobs. {@jobListings.Count} job listings returned.", jobListings.Count);
            return jobListings;
        }


        /// <summary>
        /// If captcha is detected, close the browser and open a new browser to the same URL. This should allow scraping to continue.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="driver"></param>
        /// <param name="url"></param>
        private void CheckForCaptcha(ref HtmlDocument doc, ref ChromeDriver driver, string url)
        {
            var innerText = doc.DocumentNode.InnerText;

            if (innerText.Contains(CAPTCHA_MESSAGE_GOOGLE, StringComparison.OrdinalIgnoreCase) ||
                innerText.Contains(CAPTCHA_MESSAGE_INDEED, StringComparison.OrdinalIgnoreCase))
            {
                driver.Quit();
                driver = new ChromeDriver();
                driver.Navigate().GoToUrl(url);
                doc.LoadHtml(driver.PageSource); // Reload the page now that captcha has been bypassed.
            }
        }


        // TODO: delete
        //private void CheckForCaptcha(HtmlDocument doc, ChromeDriver driver)
        //{
        //    var checkForCaptcha = doc?.DocumentNode?.InnerText;

        //    if (checkForCaptcha != null && checkForCaptcha.Contains("detected unusual traffic", StringComparison.OrdinalIgnoreCase))
        //    {
        //        var input = "";

        //        while (input != "CONTINUE")
        //        {
        //            Console.WriteLine("Solve the Google Chrome captcha then type in 'CONTINUE' to continue scraping: ");
        //            input = Console.ReadLine();
        //        }

        //        doc!.LoadHtml(driver.PageSource); // Reload the page now that captcha has been bypassed.
        //    }
        //}


        /// <summary>
        /// Get the direct hyperlinks to the job listing.
        /// </summary>
        /// <param name="listing"></param>
        /// <param name="anchorElements"></param>
        private List<ApplicationLink> GetApplicationLinks(IEnumerable<HtmlNode> anchorElements)
        {
            var applicationLinks = new List<ApplicationLink>();
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

                applicationLinks.Add(new ApplicationLink
                {
                    Link_RawHTML = anchor.OuterHtml,
                    Link = hyperlink
                });
            }

            return applicationLinks;
        }

        /// <summary>
        /// This method attempts to extract only the important portions of the scraped job listing description.
        /// </summary>
        /// <param name="listing"></param>
        private string GetDescription(JobListing listing, JobBoards jobBoardOption)
        {
            // The raw job descriptions contain a lot of unhelpful and boilerplate text to present the job on their website.
            // Usually, the main job description can be found as a substring between two keywords that are applied
            // at the beginning and end of the raw HTML job description.

            string startingIndexKeyword = String.Empty;
            int startingIndex = -1;
            int endingIndex = -1;
            int startingIndexKeywordLength = -1;

            if (jobBoardOption == JobBoards.GoogleJobSearch)
            {
                startingIndex = listing.Description_Raw.IndexOf(STARTING_INDEX_KEYWORD_GOOGLE);
                endingIndex = listing.Description_Raw.IndexOf(ENDING_INDEX_KEYWORD_GOOGLE);
                startingIndexKeywordLength = STARTING_INDEX_KEYWORD_GOOGLE.Length;
                startingIndexKeyword = STARTING_INDEX_KEYWORD_GOOGLE;
            }
            else if (jobBoardOption == JobBoards.Indeed)
            {
                startingIndex = listing.Description_Raw.IndexOf(STARTING_INDEX_KEYWORD_INDEED);
                endingIndex = listing.Description_Raw.IndexOf(ENDING_INDEX_KEYWORD_INDEED);
                startingIndexKeywordLength = STARTING_INDEX_KEYWORD_INDEED.Length;
                startingIndexKeyword = STARTING_INDEX_KEYWORD_INDEED;
            }
            else
            {
                throw new ArgumentException("JobBoards enum selection is not valid.");
            }

            // If the raw description doesn't contain the keywords in the correct order, don't attempt to extract the substring description.
            if (startingIndexKeywordLength != -1 && 
                startingIndex != -1 && 
                endingIndex != -1 && 
                endingIndex > startingIndex)
            {
                try
                {
                    var description = listing.Description_Raw.Substring(startingIndex + startingIndexKeywordLength, endingIndex - (startingIndex + startingIndexKeywordLength));
                    return StringHelpers.AddNewLinesToMisformedString(description);
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
                        startingIndexKeyword,
                        startingIndexKeywordLength);

                    return StringHelpers.AddNewLinesToMisformedString(listing.Description_Raw);
                }
            }
            else
            {
                return StringHelpers.AddNewLinesToMisformedString(listing.Description_Raw);
            }
        }
    }
}
