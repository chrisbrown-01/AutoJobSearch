using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchJobScraper.Constants
{
    // TODO: convert to config files
    internal static class ConfigVariables
    {
        public const int MAX_START_PAGE = 150;
        public const string STARTING_INDEX_KEY = "CollapseJob description";
        public const string ENDING_INDEX_KEY = "Report this listing"; 
        public const string REGEX_URL_PATTERN = @"https?://[^\s""]+";
    }
}
