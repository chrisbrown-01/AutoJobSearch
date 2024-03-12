using AutoJobSearchGUI.Models;
using System.Collections.Generic;

namespace AutoJobSearchGUI
{
    internal class Singletons
    {
        public static List<ContactModel> Contacts { get; set; } = default!;
        public static List<JobListingModel> JobListings { get; set; } = default!;
    }
}