using Serilog;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AutoJobSearchGUI.Services
{
    internal class JobScraperService
    {
        public static void StartJobScraper(int jobSearchProfileId)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Log.Information("Starting job scraper for Windows platform.");
                var process = new Process();
                process.StartInfo.FileName = "..\\AutoJobSearchJobScraper\\AutoJobSearchJobScraper.exe";
                process.StartInfo.Arguments = jobSearchProfileId.ToString();
                process.Start();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Log.Information("Starting job scraper for Linux platform.");
                var process = new Process();
                process.StartInfo.FileName = "../AutoJobSearchJobScraper/AutoJobSearchJobScraper";
                process.StartInfo.Arguments = jobSearchProfileId.ToString();
                process.Start();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Log.Information("Starting job scraper for OSX platform.");
                var process = new Process();
                process.StartInfo.FileName = "../AutoJobSearchJobScraper/AutoJobSearchJobScraper";
                process.StartInfo.Arguments = jobSearchProfileId.ToString();
                process.Start();
            }
            else
            {
                Log.Error("The job scraper could not be started because the OS platform could not be determined.");
            }
        }
    }
}