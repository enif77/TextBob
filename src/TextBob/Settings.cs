namespace TextBob;

/// <summary>
/// Application configuration.
/// </summary>
public class Settings
{
    private const int DefaultMainWindowWidth = 800;
    private const int DefaultMainWindowHeight = 600;
    
    
    #region main window
    
    /// <summary>
    /// Initial main window width.
    /// </summary>
    public int MainWindowWidth { get; init; } = DefaultMainWindowWidth;
    
    /// <summary>
    /// Initial main window height.
    /// </summary>
    public int MainWindowHeight { get; init; } = DefaultMainWindowHeight;
    
    #endregion


    #region text editor

    /// <summary>
    /// Shows line numbers in text editor.
    /// </summary>
    public bool TextEditorShowLineNumbers { get; init; }

    #endregion
    
    
    /// <summary>
    /// Returns default settings.
    /// </summary>
    public static Settings DefaultSettings
        => new()
        {
            MainWindowWidth = DefaultMainWindowWidth,
            MainWindowHeight = DefaultMainWindowHeight
        };
}
