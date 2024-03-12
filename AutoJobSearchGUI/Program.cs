using Avalonia;
using Serilog;
using System;

namespace AutoJobSearchGUI
{
    public class Program // Needs to be public for View previewer to work
    {
        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            // At the time of creating this program, this is how the Avalonia docs recommend to perform global error handling and logging.
            // Through experimentation, exceptions thrown in "async Task" methods do NOT propagate up into the below try-catch block.
            // However, exceptions thrown inside "async void" methods do. So that is why you will see async void methods inside the view models.
            // I am aware that this is normally a major anti-pattern but in this project it is actually the best way of properly throwing exceptions.
            try
            {
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "An unhandled exception occurred");
                throw;
            }
            finally
            {
                Log.Information("Application closed.");
                Log.CloseAndFlush();
            }
        }
    }
}