namespace TextBob.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
    private AppViewModel? _appViewModel;

    /// <summary>
    /// The app view model.
    /// </summary>
    public AppViewModel? AppViewModel
    {
        get
        {
            return _appViewModel;
        }

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
}
