﻿using AutoJobSearchConsoleApp.Models;
using FuzzySharp;
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Net.Http.Headers;

namespace AutoJobSearchConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            /*
            //await SeleniumTesting.Execute();


            //await LocalFileTesting.CreateJsonFiles();
            //LocalFileTesting.LoadFromJsonFileTests();
            //LocalFileTesting.ScoringTest(); 
            //LocalFileTesting.CheckForDuplicatesTest();

            //FuzzyStringTesting.Test1();
            //FuzzyStringTesting.TestStringContains();

            //SQLiteTesting.CreateDb();
            //await SQLiteTesting.PopulateDb(LocalFileTesting.LoadFromJsonFile(Paths.MULTI_PAGE_JSON_FILE_PATH));
            //await SQLiteTesting.GetAllLinks();
            //await SQLiteTesting.UpdateJobListing();
            */
        }


        private void ApplyScoringsParallelAsync(
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


        private void ApplyScoringsSingleThreadAsync(
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