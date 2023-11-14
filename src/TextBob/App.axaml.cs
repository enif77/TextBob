using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using TextBob.ViewModels;
using TextBob.Views;


namespace TextBob;

public partial class App : Application
{
    private const string AppName = "Text Bob";
    private const string AppVersionInfo = AppName + " 1.0.27";


    public App()
    {
        DataContext = new AppViewModel()
        {
            Name = AppName,
            VersionInfo = AppVersionInfo
        };
    }

    
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
                Width = Program.Settings.MainWindowWidth,
                Height = Program.Settings.MainWindowHeight,

                DataContext = new MainWindowViewModel()
                {
                    AppViewModel = (AppViewModel?)DataContext,
                    Title = AppName,
                    ShowLineNumbers = Program.Settings.TextEditorShowLineNumbers
                }
            };
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}
