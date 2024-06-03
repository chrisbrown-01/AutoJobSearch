using System;
using System.Text.RegularExpressions;

namespace AutoJobSearchShared.Helpers
{
    public static class StringHelpers
    {
        public static readonly HashSet<string> CommonWords = new HashSet<string>
        {
            "the", "be", "to", "of", "and", "a", "in", "that", "have", "I",
            "it", "for", "not", "on", "with", "he", "as", "you", "do",
            "at", "this", "but", "his", "by", "from", "they", "we", "say",
            "her", "she", "or", "an", "will", "my", "one", "all", "would",
            "there", "their", "what", "so", "up", "out", "if", "about", "who",
            "get", "which", "go", "me", "over", "then", "them", "these", "some",
            "such", "only", "see", "other", "than", "now", "its", "our", "even",
            "new", "just", "each", "more", "also", "how", "had", "into", "two",
            "well", "time", "may", "after", "where", "many", "made", "did", "must",
            "your", "before", "any", "upon", "most", "us", "own", "can", "very",
            "through", "do", "don’t", "it's", "no", "down", "year", "when", "out", "was",
            "Read", "are", "details", "type", "you’ll", "reviews", "Toronto", "team",
            "various", "site", "Job", "jobs", "full", "is", "code", "work",
            "years", "+", "Inc", "llc", "hour", "day", "minute", "second", "first", "Event",
            "Live", "people", "Host", "company", "skill", "skills", "skilled", "part",
            "Part-time", "full", "full-time", "schedule", "needed", "Charge", "new",
            "experience", "experienced", "center", "middle", "apply", "applied", "science",
            "multiple", "issue", "provide", "CA", "USA", "NA", "align", "bros", "brother",
            "plus", "sister", "mother", "father", "remarkable", "culture", "related", "degree",
            "push", "pull", "industry", "content", "Federal", "understand", "NY", "York", "()",
            "NYC", "role", "we're", "downtown", "both", "product", "written", "directly",
            "write", "writing", "type", "typing", "Skills:", "group", "platform", "Creating", "features",
            "looking", "look", "north", "south", "east", "west", "use", "using", "useful", "save",
            "sign", "years’", "create", "change", "ago", "share", "Strong", "ad", "advertisement",
            "issues", "Markets", "initiatives", "ongoing", "plan", "world", "compatible", "life", "live",
            "including", "Assist", "term", "cases", "ability", "British", "Columbia", "position", "business",
            "clients", "client", "priorities", "others", "Nation", "canada", "usa", "america", "united", "states",
            "ability", "British", "Columbia", "position", "business", "clients", "client", "priorities", "others",
            "Nation", "canada", "usa", "america", "united", "states", "world", "worlds", "worldwide", "world-wide",
            "help", "opportunity", "CDL", "user", "users", "check", "we", "we've", "we're", "been", "here", "hear",
            "want", "trip", "back", "love", "scan", "product", "products", "rocks", "rock", "you", "you'll", "will",
            "data", "us", "student", "students", "online", "human", "person", "practice", "practices", "division",
            "divide", "multiple", "meet", "high", "low", "and/or", "or", "care", "highly", "value", "based",
            "learn", "work", "working", "network", "global", "effective", "effectively", "process", "processing",
            "position", "positioning", "travel", "traveling", "days", "day", "join", "joins", "joined",
            "opportunity", "ideal", "status", "mission", "experience", "experiences", "build", "building", "like",
            "where", "do", "Health", "personalf", "personals", "centre", "center", "less", "available", "current",
            "currently", "task", "tasks", "computer", "private", "corporation", "llc", "establish", "establishment",
            "more", "most", "etc", "etc.", "sr", "senior", "junior", "love", "like", "loves", "likes", "prefer",
            "prefers", "keep", "towards", "make", "makes", "create", "creates", "created", "creation", "grow",
            "growing", "believe", "believed", "believes", "growed", "make", "benefit", "benefits", "salary",
            "range", "annual", "annum", "identify", "identifies", "meet", "meets", "met", "square", "circle",
            "triangle", "shape", "shapes", "shaped", "performance", "join", "joins", "joined", "real", "reality",
            "mission", "tech", "technology", "technological", "better", "best", "high", "higher", "highest", "low",
            "lower", "lowest", "take", "taking", "took", "ways", "way", "wayward", "least", "most", "work", "workload",
            "purpose", "purported", "modern", "every", "all", "different", "drive", "driver", "join", "detail", "details",
            "great", "staff", "staffing", "work", "working", "bonus", "wage", "wages", "salary", "compensation",
            "compensated", "per", "project", "projects", "student", "students", "candidate", "overview", "preferred",
            "prefer", "preferential", "solution", "pay", "open", "close", "closed", "opened", "opening", "opener",
            "closer", "role", "roles", "loop", "field", "limit", "limited", "address", ".", ",", ";", "`", "'", "+",
            "-", "/", "\\", "?", "!", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "$", "#", "@", "%", "*", "(",
            ")", "{", "}", "[", "]", ":", "\"", "connect", "connected", "current", "currently", "review", "reviewed",
            "reviews", "within", "with", "like", "working", "work", "worked", "super", "good", "great", "excellent",
            "excellence", "3d", "4d", "mp3", "area", "support", "for", "blue", "red", "black", "orange", "colour",
            "color", "colours", "colors", "coloured", "colored", "key", "keys", "responsibilities", "responsible",
            "responsibility", "leverage", "leveraged", "leveraging", "minimum", "maximum", "min", "max", "min.",
            "max.", "prefer", "preferred", "preferable", "again", "ensure", "ensured"
        };

        public static string AddNewLinesToMisformedString(string input)
        {
            string pattern = @"([a-z]|[.])([A-Z])";
            string replacement = "$1\n$2";
            string result = Regex.Replace(input, pattern, replacement);

            return result;
        }

        public static IEnumerable<string> ConvertCommaSeperatedStringsToIEnumerable(string commaSeperatedStrings)
        {
            return commaSeperatedStrings.Split(',')
                                        .Select(s => s.Replace("\r\n", string.Empty)) // Remove newline sequences
                                        .Where(s => !string.IsNullOrWhiteSpace(s) && s.Length >= 2); // Filter out null, whitespace, or strings with less than 2 characters
        }

        public static IEnumerable<string> HighestFrequencyWordsInString(string textInput, HashSet<string> textToIgnore, int maxNumOfWords)
        {
            // Matches lone numbers, lone punctuation, commas at beginning and end of word, periods at end of word, any 1-length words
            const string REGEX_PATTERN = @"(?<=\s|^),|,(?=\s|$)|(?<=\s|^)\b\d+\b|(?<=\s|^)[^\w\d\s]+(?=\s|$)|\b\.\B|\b\w\b";

            var cleanedText = Regex.Replace(textInput, REGEX_PATTERN, "");
            var words = cleanedText.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Count the frequency of each word
            var wordFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var word in words)
            {
                if (textToIgnore.Contains(word, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (wordFrequency.ContainsKey(word))
                {
                    wordFrequency[word]++;
                }
                else
                {
                    wordFrequency[word] = 1;
                }
            }

            return wordFrequency.OrderByDescending(kvp => kvp.Value).Take(maxNumOfWords).Select(x => x.Key);
        }
    }
}