using AutoJobSearchShared.Database;
using AutoJobSearchShared.Helpers;
using AutoJobSearchShared.Models;
using Dapper;
using FuzzySharp;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace testToDelete
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            string SQLITE_CONNECTION_STRING = "Data Source=C:\\Users\\chris\\Documents\\GitHub\\AutoJobSearch\\AutoJobSearchConsoleApp\\AutoJobSearch.db";

            using var connection = new SqliteConnection(SQLITE_CONNECTION_STRING);

            var jobs = connection.Query<JobListing>("SELECT * FROM JobListings;");
            var profile = connection.QueryFirstOrDefault<JobSearchProfile>("SELECT * FROM JobSearchProfiles;");

            var stopwatch = Stopwatch.StartNew();

            ApplyScoringsSingleThread(jobs,
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(profile.KeywordsPositive), 
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(profile.KeywordsNegative), 
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(profile.SentimentsPositive),
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(profile.SentimentsNegative));

            stopwatch.Stop();
            Console.WriteLine($"Time taken for ApplyScoringsSingleThread: {stopwatch.Elapsed}");
            stopwatch.Restart();

            ApplyScoringsParallel(jobs,
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(profile.KeywordsPositive),
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(profile.KeywordsNegative),
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(profile.SentimentsPositive),
                StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(profile.SentimentsNegative));

            stopwatch.Stop();
            Console.WriteLine($"Time taken for ApplyScoringsParallel: {stopwatch.Elapsed}");
        }

        private static void ApplyScoringsParallel(
IEnumerable<JobListing> jobListingsUnscored,
IEnumerable<string> keywordsPositive,
IEnumerable<string> keywordsNegative,
IEnumerable<string> sentimentsPositive,
IEnumerable<string> sentimentsNegative)
        {
            sentimentsPositive = sentimentsPositive.Select(s => s.ToLower());
            sentimentsNegative = sentimentsNegative.Select(s => s.ToLower());

            var jobList = jobListingsUnscored.ToList();

            Parallel.ForEach(jobList, job =>
            {
                foreach (var keyword in keywordsPositive)
                {
                    if (job.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Score++;
                    }
                }

                foreach (var item in keywordsNegative)
                {
                    if (job.Description.Contains(item, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Score--;
                    }
                }

                foreach (var sentiment in sentimentsPositive)
                {
                    if (Fuzz.WeightedRatio(sentiment, job.Description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment, job.Description.ToLower()) >= 50)
                    {
                        job.Score++;
                    }
                }

                foreach (var sentiment in sentimentsNegative)
                {
                    if (Fuzz.WeightedRatio(sentiment, job.Description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment, job.Description.ToLower()) >= 50)
                    {
                        job.Score--;
                    }
                }
            });
        }


        private static void ApplyScoringsSingleThread(
                IEnumerable<JobListing> jobListingsUnscored,
                IEnumerable<string> keywordsPositive,
                IEnumerable<string> keywordsNegative,
                IEnumerable<string> sentimentsPositive,
                IEnumerable<string> sentimentsNegative)
        {
            sentimentsPositive = sentimentsPositive.Select(s => s.ToLower());
            sentimentsNegative = sentimentsNegative.Select(s => s.ToLower());

            var jobList = jobListingsUnscored.ToList();


            foreach (var job in jobList)
            {
                foreach (var keyword in keywordsPositive)
                {
                    if (job.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Score++;
                    }
                }

                foreach (var item in keywordsNegative)
                {
                    if (job.Description.Contains(item, StringComparison.OrdinalIgnoreCase))
                    {
                        job.Score--;
                    }
                }

                foreach (var sentiment in sentimentsPositive)
                {
                    if (Fuzz.WeightedRatio(sentiment, job.Description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment, job.Description.ToLower()) >= 50)
                    {
                        job.Score++;
                    }
                }

                foreach (var sentiment in sentimentsNegative)
                {
                    if (Fuzz.WeightedRatio(sentiment, job.Description.ToLower()) >= 50 &&
                       Fuzz.PartialRatio(sentiment, job.Description.ToLower()) >= 50)
                    {
                        job.Score--;
                    }
                }
            }
        }

    }
}