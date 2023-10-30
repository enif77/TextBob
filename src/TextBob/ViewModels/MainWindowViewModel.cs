namespace TextBob.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
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
