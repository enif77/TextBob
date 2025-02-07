namespace TextBob.Services;

using System.Collections.Generic;

using TextBob.Models;


internal interface IAppService
{
    string GetSettingsJson();
    bool SaveSettings(string settingsJson);

    string GetAboutText();

    public IList<Snapshot> GetSnapshotsList();

    /// <summary>
    /// Loads text from a snapshot.
    /// </summary>
    /// <param name="snapshotFilePath">A path to the snapshot file.</param>
    /// <returns>A text from the snapshot.</returns>
    string LoadTextFromSnapshot(string snapshotFilePath);

    /// <summary>
    /// Saves text to a snapshot.
    /// </summary>
    /// <param name="snapshotFilePath">A path to the snapshot file.</param>
    /// <param name="text">A text to be saved to the snapshot file.</param>
    void SaveTextToSnapshot(string snapshotFilePath, string text);


    int GetMainWindowWidth();
    int GetMainWindowHeight();
    int GetTextEditorFontSize();
    string GetTextEditorFontFamily();
}
