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
        public const int MAX_START_PAGE = 20;
        public const string STARTING_INDEX_KEY = "Click to copy link";
        public const string ENDING_INDEX_KEY = "Report this listing"; 
        public const string REGEX_URL_PATTERN = @"https?://[^\s""]+";
        public static readonly List<string> KEYWORDS_POSITIVE = new()
        {
            "first role",
            "first job",
            "canada",
            "canadian",
            "Report to",
            "Reporting to",
            "Recent",
            "new",
            "grad",
            "limited",
            "no experience",
            "Early Career",
            "beginner",
            "junior",
            "entry",
            "small",
            "Fresh",
            "1+",
            "1-",
            "1 year",
            "not required",
            "sponsorship"
        };
        public static readonly List<string> KEYWORDS_NEGATIVE = new()
        {
            "GPA",
            "End Date",
            "contract",
            "intern",
            "not available",
            "unavailable",
            "CANDIDATES ONLY",
            "only",
            "NO C2C",
            "Citizenship Required",
            "citizen only",
            "citizens only",
            "no sponsor",
            "visa required",
            "not our practice",
            "Unable",
            "intended graduation",
            "upcoming graduation",
            "enrolled",
            "US Citizen",
            "local",
            "live in",
            "without supervision",
            "without sponsorship",
            "authorized",
            "Sr",
            "Senior",
            "Highly",
            "5+",
            "6+",
            "7+",
            "8+",
            "9+",
            "10+",
            "5-",
            "6-",
            "7-",
            "8-",
            "9-",
            "10-",
            "5 +",
            "6 +",
            "7 +",
            "8 +",
            "9 +",
            "10 +",
            "5 -",
            "6 -",
            "7 -",
            "8 -",
            "9 -",
            "10 -",
            "5-year",
            "6-year",
            "7-year",
            "8-year",
            "9-year",
            "10-year",
            "5 year",
            "6 year",
            "7 year",
            "8 year",
            "9 year",
            "10 year",
            "5year",
            "6year",
            "7year",
            "8year",
            "9year",
            "10year",
            "client",
        };
        public static readonly List<string> SENTIMENTS_POSITIVE = new()
        {
            "Limited immigration sponsorship visa may be available",
            "We're looking for someone seeking their first role as a software developer",
            "You’ll be given the opportunity to put the computer science fundamentals you’ve learned in school to practical use on live products used by thousands of customers around the world",
            "You’ll receive mentorship and support from highly experienced developers.",
            "REMOTE (US/Canada Residing people only, with work permit)",
            "Reporting directly to one of the Principal Software Engineers, the Junior Developer would be spending most of their time",
            "Recent graduate with Bachelor's Degree in",
            "This is an excellent opportunity for recent graduates or individuals with limited professional experience in.NET development",
            "Early Career C# / .NET Software Engineer",
            "we are looking for a developer new to the industry to join a smaller tech company",
            "Freshers and new graduates are encouraged to apply",
            "join our team as an entry-level .NET Developer",
        };
        public static readonly List<string> SENTIMENTS_NEGATIVE = new()
        {
            "Relocation and housing assistance will not be available for an internship.",
            "LOCAL MASSACHUSSETS CANDIDATES ONLY AND NO C2C PLEASE!",
            "US Citizenship Required",
            "it is not our practice to sponsor individuals for work visas",
        };
    }
}
