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
        public void OnStartConsoleAppEvent()
        {
            Debug.WriteLine("starting console app"); // TODO: proper logging
            // TODO: figure out how to start executable after program is installed, try using relative paths?
            //var process = new System.Diagnostics.Process();
            //process.StartInfo.FileName = "C:\\Users\\chris\\Documents\\GitHub\\AutoJobSearch\\AutoJobSearchConsoleApp\\bin\\Debug\\net7.0\\AutoJobSearchConsoleApp.exe";
            //process.Start();
        }
    }
}
