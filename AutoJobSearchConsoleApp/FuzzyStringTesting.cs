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
        // TODO: probably just continue simple scoring for short key words, then do a fuzzy logic algorithm based on longer sentences. short terms are not reliable for fuzzy
        public static void Test1()
        {
            var goodJobs = DataHelpers.GOOD_LISTINGS;
            var badJobs = DataHelpers.BAD_LISTINGS;
            var okJobs = DataHelpers.OK_LISTINGS;

            var goodJob = goodJobs[5];
            var badJob = badJobs[4];
            var okJob = okJobs[0];

            TestGoodJob(goodJob);
            //TestBadJob(badJob);

            Console.WriteLine();
        }

        private static void TestGoodJob(string goodJob)
        {
            Console.WriteLine("--- Good job ---");
            //Console.WriteLine(goodJob);
            Console.WriteLine();

            foreach (var positive in DataHelpers.Positives)
            {
                var key = positive.ToLower();
                var job = goodJob.ToLower();

                Console.WriteLine();
                Console.WriteLine($"Positive term: {key}");
                Console.WriteLine($"Description contains term? {job.Contains(key, StringComparison.OrdinalIgnoreCase)}");
                Console.WriteLine($"Ratio: {Fuzz.Ratio(key, job)}");
                Console.WriteLine($"Weighted Ratio: {Fuzz.WeightedRatio(key, job)}");
                Console.WriteLine($"Partial ratio: {Fuzz.PartialRatio(key, job)}");
            }
        }

        private static void TestBadJob(string badJob)
        {
            Console.WriteLine("--- Bad job ---");
            //Console.WriteLine(badJob);
            Console.WriteLine();

            foreach (var negative in DataHelpers.Negatives)
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
    }
}
