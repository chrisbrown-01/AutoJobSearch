using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _contentViewModel;

        [ObservableProperty]
        private JobBoardViewModel _jobBoardViewModel;

        [ObservableProperty]
        private JobSearchViewModel _jobSearchViewModel;

        public MainWindowViewModel()
        {
            JobBoardViewModel = new JobBoardViewModel();
            JobSearchViewModel = new JobSearchViewModel();
            ContentViewModel = JobBoardViewModel;
        }

        private void ChangeViewToJobBoard()
        {
            ContentViewModel = JobBoardViewModel;
        }

        private void ChangeViewToJobSearch()
        {
            ContentViewModel = JobSearchViewModel;
        }
    }
}