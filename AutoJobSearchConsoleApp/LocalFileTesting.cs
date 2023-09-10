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
        // TODO: create tests
        private const string STARTING_INDEX_KEY = "CollapseJob description";
        private const string ENDING_INDEX_KEY = "Show full description"; // "Show full description" or if none found, "Report this job"
        private const string REGEX_URL_PATTERN = @"https?://[^\s""]+";

        public static void CheckForDuplicatesTest()
        {
            var jobList = LoadFromJsonFile(Paths.MULTI_PAGE_JSON_FILE_PATH);

            var links = jobList.SelectMany(x => x.Links).ToList();

            var distinctLinks = links.Distinct().ToList();

            var nonUniqueInnerTexts = jobList
                                      .GroupBy(jp => jp.InnerText)
                                      .Where(g => g.Count() > 1)
                                      .Select(g => g.Key)
                                      .ToList();

            var nonUniqueJobPostings = jobList
                                       .Where(jp => nonUniqueInnerTexts.Contains(jp.InnerText))
                                       .ToList();

            Console.WriteLine();
        }

        public static void ScoringTest()
        {
            var jobList = LoadFromJsonFile(Paths.MULTI_PAGE_JSON_FILE_PATH);

            foreach(var job in jobList)
            {
                var description = job.InnerTextCleaned;

                foreach(var keyword in DataHelpers.KEYWORDS_POSITIVE)
                {
                    if (description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Score++;
                    }
                }

                foreach (var item in DataHelpers.KEYWORDS_NEGATIVE)
                {
                    if (description.Contains(item, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Score--;
                    }
                }

                // TODO: more robust ensuring that arguments are made lower case
                foreach (var sentiment in DataHelpers.SENTIMENTS_POSITIVE)
                {
                    if(Fuzz.WeightedRatio(sentiment.ToLower(), description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment.ToLower(), description.ToLower()) >= 50)
                    {
                        job.Score++;
                    }
                }

                foreach (var sentiment in DataHelpers.SENTIMENTS_NEGATIVE)
                {
                    if (Fuzz.WeightedRatio(sentiment.ToLower(), description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment.ToLower(), description.ToLower()) >= 50)
                    {
                        job.Score--;
                    }
                }
            }

            jobList = jobList.OrderByDescending(x => x.Score).ToList();
            Console.WriteLine();
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
            return JsonSerializer.Deserialize<List<JobListing>>(File.ReadAllText(path))!; // TODO: exception handling
        }

        private static async Task GenerateJobListingJsonFileFromMultipleTextFiles()
        {
            var doc = new HtmlDocument();
            var jobListings = new List<JobListing>();

            for (int i = 1; i < 12; i++)
            {
                var textFile = await File.ReadAllTextAsync($"..\\..\\..\\DataFiles\\DotNetTennessee{i}of11.txt");
                doc.LoadHtml(textFile);

                jobListings.AddRange(ExtractJobs(doc));
            }

            var jobListingsScrubbed = RemoveDuplicates(jobListings); // TODO: better variable naming

            await File.WriteAllTextAsync(Paths.MULTI_PAGE_JSON_FILE_PATH, JsonSerializer.Serialize(jobListingsScrubbed));
        }

        private static List<JobListing> RemoveDuplicates(List<JobListing> listWithPossibleDuplicates)
        {
            var uniqueLinks = new List<string>(); // TODO: uniqueLinks will need to first be populated with existing items from database
            var uniqueJobPostings = new List<JobListing>();

            foreach (var jobPosting in listWithPossibleDuplicates)
            {
                bool isDuplicate = false;
                foreach (string link in jobPosting.Links)
                {
                    if (uniqueLinks.Contains(link))
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (isDuplicate) continue;
                
                uniqueJobPostings.Add(jobPosting);

                foreach (string link in jobPosting.Links)
                {
                    uniqueLinks.Add(link);
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

            var jobListingsScrubbed = RemoveDuplicates(jobListings); // TODO: better variable naming

            await File.WriteAllTextAsync(Paths.SINGLE_PAGE_JSON_FILE_PATH, JsonSerializer.Serialize(jobListingsScrubbed));
        }

        private static List<JobListing> ExtractJobs(HtmlDocument htmlDocument)
        {
            var jobList = new List<JobListing>();

            var liElements = htmlDocument.DocumentNode.SelectNodes("//li").ToList();            

            foreach (var li in liElements)
            {
                // TODO: update SearchTerm property
                var listing = new JobListing();

                var links = li.Descendants("a")
                    .Where(a => a.InnerText.Contains("apply", StringComparison.OrdinalIgnoreCase));

                foreach (var link in links)
                {
                    listing.LinksOuterHtml.Add(link.OuterHtml);

                    MatchCollection matches = Regex.Matches(link.OuterHtml, REGEX_URL_PATTERN);

                    foreach (Match match in matches)
                    {

                        // listing.Links.Add(match.Value.Replace('"', ' ').Trim()); // TODO: remove
                        listing.Links.Add(match.Value);
                    }
                }

                //listing.InnerText = li.InnerText; // TODO: remove non-html decoding
                listing.InnerText = WebUtility.HtmlDecode(li.InnerText); 

                // TODO: extract to method
                var startingIndex = listing.InnerText.IndexOf(STARTING_INDEX_KEY);
                var endingIndex = listing.InnerText.IndexOf(ENDING_INDEX_KEY);

                if (startingIndex != -1 && endingIndex != -1)
                {
                    try
                    {
                        listing.InnerTextCleaned = listing.InnerText.Substring(startingIndex + STARTING_INDEX_KEY.Length, endingIndex - (startingIndex + STARTING_INDEX_KEY.Length));
                    }
                    catch
                    {
                        Console.WriteLine("Substring error"); // TODO: implement logger and replace
                        listing.InnerTextCleaned = StringFormattingHeuristic(listing.InnerText);
                    }
                }
                else
                {
                    listing.InnerTextCleaned = StringFormattingHeuristic(listing.InnerText);
                }

                jobList.Add(listing);
            }

            return jobList;
        }

        private static string StringFormattingHeuristic(string input)
        {
            string pattern = @"([a-z]|[.])([A-Z])";
            string replacement = "$1\n$2";
            string result = Regex.Replace(input, pattern, replacement);

            return result;
        }
    }
}
