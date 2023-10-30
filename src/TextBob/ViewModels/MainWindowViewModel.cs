namespace TextBob.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
    private string? _title;

    /// <summary>
    /// The main window title.
    /// </summary>
    public string? Title
    {
        get
        {
            return _title;
        }

        set
        {
            if (_title != value)
            {
                _title = value;
                RaisePropertyChanged();
            }
        }
    }
}
