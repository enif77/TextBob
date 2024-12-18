using ReactiveUI;


namespace TextBob.ViewModels;

/// <summary>
/// View model for the text buffer.
/// </summary>
public class TextBuffer: ReactiveObject
{
    private string _name = Defaults.DefaultSnapshotName;
    
    /// <summary>
    /// A name of the text buffer displayed in the UI.
    /// </summary>
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    
    
    private string _text = string.Empty;
    
    /// <summary>
    /// The text.
    /// </summary>
    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }
    
    
    private string _path = Defaults.DefaultSnapshotFilePath;
    
    /// <summary>
    /// A path to the file where the text buffer is saved.
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
    
    
    /// <summary>
    /// Returns a string representation of the text buffer.
    /// </summary>
    /// <returns>A string representation of the text buffer.</returns>
    public override string ToString()
    {
        return IsReadOnly ? $"{Name} (R/O)" : Name;
    }
}
