using System;
using AvaloniaEdit.Utils;

namespace TextBob;

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
    public IServiceProvider Services { get; private set; }


    public App()
    {
        DataContext = new AppViewModel()
        {
            Name = Defaults.AppName
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
            Services = ConfigureServices();

            var appService = Services.GetService<IAppService>()!;

            var mainWindow = new MainWindow
            {
                Width = appService.GetMainWindowWidth(),
                Height = appService.GetMainWindowHeight()
            };

            var viewModel = Services.GetService<MainWindowViewModel>()!;

            viewModel.Title = Defaults.AppName;
            viewModel.FontSize = appService.GetTextEditorFontSize();
            viewModel.FontFamily = appService.GetTextEditorFontFamily();
            viewModel.ShowLineNumbers = Program.Settings.TextEditorShowLineNumbers;
            viewModel.TextEditorOptions = new TextEditorOptions()
            {
                ConvertTabsToSpaces = Program.Settings.TextEditorConvertTabsToSpaces,
                EnableEmailHyperlinks = Program.Settings.TextEditorEnableEmailHyperlinks,
                EnableHyperlinks = Program.Settings.TextEditorEnableHyperlinks,
                HighlightCurrentLine = Program.Settings.TextEditorHighlightCurrentLine,
                IndentationSize = Program.Settings.TextEditorIndentationSize,
            };
            viewModel.TextEditorHandler = mainWindow;
            viewModel.Document = new TextDocument();

            // TODO: Load the list of text buffers and assign it here. View mode should get data, not load them from somewhere.

            viewModel.UpdateTextBuffersList();
            
            mainWindow.DataContext = viewModel;
            desktop.MainWindow = mainWindow;
        }
        
        base.OnFrameworkInitializationCompleted();
    }


    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IAppService, AppService>();

        services.AddSingleton<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
