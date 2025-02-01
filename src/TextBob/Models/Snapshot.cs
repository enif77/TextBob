namespace TextBob.Models;

/// <summary>
/// Describes a snapshot file.
/// </summary>
public class Snapshot
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
    /// A flag indicating whether the snapshot is read only.
    /// </summary>
    public bool ReadOnly { get; set; }
    
    /// <summary>
    /// Returns the name of the snapshot.
    /// </summary>
    /// <returns>The name of the snapshot.</returns>
    public override string ToString()
        => ReadOnly ? $"{Name} (R/O)" : Name;
}
