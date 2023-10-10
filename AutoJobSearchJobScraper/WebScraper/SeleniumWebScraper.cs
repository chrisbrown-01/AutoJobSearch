using AutoJobSearchJobScraper.Constants;
using AutoJobSearchShared.Helpers;
using AutoJobSearchShared.Models;
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
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
        public SeleniumWebScraper()
        {

        }

        // TODO: surround in try-catch so that results are still saved even if captcha kills selenium
        public async Task<List<JobListing>> ScrapeJobs(IEnumerable<string> searchTerms) // TODO: extract interface
        {
            var jobListings = new List<JobListing>();

            var innerTexts = new List<string>(); // TODO: remove?
            var linksList = new List<string>();
            var doc = new HtmlDocument();
            var driver = new ChromeDriver();

            try
            {
                foreach (var searchTerm in searchTerms)
                {
                    // TODO: place try block inside this loop instead?

                    for (int i = 0; i < ConfigVariables.MAX_START_PAGE + 1; i += 10)
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
        private List<JobListing> ExtractJobListingsFromLiElements(IEnumerable<HtmlNode> liElements, string searchTerm) // TODO: accessibility, interface
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

                    MatchCollection matches = Regex.Matches(link.OuterHtml, ConfigVariables.REGEX_URL_PATTERN);

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
                var startingIndex = listing.Description_Raw.IndexOf(ConfigVariables.STARTING_INDEX_KEY);
                var endingIndex = listing.Description_Raw.IndexOf(ConfigVariables.ENDING_INDEX_KEY);

                if (startingIndex != -1 && endingIndex != -1)
                {
                    try
                    {
                        listing.Description = listing.Description_Raw.Substring(startingIndex + ConfigVariables.STARTING_INDEX_KEY.Length, endingIndex - (startingIndex + ConfigVariables.STARTING_INDEX_KEY.Length));
                        listing.Description = StringHelpers.AddNewLinesToMisformedString(listing.Description); // TODO: test effectiveness
                    }
                    catch
                    {
                        Console.WriteLine("Substring error"); // TODO: implement logger and replace
                        listing.Description = StringHelpers.AddNewLinesToMisformedString(listing.Description_Raw); // TODO: Delete?
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
