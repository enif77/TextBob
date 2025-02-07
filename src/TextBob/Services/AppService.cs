namespace TextBob.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using Avalonia.Platform;

using TextBob.Models;


internal class AppService : IAppService
{
    public string GetSettingsJson()
    {
        var settingsFilePath = Program.GetSettingsFilePath();

        return File.Exists(settingsFilePath)
            ? LoadTextFromSnapshot(settingsFilePath)
            : Program.Settings.ToJson();
    }


    public bool SaveSettings(string settingsJson)
    {
        try
        {
            // Validate settings.
            _ = JsonSerializer.Deserialize<Settings>(settingsJson);

            // Save settings.
            File.WriteAllText(Program.GetSettingsFilePath(), settingsJson);

            return true;
        }
        catch (Exception)
        {
            // TODO: Log the exception.

            return false;
        }
    }


    public string GetAboutText()
    {
        // https://docs.avaloniaui.net/docs/basics/user-interface/assets

        using (var reader = new StreamReader(AssetLoader.Open(new Uri("avares://TextBob/Assets/Windows/about.txt")), System.Text.Encoding.UTF8))
        {
            return reader.ReadToEnd()
                .Replace("${version-info}", Defaults.AppVersionInfo);
        }
    }


    public IList<Snapshot> GetSnapshotsList()
    {
        var snapshotsDirectoryPath = GetSnapshotsDirectoryPath(Program.Settings);

        var snapshotsListFilePath = Path.Combine(snapshotsDirectoryPath, Defaults.SnapshotsListFileName);
        if (File.Exists(snapshotsListFilePath) == false)
        {
            File.WriteAllText(snapshotsListFilePath, SnapshotsList.DefaultSnapshotsList.ToJson());
        }

        var snapshotsMap = new Dictionary<string, string>();
        var snapshotsList = new List<Snapshot>();

        // Load the list of snapshots from the settings.
        ProcessSnapshotsList(Program.Settings.Snapshots, snapshotsDirectoryPath, snapshotsMap, snapshotsList);

        // Load the list of snapshots from the snapshots list file.
        var userDefinedSnapshotsList = JsonSerializer.Deserialize<SnapshotsList>(File.ReadAllText(snapshotsListFilePath)) ??
                                       SnapshotsList.DefaultSnapshotsList;
        ProcessSnapshotsList(userDefinedSnapshotsList.Snapshots, snapshotsDirectoryPath, snapshotsMap, snapshotsList);

        return snapshotsList;
    }


