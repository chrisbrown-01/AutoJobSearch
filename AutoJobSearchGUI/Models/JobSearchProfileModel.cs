using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Models
{
    public partial class JobSearchProfileModel : ObservableObject
    {
        public int Id { get; }

        [ObservableProperty]
        private string _profileName = string.Empty;

        [ObservableProperty]
        private string _searches = string.Empty;

        [ObservableProperty]
        private string _keywordsPositive = string.Empty;

        [ObservableProperty]
        private string _keywordsNegative = string.Empty;

        [ObservableProperty]
        private string _sentimentsPositive = string.Empty;

        [ObservableProperty]
        private string _sentimentsNegative = string.Empty;
    }
}
