using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchJobScraper.Exceptions
{
    internal class AppSettingsFileArgumentException : Exception
    {
        public string? ConfigSettingName { get; set; }

        public AppSettingsFileArgumentException()
        {
        }

        public AppSettingsFileArgumentException(string message) : base(message)
        {
        }

        public AppSettingsFileArgumentException(string message, string configSettingName) : base(message)
        {
            ConfigSettingName = configSettingName;
        }

        public AppSettingsFileArgumentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
