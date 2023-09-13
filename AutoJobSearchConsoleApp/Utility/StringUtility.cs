using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoJobSearchConsoleApp.Utility
{
    public class StringUtility
    {
        public static string AddNewLinesToMisformedString(string input)
        {
            string pattern = @"([a-z]|[.])([A-Z])";
            string replacement = "$1\n$2";
            string result = Regex.Replace(input, pattern, replacement);

            return result;
        }
    }
}
