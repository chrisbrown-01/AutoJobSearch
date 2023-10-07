using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private JobBoardViewModel jobBoardViewModel;
        private JobSearchViewModel jobSearchViewModel;

        [ObservableProperty]
        private ViewModelBase _contentViewModel;

        public MainWindowViewModel()
        {
            jobBoardViewModel = new JobBoardViewModel();
            jobSearchViewModel = new JobSearchViewModel();
            ContentViewModel = jobBoardViewModel;
        }

        public void ChangeViewToJobBoard()
        {
            ContentViewModel = jobBoardViewModel;
        }

        public void ChangeViewToJobSearch()
        {
            ContentViewModel = jobSearchViewModel;
        }
    }
}