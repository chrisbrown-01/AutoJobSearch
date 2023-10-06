using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _contentViewModel;

        public MainWindowViewModel()
        {
            ContentViewModel = new JobBoardViewModel();
        }

        private void ChangeViewToJobBoard()
        {
            ContentViewModel = new JobBoardViewModel();
        }

        private void ChangeViewToJobSearch()
        {
            ContentViewModel = new JobSearchViewModel();
        }
    }
}