namespace TextBob.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
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
}
