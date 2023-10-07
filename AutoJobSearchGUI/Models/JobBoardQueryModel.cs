using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Models
{
    public partial class JobBoardQueryModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isAppliedTo;

        [ObservableProperty]
        private bool _isInterviewing;

        [ObservableProperty]
        private bool _isRejected;

        [ObservableProperty]
        private bool _isFavourite;

        [ObservableProperty]
        private bool _isHidden;

        [ObservableProperty]
        private bool _orderById;

        [ObservableProperty]
        private bool _orderBySearchTerm;

        [ObservableProperty]
        private bool _orderByCreatedAt;

        [ObservableProperty]
        private bool _orderByScore;
    }
}
