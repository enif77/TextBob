using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Document;

using TextBob.ViewModels;
using TextBob.Views;


namespace TextBob;

public partial class App : Application
{
    public App()
    {
        DataContext = new AppViewModel()
        {
            Name = Defaults.AppName,
            VersionInfo = Defaults.AppVersionInfo
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
                Width = GetMainWindowWidth(),
                Height = GetMainWindowHeight()
            };

            var viewModel = new MainWindowViewModel()
            {
                AppViewModel = (AppViewModel?)DataContext,
                Title = Defaults.AppName,
                FontSize = GetTextEditorFontSize(),
                FontFamily = GetTextEditorFontFamily(),
                ShowLineNumbers = Program.Settings.TextEditorShowLineNumbers,
                TextEditorOptions = new TextEditorOptions()
                {
                    ConvertTabsToSpaces = Program.Settings.TextEditorConvertTabsToSpaces,
                    EnableEmailHyperlinks = Program.Settings.TextEditorEnableEmailHyperlinks,
                    EnableHyperlinks = Program.Settings.TextEditorEnableHyperlinks,
                    HighlightCurrentLine = Program.Settings.TextEditorHighlightCurrentLine,
                    IndentationSize = Program.Settings.TextEditorIndentationSize,
                },
                TextEditorHandler = mainWindow,
                Document = new TextDocument()
            };
            
            // TODO: Load text buffers and assign them to the view model here. VM should not load anything.
            
            viewModel.UpdateTextBuffersList();
            
            mainWindow.DataContext = viewModel;
            desktop.MainWindow = mainWindow;
        }
        
        base.OnFrameworkInitializationCompleted();
    }
    
    
    private int GetMainWindowWidth()
    {
        var width = Program.Settings.MainWindowWidth;
        width = width switch
        {
            < 320 => 320,
            > 4096 => 4096,
            _ => width
        };

        return width;
    }
    
    
    private int GetMainWindowHeight()
    {
        var height = Program.Settings.MainWindowHeight;
        height = height switch
        {
            < 240 => 240,
            > 4096 => 4096,
            _ => height
        };

        return height;
    }
    
    
    private int GetTextEditorFontSize()
    {
        var fontSize = Program.Settings.TextEditorFontSize;
        fontSize = fontSize switch
        {
            < 6 => 6,
            > 72 => 72,
            _ => fontSize
        };

        return fontSize;
    }
    
    
    private string GetTextEditorFontFamily()
    {
        var fontFamily = Program.Settings.TextEditorFontFamily;
        if (string.IsNullOrWhiteSpace(fontFamily))
        {
            fontFamily = "Monospace";
        }

        return fontFamily;
    }
}
