using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoJobSearchShared.Helpers
{
    public static class StringHelpers
    {
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
    }
}
