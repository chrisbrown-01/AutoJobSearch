using AutoJobSearchGUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class JobSearchViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        [ObservableProperty]
        private List<JobSearchProfileModel> _searchProfiles;

        [ObservableProperty]
        private JobSearchProfileModel? _selectedSearchProfile;

        public JobSearchViewModel()
        {
            SearchProfiles = new List<JobSearchProfileModel>();
        }

        //partial void OnSelectedComboBoxItemChanged(string? value)
        //{
        //    if (value.IsNullOrEmpty()) return;
        //    Debug.WriteLine($"selected value is {value}");
        //}
    }
}
