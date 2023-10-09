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
    }
}
