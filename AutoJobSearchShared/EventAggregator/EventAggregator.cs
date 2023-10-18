using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchShared.EventAggregator
{
    public class EventAggregator
    {
        public void StartJobScraper(int jobSearchProfileId)
        {
            // TODO: ensure cross platform
            var process = new Process();
            process.StartInfo.FileName = "..\\AutoJobSearchJobScraper\\AutoJobSearchJobScraper.exe";
            process.StartInfo.Arguments = jobSearchProfileId.ToString();
            process.Start();
        }
    }
}
