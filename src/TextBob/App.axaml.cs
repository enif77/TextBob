namespace TextBob;

using System;

using Microsoft.Extensions.DependencyInjection;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Document;

using TextBob.Services;
using TextBob.ViewModels;
using TextBob.Views;


public partial class App : Application
{
    private IServiceProvider Services { get; }


    public App()
    {
        Services = ConfigureServices();
        
        var appViewModel = Services.GetService<AppViewModel>()!;

        appViewModel.Name = Defaults.AppName;
            
        DataContext = appViewModel;
    }


    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }


    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var appService = Services.GetService<IAppService>()!;

            var mainWindow = new MainWindow
            {
                Width = appService.GetMainWindowWidth(),
                Height = appService.GetMainWindowHeight()
            };

            var mainWindowViewModel = Services.GetService<MainWindowViewModel>()!;

            mainWindowViewModel.Title = Defaults.AppName;
            mainWindowViewModel.FontSize = appService.GetTextEditorFontSize();
            mainWindowViewModel.FontFamily = appService.GetTextEditorFontFamily();
            mainWindowViewModel.ShowLineNumbers = Program.Settings.TextEditorShowLineNumbers;
            mainWindowViewModel.TextEditorOptions = new TextEditorOptions()
            {
                ConvertTabsToSpaces = Program.Settings.TextEditorConvertTabsToSpaces,
                EnableEmailHyperlinks = Program.Settings.TextEditorEnableEmailHyperlinks,
                EnableHyperlinks = Program.Settings.TextEditorEnableHyperlinks,
                HighlightCurrentLine = Program.Settings.TextEditorHighlightCurrentLine,
                IndentationSize = Program.Settings.TextEditorIndentationSize,
            };
            mainWindowViewModel.TextEditorHandler = mainWindow;
            mainWindowViewModel.Document = new TextDocument();

            // TODO: Load the list of text buffers and assign it here. View mode should get data, not load them from somewhere.

            mainWindowViewModel.UpdateTextBuffersList();
            
            mainWindow.DataContext = mainWindowViewModel;
            desktop.MainWindow = mainWindow;
        }
        
        base.OnFrameworkInitializationCompleted();
    }


    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IAppService, AppService>();

        services.AddSingleton<AppViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
