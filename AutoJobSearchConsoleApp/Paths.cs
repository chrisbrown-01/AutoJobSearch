using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchConsoleApp
{
    internal static class Paths
    {
        public const string SINGLE_PAGE_JSON_FILE_PATH = "..\\..\\..\\DataFiles\\singlePageResult.json";
        public const string MULTI_PAGE_JSON_FILE_PATH = "..\\..\\..\\DataFiles\\multiPageResult.json";
        public const string RAW_PAGE_SOURCE_FILE_PATH = "..\\..\\..\\DataFiles\\DotNetTennesseJobsSeachStart0.txt"; // 10 jobList items, each have atleast 1 apply link
    }
}
