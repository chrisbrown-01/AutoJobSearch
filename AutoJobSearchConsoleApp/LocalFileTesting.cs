using AutoJobSearchConsoleApp.Models;
using FuzzySharp;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AutoJobSearchConsoleApp.LocalFileTesting;

namespace AutoJobSearchConsoleApp
{
    internal class LocalFileTesting
    {
        private const string STARTING_INDEX_KEY = "CollapseJob description";
        private const string ENDING_INDEX_KEY = "Show full description"; // "Show full description" or if none found, "Report this job"
        private const string REGEX_URL_PATTERN = @"https?://[^\s""]+";

        private static string StringFormattingHeuristic(string input)
        {
            string pattern = @"([a-z]|[.])([A-Z])";
            string replacement = "$1\n$2";
            string result = Regex.Replace(input, pattern, replacement);

            return result;
        }

        public static void CheckForDuplicatesTest()
        {
            var jobList = LoadFromJsonFile(Paths.MULTI_PAGE_JSON_FILE_PATH);

            var links = jobList.SelectMany(x => x.ApplicationLinks).ToList();

            var distinctLinks = links.Distinct().ToList();

            var nonUniqueInnerTexts = jobList
                                      .GroupBy(jp => jp.Description_Raw)
                                      .Where(g => g.Count() > 1)
                                      .Select(g => g.Key)
                                      .ToList();

            var nonUniqueJobPostings = jobList
                                       .Where(jp => nonUniqueInnerTexts.Contains(jp.Description_Raw))
                                       .ToList();

            Console.WriteLine();
        }

        public static void ScoringTest()
        {
            var jobList = LoadFromJsonFile(Paths.MULTI_PAGE_JSON_FILE_PATH);
            ApplyScorings(jobList);

            jobList = jobList.OrderByDescending(x => x.Score).ToList();
            Console.WriteLine();
        }

        private static List<JobListing> ApplyScorings(List<JobListing> jobList) 
        {
            foreach (var job in jobList)
            {
                foreach (var keyword in DataHelpers.KEYWORDS_POSITIVE)
                {
                    if (job.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Score++;
                    }
                }

                foreach (var item in DataHelpers.KEYWORDS_NEGATIVE)
                {
                    if (job.Description.Contains(item, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Score--;
                    }
                }

                foreach (var sentiment in DataHelpers.SENTIMENTS_POSITIVE)
                {
                    if (Fuzz.WeightedRatio(sentiment.ToLower(), job.Description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment.ToLower(), job.Description.ToLower()) >= 50)
                    {
                        job.Score++;
                    }
                }

                foreach (var sentiment in DataHelpers.SENTIMENTS_NEGATIVE)
                {
                    if (Fuzz.WeightedRatio(sentiment.ToLower(), job.Description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment.ToLower(), job.Description.ToLower()) >= 50)
                    {
                        job.Score--;
                    }
                }
            }

            return jobList;
        }

        public static async Task CreateJsonFiles()
        {
            await GenerateJobListingJsonFileFromSingleTextFile();
            await GenerateJobListingJsonFileFromMultipleTextFiles();
        }

        public static void LoadFromJsonFileTests()
        {
            var singlePageList = LoadFromJsonFile(Paths.SINGLE_PAGE_JSON_FILE_PATH);
            var multiPageList = LoadFromJsonFile(Paths.MULTI_PAGE_JSON_FILE_PATH);

            Console.WriteLine();
        }

        public static List<JobListing> LoadFromJsonFile(string path)
        {
            return JsonSerializer.Deserialize<List<JobListing>>(File.ReadAllText(path))!; 
        }

        public static async Task<List<JobListing>> GetJobListingsFromFiles()
        {
            var doc = new HtmlDocument();
            var jobListings = new List<JobListing>();

            for (int i = 1; i < 12; i++)
            {
                var textFile = await File.ReadAllTextAsync($"..\\..\\..\\DataFiles\\DotNetTennessee{i}of11.txt");
                doc.LoadHtml(textFile);

                jobListings.AddRange(ExtractJobs(doc));
            }

            var jobListingsScrubbed = RemoveDuplicates(jobListings); 

            ApplyScorings(jobListingsScrubbed);

            return jobListingsScrubbed;
        }

        private static async Task GenerateJobListingJsonFileFromMultipleTextFiles()
        {
            var jobListingsScrubbed = await GetJobListingsFromFiles(); 

            await File.WriteAllTextAsync(Paths.MULTI_PAGE_JSON_FILE_PATH, JsonSerializer.Serialize(jobListingsScrubbed));
        }

        private static List<JobListing> RemoveDuplicates(List<JobListing> listWithPossibleDuplicates)
        {
            var uniqueLinks = new List<string>(); 
            var uniqueJobPostings = new List<JobListing>();

            foreach (var jobPosting in listWithPossibleDuplicates)
            {
                bool isDuplicate = false;

                foreach (var link in jobPosting.ApplicationLinks)
                {
                    if (uniqueLinks.Contains(link.Link))
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (isDuplicate) continue;

                uniqueJobPostings.Add(jobPosting);

                foreach (var link in jobPosting.ApplicationLinks)
                {
                    uniqueLinks.Add(link.Link);
                }
            }

            return uniqueJobPostings;
        }

        private static async Task GenerateJobListingJsonFileFromSingleTextFile()
        {
            var textFile = await File.ReadAllTextAsync(Paths.RAW_PAGE_SOURCE_FILE_PATH);
            var doc = new HtmlDocument();
            doc.LoadHtml(textFile);

            var jobListings = ExtractJobs(doc);

            var jobListingsScrubbed = RemoveDuplicates(jobListings); 

            await File.WriteAllTextAsync(Paths.SINGLE_PAGE_JSON_FILE_PATH, JsonSerializer.Serialize(jobListingsScrubbed));
        }

        private static List<JobListing> ExtractJobs(HtmlDocument htmlDocument)
        {
            var jobList = new List<JobListing>();

            var liElements = htmlDocument.DocumentNode.SelectNodes("//li").ToList();

            foreach (var li in liElements)
            {
                var listing = new JobListing();

                var links = li.Descendants("a")
                    .Where(a => a.InnerText.Contains("apply", StringComparison.OrdinalIgnoreCase));

                foreach (var link in links)
                {
                    var applicationLink = new ApplicationLink();

                    applicationLink.Link_RawHTML = link.OuterHtml;

                    //listing.ApplicationLinks.Add(link.OuterHtml);

                    MatchCollection matches = Regex.Matches(link.OuterHtml, REGEX_URL_PATTERN);

                    if(matches.FirstOrDefault() != null) 
                    {
                        applicationLink.Link = matches.First().Value;
                    }

                    listing.ApplicationLinks.Add(applicationLink);
                }

                listing.Description_Raw = WebUtility.HtmlDecode(li.InnerText);

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
                        Console.WriteLine("Substring error"); 
                        listing.Description = StringFormattingHeuristic(listing.Description_Raw);
                    }
                }
                else
                {
                    listing.Description = StringFormattingHeuristic(listing.Description_Raw);
                }

                jobList.Add(listing);
            }

            return jobList;
        }
    }
}