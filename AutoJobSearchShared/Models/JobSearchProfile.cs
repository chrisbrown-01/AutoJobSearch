using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchShared.Models
{
    public class JobSearchProfile
    {
        public JobSearchProfile()
        {
            
        }

        // TODO: delete
        //public JobSearchProfile(
        //    int id = 0, 
        //    string profileName = "New Profile",
        //    string searches = "",
        //    string keywordsPositive = "",
        //    string keywordsNegative = "",
        //    string sentimentsPositive = "",
        //    string sentimentsNegative = "")
        //{
        //    Id = id;
        //    ProfileName = profileName;
        //    Searches = searches;
        //    KeywordsPositive = keywordsPositive;
        //    KeywordsNegative = keywordsNegative;
        //    SentimentsPositive = sentimentsPositive;
        //    SentimentsNegative = sentimentsNegative;
        //}

        public int Id { get; }

        public string ProfileName { get; set; } = "New Profile";

        public string Searches { get; set; } = string.Empty;

        public string KeywordsPositive { get; set; } = string.Empty;

        public string KeywordsNegative { get; set; } = string.Empty;

        public string SentimentsPositive { get; set; } = string.Empty;

        public string SentimentsNegative { get; set; } = string.Empty;
    }
}
