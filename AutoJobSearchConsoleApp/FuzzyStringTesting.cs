using FuzzySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchConsoleApp
{
    internal class FuzzyStringTesting
    {
        // TODO: case sensitivity matters so convert listing to upper/lower before running fuzzy methods
        public static void Test1()
        {
            var goodJobs = DataHelpers.GOOD_LISTINGS;
            var badJobs = DataHelpers.BAD_LISTINGS;
            var okJobs = DataHelpers.OK_LISTINGS;

            var goodJob = goodJobs[5];
            var badJob = badJobs[4];
            var okJob = okJobs[0];

            TestGoodJob(goodJobs);
            //TestBadJob(badJob);

            Console.WriteLine();
        }

        private static void TestGoodJob(List<string> goodJob)
        {
            foreach(var listing in goodJob)
            {
                Console.WriteLine("--- Good job ---");
                Console.WriteLine(goodJob);
                Console.WriteLine();

                foreach (var positive in DataHelpers.SENTIMENTS_POSITIVE)
                {
                    var key = positive.ToLower();
                    var job = listing.ToLower();

                    Console.WriteLine();
                    Console.WriteLine($"Positive term: {key}");
                    Console.WriteLine($"Description contains term? {job.Contains(key, StringComparison.OrdinalIgnoreCase)}");
                    Console.WriteLine($"Ratio: {Fuzz.Ratio(key, job)}");
                    Console.WriteLine($"Weighted Ratio: {Fuzz.WeightedRatio(key, job)}");
                    Console.WriteLine($"Partial ratio: {Fuzz.PartialRatio(key, job)}");
                    Console.WriteLine($"Reversed Weighted Ratio: {Fuzz.WeightedRatio(job, key)}");
                    Console.WriteLine($"Reversed Partial ratio: {Fuzz.PartialRatio(job, key)}");
                }
            }
        }

        private static void TestBadJob(string badJob)
        {
            Console.WriteLine("--- Bad job ---");
            //Console.WriteLine(badJob);
            Console.WriteLine();

            foreach (var negative in DataHelpers.SENTIMENTS_NEGATIVE)
            {
                var key = negative.ToLower();
                var job = badJob.ToLower();

                Console.WriteLine();
                Console.WriteLine($"Positive term: {key}");
                Console.WriteLine($"Description contains term? {job.Contains(key, StringComparison.OrdinalIgnoreCase)}");
                //Console.WriteLine($"Ratio: {Fuzz.Ratio(key, job)}");
                Console.WriteLine($"Weighted Ratio: {Fuzz.WeightedRatio(key, job)}");
                Console.WriteLine($"Partial ratio: {Fuzz.PartialRatio(key, job)}");
            }
        }

        public static void TestStringContains()
        {
            string description = "new graduate junior developer freshly graduated no experience required 1-2 years experience entry-level will be reporting to the senior";

            var test1 = description.Contains("new grad");
            var test2 = description.Contains("junior");
            var test3 = description.Contains("1-");
            var test4 = description.Contains("entry");
            var test5 = description.Contains("no experience");
            var test6 = description.Contains("report");

            Console.WriteLine();
        }
    }
}
