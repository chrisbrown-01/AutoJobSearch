using AutoJobSearchGUI.ViewModels;
using AutoJobSearchGUI.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Serilog;
using System.ComponentModel;

namespace AutoJobSearchGUI
{
    public partial class App : Application // Needs to be public for View previewer to work
    {
        private IClassicDesktopStyleApplicationLifetime? desktop;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                desktop = desktopLifetime;

                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);
                desktopLifetime.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                    WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterScreen,
                    WindowState = Avalonia.Controls.WindowState.Maximized
                };

                desktopLifetime.MainWindow.Closing += CloseConnections;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void CloseConnections(object? sender, CancelEventArgs e)
        {
            Log.Information("Closing all connections for application shutdown.");

            if (desktop?.MainWindow?.DataContext is MainWindowViewModel viewModel)
            {
                viewModel.Dispose();
            }
        }
    }
}