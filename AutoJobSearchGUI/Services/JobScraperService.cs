using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
                // TODO: need to either create an installer or provide instructions to users on how to set permissions for extracted files
                // ie. need to make the GUI and job scraper applications executable, as well as same for the selenium chrome file
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
