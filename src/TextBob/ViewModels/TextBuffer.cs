using MiniMvvm;


namespace TextBob.ViewModels;

/// <summary>
/// View model for the text buffer.
/// </summary>
public class TextBuffer: ViewModelBase
{
    private string _name = Defaults.DefaultSnapshotName;
    
    /// <summary>
    /// A name of the text buffer displayed in the UI.
    /// </summary>
    public string Name
    {
        get => _name;

        set
        {
            if (_name == value)
            {
                return;
            }

            _name = value;
            
            RaisePropertyChanged();
        }
    }
    
    
    private string _text = string.Empty;
    
    /// <summary>
    /// The text.
    /// </summary>
    public string Text
    {
        get => _text;

        set
        {
            if (_text == value)
            {
                return;
            }

            _text = value;
            
            RaisePropertyChanged();
        }
    }
    
    
    private string _path = Defaults.DefaultSnapshotFilePath;
    
    /// <summary>
    /// A path to the file where the text buffer is saved.
    /// </summary>
    public string Path
    {
        get => _path;

        set
        {
            if (_path == value)
            {
                return;
            }

            _path = value;
            
            RaisePropertyChanged();
        }
    }
    
    
    private bool _isReadOnly;
    
    /// <summary>
    /// Indicates whether the text buffer is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => _isReadOnly;

        set
        {
            if (_isReadOnly == value)
            {
                return;
            }

            _isReadOnly = value;
            
            RaisePropertyChanged();
        }
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
