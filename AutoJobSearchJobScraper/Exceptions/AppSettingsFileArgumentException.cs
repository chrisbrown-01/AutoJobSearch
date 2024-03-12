namespace AutoJobSearchJobScraper.Exceptions
{
    internal class AppSettingsFileArgumentException : Exception
    {
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

        public string? ConfigSettingName { get; set; }
    }
}