    private void ProcessSnapshotsList(
        IList<Snapshot> snapshots,
        string snapshotsDirectoryPath,
        Dictionary<string, string> snapshotsMap,
        List<Snapshot> snapshotsList)
    {
        foreach (var userDefinedSnapshot in snapshots)
        {
            // The ellipsis is a marker, that loads all snapshots from the snapshot directory, that are not already loaded.
            // Use this at the end of the list of snapshots.
            if (userDefinedSnapshot.Name == "...")
            {
                // Load the list of snapshots from the snapshots directory.
                var directSnapshotFilePathsList = GetSnapshotFilePaths(snapshotsDirectoryPath);
                foreach (var directSnapshotFilePath in directSnapshotFilePathsList)
                {
                    // Skip snapshots defined in the user defined list of snapshots.
                    if (snapshotsMap.ContainsKey(directSnapshotFilePath))
                    {
                        continue;
                    }

                    var snapshot = new Snapshot
                    {
                        Name = Path.GetFileName(directSnapshotFilePath),
                        Path = directSnapshotFilePath
                    };

                    snapshotsMap[directSnapshotFilePath] = directSnapshotFilePath;
                    snapshotsList.Add(snapshot);
                }

                continue;
            }

            // Skip entries without a path.
            var snapshotFilePath = userDefinedSnapshot.Path;
            if (string.IsNullOrWhiteSpace(snapshotFilePath))
            {
                continue;
            }

            // If the path is relative to the user home directory, make it absolute.
            if (snapshotFilePath.StartsWith("~/") || snapshotFilePath.StartsWith("~\\"))
            {
                snapshotFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    snapshotFilePath[2..]);
            }

            // If the path is relative to the current directory, make it absolute using the snapshots directory path.
            else if (snapshotFilePath.StartsWith("./") || snapshotFilePath.StartsWith(".\\"))
            {
                snapshotFilePath = Path.Combine(
                    snapshotsDirectoryPath,
                    snapshotFilePath[2..]);
            }

            // Try to find the file. If it exists or if it is an absolute path, do nothing.
            // If not, try to find it in the snapshots directory.
            else if (Path.IsPathRooted(snapshotFilePath) == false)
            {
                if (File.Exists(snapshotFilePath) == false)
                {
                    // Add the snapshots directory st the root of this snapshot path.
                    snapshotFilePath = Path.Combine(
                        snapshotsDirectoryPath,
                        snapshotFilePath);
                }
            }

            // Do not add duplicities.
            if (snapshotsMap.ContainsKey(snapshotFilePath))
            {
                continue;
            }

            // Change the path to the absolute path.
            userDefinedSnapshot.Path = snapshotFilePath;

            // Remember the snapshot.
            snapshotsMap[snapshotFilePath] = snapshotFilePath!;
            snapshotsList.Add(userDefinedSnapshot);
        }
    }


    private IList<string> GetSnapshotFilePaths(string snapshotsDirectoryPath)
    {
        var snapshotFilePaths = Directory.GetFiles(snapshotsDirectoryPath, "*.*");

        var snapshotFilePathsList = new List<string>();

        foreach (var snapshotFilePath in snapshotFilePaths)
        {
            // TODO: Toto řešit jako jako konfiguraci aplikace a ne jako běžný buffer/snapshot.

            //var snapshotFileName = Path.GetFileName(snapshotFilePath);
            //if (snapshotFileName == Defaults.SnapshotsListFileName)
            //{
            //    continue;
            //}

            snapshotFilePathsList.Add(snapshotFilePath);
        }

        return snapshotFilePathsList.OrderBy(path => path).ToList();
    }


    private string GetSnapshotsDirectoryPath(Settings settings)
    {
        var snapshotsDirectoryPath = settings.SnapshotsDirectoryPath;
        if (string.IsNullOrWhiteSpace(snapshotsDirectoryPath))
        {
            snapshotsDirectoryPath = Defaults.DefaultSnapshotsDirectoryPath;
        }

        if (Directory.Exists(snapshotsDirectoryPath))
        {
            return snapshotsDirectoryPath;
        }

        if (snapshotsDirectoryPath.StartsWith("~/"))
        {
            snapshotsDirectoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                snapshotsDirectoryPath[2..]);
        }

        if (Directory.Exists(snapshotsDirectoryPath) == false)
        {
            Directory.CreateDirectory(snapshotsDirectoryPath);
        }

        return snapshotsDirectoryPath;
    }


    /// <summary>
    /// Loads text from the snapshot.
    /// </summary>
    /// <param name="snapshotFilePath">A path to the snapshot file.</param>
    /// <returns>A text from the snapshot.</returns>
    public string LoadTextFromSnapshot(string snapshotFilePath)
    {
        if (string.IsNullOrEmpty(snapshotFilePath))
        {
            return string.Empty;
        }

        try
        {
            snapshotFilePath = GetSnapshotFilePath(snapshotFilePath);

            // Load text from the snapshot.
            return File.Exists(snapshotFilePath)
                ? File.ReadAllText(snapshotFilePath)
                : string.Empty;
        }
        catch (IOException)
        {
            // We are OK with all IO exceptions here.
            return string.Empty;
        }
    }

    /// <summary>
    /// Saves text to the snapshot.
    /// </summary>
    /// <param name="snapshotFilePath">A path to the snapshot file.</param>
    /// <param name="text">A text to be saved to the snapshot file.</param>
    public void SaveTextToSnapshot(string snapshotFilePath, string text)
    {
        if (string.IsNullOrEmpty(snapshotFilePath))
        {
            return;
        }

        try
        {
            File.WriteAllText(GetSnapshotFilePath(snapshotFilePath), text);
        }
        catch (IOException)
        {
            // We are OK with all IO exceptions here.
        }
    }


    private string GetSnapshotFilePath(string snapshotFilePath)
    {
        if (File.Exists(snapshotFilePath))
        {
            return snapshotFilePath;
        }

        if (snapshotFilePath.StartsWith("~/"))
        {
            snapshotFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                snapshotFilePath[2..]);
        }

        return snapshotFilePath;
    }


    public int GetMainWindowWidth()
    {
        var width = Program.Settings.MainWindowWidth;
        width = width switch
        {
            < 320 => 320,
            > 4096 => 4096,
            _ => width
        };

        return width;
    }


    public int GetMainWindowHeight()
    {
        var height = Program.Settings.MainWindowHeight;
        height = height switch
        {
            < 240 => 240,
            > 4096 => 4096,
            _ => height
        };

        return height;
    }


    public int GetTextEditorFontSize()
    {
        var fontSize = Program.Settings.TextEditorFontSize;
        fontSize = fontSize switch
        {
            < 6 => 6,
            > 72 => 72,
            _ => fontSize
        };

        return fontSize;
    }


    public string GetTextEditorFontFamily()
    {
        var fontFamily = Program.Settings.TextEditorFontFamily;
        if (string.IsNullOrWhiteSpace(fontFamily))
        {
            fontFamily = "Monospace";
        }

        return fontFamily;
    }
}
