using System.Collections.Generic;

namespace TextBob.Models;

using System.Text.Json;

/// <summary>
/// A list of snapshots.
/// </summary>
public class SnapshotsList
{
    #region snapshot files

    /// <summary>
    /// A list of snapshot files.
    /// </summary>
    public IList<Snapshot> Snapshots { get; set; } = new List<Snapshot>();
    
    #endregion


    /// <summary>
    /// Returns empty snapshots list.
    /// </summary>
    public static SnapshotsList DefaultSnapshotsList
    {
        get
        {
            var snapshotsList = new SnapshotsList();

            snapshotsList.Snapshots.Add(new Snapshot()
            {
                Name = "...",
                Path = string.Empty
            });

            return snapshotsList;
        }
    }

    /// <summary>
    /// Converts this instance to JSON.
    /// </summary>
    /// <returns>JSON representation of this instance.</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(
            this,
            JsonSerializerOptions);
    }

    
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };
}
