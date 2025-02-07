namespace TextBob.ViewModels;

using ReactiveUI;


/// <summary>
/// Describes a text buffer.
/// </summary>
public class TextBuffer : ReactiveObject
{
    /// <summary>
    /// Name of the buffer displayed in the UI.
    /// </summary>
    public string DisplayText => ToString();


    private string _name = Defaults.DefaultSnapshotName;

    /// <summary>
    /// Name of the buffer displayed in the UI.
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;

                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(DisplayText));
            }
        }
    }


    private string _text = string.Empty;

    /// <summary>
    /// The text. If null, this buffer was not loaded from a snapshot yet.
    /// </summary>
    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }
    

    private string _path = Defaults.DefaultSnapshotFilePath;

    /// <summary>
    /// Path to the snapshot file.
    /// </summary>
    public string Path
    {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }


    private bool _isReadOnly;

    /// <summary>
    /// Indicates whether the text buffer is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => _isReadOnly;
        set => this.RaiseAndSetIfChanged(ref _isReadOnly, value);
    }


    private bool _isModified;

    /// <summary>
    /// Indicates, that the text has unsaved changes.
    /// </summary>
    public bool IsModified
    {
        get => _isModified;
        set
        {
            if (_isModified != value)
            {
                _isModified = value;

                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(DisplayText));
            }
        }
    }


    private bool _textLoaded;

    /// <summary>
    /// Indicates, that the text was at least once loaded from a snapshot.
    /// </summary>
    public bool TextLoaded
    {
        get => _textLoaded;
        set => this.RaiseAndSetIfChanged(ref _textLoaded, value);
    }


    /// <summary>
    /// Returns the string representation of the text buffer.
    /// </summary>
    /// <returns>The string representation of the text buffer.</returns>
    public override string ToString()
    {
        var info = IsReadOnly
            ? $"{Name} (R/O)"
            : Name;

        return IsModified
            ? $"{info} *"
            : info;
    }
}
