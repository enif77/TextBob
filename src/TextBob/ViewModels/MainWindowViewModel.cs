using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

using MiniMvvm;


namespace TextBob.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
    #region properties
    
    private AppViewModel? _appViewModel;

    /// <summary>
    /// The app view model.
    /// </summary>
    public AppViewModel? AppViewModel
    {
        get => _appViewModel;

        set
        {
            if (_appViewModel == value)
            {
                return;
            }

            _appViewModel = value;
            
            RaisePropertyChanged();
        }
    }


    private string? _title;

    /// <summary>
    /// The main window title.
    /// </summary>
    public string? Title
    {
        get => _title;

        set
        {
            if (_title == value)
            {
                return;
            }

            _title = value;
            RaisePropertyChanged();
        }
    }
    
    
    private bool _showLineNumbers;

    /// <summary>
    /// Shows or hides line numbers in the left column.
    /// </summary>
    public bool ShowLineNumbers
    {
        get => _showLineNumbers;

        set
        {
            if (_showLineNumbers == value)
            {
                return;
            }

            _showLineNumbers = value;

            RaisePropertyChanged();
        }
    }
    
    
    private bool _convertTabsToSpaces;

    /// <summary>
    /// Converts tabs to spaces during text indentation.
    /// </summary>
    public bool ConvertTabsToSpaces
    {
        get => _convertTabsToSpaces;

        set
        {
            if (_convertTabsToSpaces == value)
            {
                return;
            }

            _convertTabsToSpaces = value;

            RaisePropertyChanged();
        }
    }


    private bool _enableEmailHyperlinks;

    /// <summary>
    /// Enables emails to be clickable.
    /// </summary>
    public bool EnableEmailHyperlinks
    {
        get => _enableEmailHyperlinks;

        set
        {
            if (_enableEmailHyperlinks == value)
            {
                return;
            }

            _enableEmailHyperlinks = value;

            RaisePropertyChanged();
        }
    }


    private bool _enableHyperlinks;

    /// <summary>
    /// Enables hyperlinks to be clickable.
    /// </summary>
    public bool EnableHyperlinks
    {
        get => _enableHyperlinks;

        set
        {
            if (_enableHyperlinks == value)
            {
                return;
            }

            _enableHyperlinks = value;

            RaisePropertyChanged();
        }
    }


    private bool _highlightCurrentLine;

    /// <summary>
    /// Enables hyperlinks to be clickable.
    /// </summary>
    public bool HighlightCurrentLine
    {
        get => _highlightCurrentLine;

        set
        {
            if (_highlightCurrentLine == value)
            {
                return;
            }

            _highlightCurrentLine = value;

            RaisePropertyChanged();
        }
    }


    private int _indentationSize = 4;

    /// <summary>
    /// The number of indentation elements to use to intend text. 4 by default
    /// </summary>
    public int IndentationSize
    {
        get => _indentationSize;

        set
        {
            if (_indentationSize == value)
            {
                return;
            }

            _indentationSize = value;

            RaisePropertyChanged();
        }
    }
    
    #endregion
    
    
    #region commands
    
    public MiniCommand AboutCommand { get; }

    public MiniCommand ExitCommand { get; }
    
    #endregion
    
    
    #region ctor
    
    public MainWindowViewModel()
    {
        AboutCommand = MiniCommand.CreateFromTask(async () =>
        {
            if (AppViewModel == null)
            {
                return;
            }
            
            await AppViewModel.ShowAboutWindow();
            
            // var dialog = new AboutAvaloniaDialog();
            //
            // if ((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow is { } mainWindow)
            // {
            //     await dialog.ShowDialog(mainWindow);
            // }
        });
        
        ExitCommand = MiniCommand.Create(() =>
        {
            (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Shutdown();
        });
    }
    
    #endregion
}
