using AutoJobSearchConsoleApp.Models;
using AutoJobSearchConsoleApp.Utility;
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoJobSearchConsoleApp
{
    internal class JobScraperSelenium
    {
        // TODO: convert to use config file
        private const string STARTING_INDEX_KEY = "CollapseJob description";
        private const string ENDING_INDEX_KEY = "Show full description"; // "Show full description" or if none found, "Report this job"
        private const string REGEX_URL_PATTERN = @"https?://[^\s""]+";
        private const int MAX_START_PAGE = 100; 

        // TODO: surround in try-catch so that results are still saved even if captcha kills selenium
        public static async Task<IList<JobListing>> ScrapeJobs(IEnumerable<string> searchTerms) // TODO: extract interface
        {
            var jobListings = new List<JobListing>();

            var doc = new HtmlDocument();
            var driver = new ChromeDriver();

            try
            {
                foreach (var searchTerm in searchTerms)
                {
                    for (int i = 0; i < MAX_START_PAGE + 1; i += 10)
                    {
                        driver.Navigate().GoToUrl($"https://www.google.com/search?q={WebUtility.UrlEncode(searchTerm)}&ibp=htl;jobs&start={i}");

                        var newUrl = driver.Url; // TODO: experiment with selenium URLs

                        doc.LoadHtml(driver.PageSource);
                        var liElements = doc.DocumentNode?.SelectNodes("//li")?.ToList();

                        if (liElements == null) break;

                        jobListings.AddRange(ExtractJobListingsFromLiElements(liElements, searchTerm));

                        await Task.Delay(Random.Shared.Next(3000, 6000)); // TODO: make delay values part of config file
                    }
                }

                return jobListings;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                driver.Close();
                driver.Quit();
            }
        }

        private static IList<JobListing> ExtractJobListingsFromLiElements(IEnumerable<HtmlNode> liElements, string searchTerm)
        {
            var jobList = new List<JobListing>();

            foreach (var li in liElements)
            {
                var listing = new JobListing();
                listing.SearchTerm = searchTerm;

                var links = li.Descendants("a")
                    .Where(a => a.InnerText.Contains("apply", StringComparison.OrdinalIgnoreCase));

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

                    listing.ApplicationLinks.Add(applicationLink);
                }

                listing.Description_Raw = WebUtility.HtmlDecode(li.InnerText);

                // TODO: extract to method
                var startingIndex = listing.Description_Raw.IndexOf(STARTING_INDEX_KEY);
                var endingIndex = listing.Description_Raw.IndexOf(ENDING_INDEX_KEY);

                if (startingIndex != -1 && endingIndex != -1)
                {
                    try
                    {
                        listing.Description = listing.Description_Raw.Substring(startingIndex + STARTING_INDEX_KEY.Length, endingIndex - (startingIndex + STARTING_INDEX_KEY.Length));
                    }
                    catch
                    {
                        Console.WriteLine("Substring error"); // TODO: implement logger and replace
                        listing.Description = StringUtility.AddNewLinesToMisformedString(listing.Description_Raw);
                    }
                }
                else
                {
                    listing.Description = StringUtility.AddNewLinesToMisformedString(listing.Description_Raw);
                }

                jobList.Add(listing);
            }

            return jobList;
        }
    }
}
