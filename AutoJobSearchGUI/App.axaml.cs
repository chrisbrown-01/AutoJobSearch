using AutoJobSearchGUI.Services;
using AutoJobSearchGUI.ViewModels;
using AutoJobSearchGUI.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
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

                var services = new ServiceCollection();

                services.AddSingleton<IFilesService>(x => new FilesService(desktopLifetime.MainWindow));

                // TODO: consolidate all manual dependency injections (ex. SQLite database) into similar setup as below
                Services = services.BuildServiceProvider();

                desktopLifetime.MainWindow.Closing += CloseConnections;
            }

            base.OnFrameworkInitializationCompleted();
        }

        public new static App? Current => Application.Current as App;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider? Services { get; private set; }

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