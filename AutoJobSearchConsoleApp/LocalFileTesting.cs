using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
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
        //private const string REGEX_URL_PATTERN = @"href=\\""(https ?://[^\""]*)\""";
        //private const string REGEX_URL_PATTERN = @"https?://\S+";
        private const string REGEX_URL_PATTERN = @"https?://[^\s""]+";

        private static List<string> Positives = new()
        {
            "1-",
            "1 +",
            "1 -",
            "1+ ",
            "1- ",
            " 1+ ",
            " 1 ",
            " 1- ",
            "Junior",
            "Entry",
            "Entry-level",
            " year ",
            " -year ",
            " new ",
            "new grad",
            "recent grad",
            "new graduate",
            "recent graduate",
            "recently graduated",
            "not required"
        };

        private static List<string> Negatives = new()
        {
            "authorized to work",
            "Sr.",
            "Sr",
            "Senior",
            "Highly",
            "Highly experienced",
            "5+ years",
            "6+ years",
            "7+ years",
            "8+ years",
            "9+ years",
            "10+ years",
            "5+ year",
            "6+ year",
            "7+ year",
            "8+ year",
            "9+ year",
            "10+ year",
            "5+",
            "6+",
            "7+",
            "8+",
            "9+",
            "10+",
            "5 -",
            "6 -",
            "7 -",
            "8 -",
            "9 -",
            "10 -",
            "5-",
            "6-",
            "7-",
            "8-",
            "9-",
            "10-",
            " 5 -",
            " 6 -",
            " 7 -",
            " 8 -",
            " 9 -",
            " 10 -",
            " 5-",
            " 6-",
            " 7-",
            " 8-",
            " 9-",
            " 10-",
            "US Citizen",
            "citizen only",
            "citizens only",
            "citizens-only",
            "only",
            "client"
        };

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

                foreach(var item in Positives)
                {
                    if (description.Contains(item, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Scoring++;
                    }
                }

                foreach(var item in Negatives)
                {
                    if (description.Contains(item, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Scoring--;
                    }
                }
            }

            jobList = jobList.OrderByDescending(x => x.Scoring).ToList();
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

        public static List<JobPosting> LoadFromJsonFile(string path)
        {
            return JsonSerializer.Deserialize<List<JobPosting>>(File.ReadAllText(path))!; // TODO: exception handling
        }

        private static async Task GenerateJobListingJsonFileFromMultipleTextFiles()
        {
            var doc = new HtmlDocument();
            var jobListings = new List<JobPosting>();
            var existingLinks = new List<string>();

            for (int i = 1; i < 12; i++)
            {
                var textFile = await File.ReadAllTextAsync($"..\\..\\..\\DataFiles\\DotNetTennessee{i}of11.txt");
                doc.LoadHtml(textFile);

                jobListings.AddRange(ExtractJobs(doc, existingLinks));
            }

            await File.WriteAllTextAsync(Paths.MULTI_PAGE_JSON_FILE_PATH, JsonSerializer.Serialize(jobListings));
        }

        private static async Task GenerateJobListingJsonFileFromSingleTextFile()
        {
            var textFile = await File.ReadAllTextAsync(Paths.RAW_PAGE_SOURCE_FILE_PATH);
            var doc = new HtmlDocument();
            doc.LoadHtml(textFile);

            var jobListings = ExtractJobs(doc, new List<string>());

            await File.WriteAllTextAsync(Paths.SINGLE_PAGE_JSON_FILE_PATH, JsonSerializer.Serialize(jobListings));
        }

        private static List<JobPosting> ExtractJobs(HtmlDocument htmlDocument, List<string> existingLinks)
        {
            var jobList = new List<JobPosting>();

            var liElements = htmlDocument.DocumentNode.SelectNodes("//li").ToList();            

            foreach (var li in liElements)
            {
                var listing = new JobPosting();
                bool isDuplicate = false;

                var links = li.Descendants("a")
                    .Where(a => a.InnerText.Contains("apply", StringComparison.OrdinalIgnoreCase));

                foreach (var link in links)
                {
                    listing.LinksOuterHtml.Add(link.OuterHtml);

                    MatchCollection matches = Regex.Matches(link.OuterHtml, REGEX_URL_PATTERN);

                    foreach (Match match in matches)
                    {
                        // TODO: better checks or importing methods for existing links
                        if(existingLinks.Contains(match.Value))
                        {
                            isDuplicate = true;
                            break;
                        }

                        // listing.Links.Add(match.Value.Replace('"', ' ').Trim()); 
                        listing.Links.Add(match.Value);
                        existingLinks.Add(match.Value);
                    }
                }

                if (isDuplicate) continue;

                listing.InnerText = li.InnerText; // TODO: html decode (ie. convert &amp; to regular &)

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
                        listing.InnerTextCleaned = listing.InnerText;
                    }
                }
                else
                {
                    listing.InnerTextCleaned = listing.InnerText;
                }

                jobList.Add(listing);
            }

            return jobList;
        }

        // TODO: move to seperate class file
        public class JobPosting
        {
            public string InnerText { get; set; } = string.Empty;
            public string InnerTextCleaned { get; set; } = string.Empty;

            public List<string> LinksOuterHtml { get; set; } = new();
            public List<string> Links { get; set; } = new();

            public int Scoring { get; set; } = 0;
        }
    }
}
