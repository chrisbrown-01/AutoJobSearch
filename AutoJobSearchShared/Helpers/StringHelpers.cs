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
            // TODO: extract to config file?
            string pattern = @"([a-z]|[.])([A-Z])";
            string replacement = "$1\n$2";
            string result = Regex.Replace(input, pattern, replacement);

            return result;
        }

        public static IEnumerable<string> ConvertCommaSeperatedStringsToIEnumerable(string commaSeperatedStrings)
        {
            // TODO: remove any null/whitespace entries, if last character of final split string is a comma then remove it?

            return commaSeperatedStrings.Split(',')
                                        .Select(s => s.Replace("\r\n", string.Empty)) // Remove newline sequences
                                        .ToList();
        }
    }
}
