using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TextBob.ViewModels;
using TextBob.Views;


namespace TextBob;

public partial class App : Application
{
    private const string AppVersionInfo = "Text Bob 1.0.23";


    public App()
    {
        DataContext = new AppViewModel()
        {
            Name = AppVersionInfo
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
            var mainWindow = new MainWindow
            {
                Width = Program.Settings.MainWindowWidth,
                Height = Program.Settings.MainWindowHeight,

                DataContext = new MainWindowViewModel()
                {
                    AppViewModel = (AppViewModel?)DataContext,
                    Title = AppVersionInfo
                }
            };

            mainWindow.SetShowLineNumbers(Program.Settings.TextEditorShowLineNumbers);
            
            desktop.MainWindow = mainWindow;
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}
