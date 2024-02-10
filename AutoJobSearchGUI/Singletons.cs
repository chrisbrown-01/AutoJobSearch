using AutoJobSearchGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI
{
    internal class Singletons()
    {
        public static List<JobListingModel> JobListings { get; set; } = default!;
    }
}
