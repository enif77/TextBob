using System.Text.Json;

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
    
    
    #region snapshot file
    
    /// <summary>
    /// A path to the snapshot file.
    /// </summary>
    public string? SnapshotFilePath { get; init; }
    
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

    /// <summary>
    /// Converts this instance to JSON.
    /// </summary>
    /// <returns>JSON representation of this instance.</returns>
    public string? ToJson()
    {
        return JsonSerializer.Serialize(
            new SettingsContainer()
            {
                Settings = this
            },
            new JsonSerializerOptions()
            {
                WriteIndented = true
            });
    }


    private class SettingsContainer
    {
        public Settings? Settings { get; set; }
    }
}
