using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        //[ObservableProperty]
        //private ViewModelBase _contentViewModel;

        //[ObservableProperty]
        //private JobBoardViewModel _jobBoardViewModel;

        //[ObservableProperty]
        //private JobSearchViewModel _jobSearchViewModel;

        public JobBoardViewModel JobBoardViewModel { get; }
        public JobSearchViewModel JobSearchViewModel { get; }

        [ObservableProperty]
        private ViewModelBase _contentViewModel;

        public MainWindowViewModel()
        {
            JobBoardViewModel = new JobBoardViewModel();
            JobSearchViewModel = new JobSearchViewModel();
            ContentViewModel = JobBoardViewModel;
        }

        public void ChangeViewToJobBoard()
        {
            ContentViewModel = JobBoardViewModel;
        }

        public void ChangeViewToJobSearch()
        {
            ContentViewModel = JobSearchViewModel;
        }
    }
}