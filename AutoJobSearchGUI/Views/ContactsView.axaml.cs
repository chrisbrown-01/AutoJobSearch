using Avalonia.Controls;

namespace AutoJobSearchGUI.Views
{
    public partial class ContactsView : UserControl
    {
        public ContactsView()
        {
            InitializeComponent();
        }

        private void AutoCompleteBox_DropDownClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}