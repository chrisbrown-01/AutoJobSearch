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
            var process = new Process();
            process.StartInfo.FileName = "AutoJobSearchJobScraper.dll";
            process.StartInfo.Arguments = jobSearchProfileId.ToString();
            process.Start();
        }
    }
}
