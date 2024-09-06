using System.Collections.Generic;
using System.Text.Json;

namespace TextBob.Models;

/// <summary>
/// Application configuration.
/// </summary>
public class Settings
{
    #region main window
    
    /// <summary>
    /// Initial main window width.
    /// </summary>
    public int MainWindowWidth { get; init; } = Defaults.DefaultMainWindowWidth;
    
    /// <summary>
    /// Initial main window height.
    /// </summary>
    public int MainWindowHeight { get; init; } = Defaults.DefaultMainWindowHeight;
    
    #endregion


    #region text editor

    /// <summary>
    /// The size of the font in the text editor.
    /// </summary>
    public int TextEditorFontSize { get; init; } = Defaults.DefaultFontSize;
    
    /// <summary>
    /// The font family of the font in the text editor.
    /// </summary>
    public string? TextEditorFontFamily { get; init; } = Defaults.DefaultFontFamily;
    
    /// <summary>
    /// Shows line numbers in text editor.
    /// </summary>
    public bool TextEditorShowLineNumbers { get; init; } = Defaults.DefaultShowLineNumbers;
    
    /// <summary>
    /// Converts TABs to SPACEs.
    /// </summary>
    public bool TextEditorConvertTabsToSpaces { get; init; } = Defaults.DefaultConvertTabsToSpaces;

    /// <summary>
    /// Emails are clickable.
    /// </summary>
    public bool TextEditorEnableEmailHyperlinks { get; init; } = Defaults.DefaultEnableEmailHyperlinks;

    /// <summary>
    /// URLs are clickable.
    /// </summary>
    public bool TextEditorEnableHyperlinks { get; init; } = Defaults.DefaultEnableHyperlinks;

    /// <summary>
    /// Highlights the current line.
    /// </summary>
    public bool TextEditorHighlightCurrentLine { get; init; } = Defaults.DefaultHighlightCurrentLine;

    /// <summary>
    /// The number of indentation unit to use in indentation.
    /// </summary>
    public int TextEditorIndentationSize { get; init; } = Defaults.DefaultIndentationSize;
    
    #endregion
    
    
    #region snapshot files
    
    /// <summary>
    /// A list of snapshot files.
    /// </summary>
    public IList<SnapshotFile> Snapshots { get; set; } = new List<SnapshotFile>();
    
    #endregion
    
    
    /// <summary>
    /// Returns default settings.
    /// </summary>
    public static Settings DefaultSettings
        => new();

    /// <summary>
    /// Converts this instance to JSON.
    /// </summary>
    /// <returns>JSON representation of this instance.</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(
            new SettingsContainer()
            {
                Settings = this
            },
            JsonSerializerOptions);
    }

    
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };
    

    private class SettingsContainer
    {
        public Settings? Settings { get; set; }
    }
}