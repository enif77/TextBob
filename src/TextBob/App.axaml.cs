using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace TextBob;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                // Get the initial window size from the settings.
                Width = Program.Settings.MainWindowWidth,
                Height = Program.Settings.MainWindowHeight
            };
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}
