namespace TextBob;

/// <summary>
/// Application configuration.
/// </summary>
public class Settings
{
    /// <summary>
    /// Initial main window width.
    /// </summary>
    public int MainWindowWidth { get; init; } = 800;
    
    /// <summary>
    /// Initial main window height.
    /// </summary>
    public int MainWindowHeight { get; init; } = 600;
    
    
    /// <summary>
    /// Returns default settings.
    /// </summary>
    public static Settings DefaultSettings
        => new()
        {
            MainWindowWidth = 800,
            MainWindowHeight = 600
        };
}
