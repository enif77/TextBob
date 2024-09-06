namespace TextBob.Models;

/// <summary>
/// Describes a snapshot file.
/// </summary>
public class SnapshotFile
{
    /// <summary>
    /// Name of the snapshot displayed in the UI.
    /// </summary>
    public string Name { get; set; } = Defaults.DefaultSnapshotName;
    
    /// <summary>
    /// Path to the snapshot file.
    /// </summary>
    public string Path { get; set; } = Defaults.DefaultSnapshotFilePath;

    
    /// <summary>
    /// Returns the name of the snapshot.
    /// </summary>
    /// <returns>The name of the snapshot.</returns>
    public override string ToString()
        => Name;
}
