using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TextBob.Models;

namespace TextBob.Views;

using System;
using System.Runtime.InteropServices;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

using TextBob.ViewModels;


public partial class MainWindow : Window, ITextEditorHandler
{
    public MainWindow()
    {
        InitializeComponent();
        
        MainTextBox.TextArea.Caret.PositionChanged += (sender, args) => UpdateInfoText();
        MainTextBox.TextChanged += (sender, args) =>
        {
            if (DataContext is not MainWindowViewModel viewModel)
            {
                return;
            }
            
            viewModel.TextChanged = MainTextBox.IsModified;
        };
        MainTextBox.TextArea.SelectionChanged += (sender, args) =>
        {
            if (DataContext is not MainWindowViewModel viewModel)
            {
                return;
            }
            
            viewModel.SelectionChanged();
            UpdateInfoText();
        };

        HotKeyManager.SetHotKey(OpenButton, MenuOpenGesture);
        HotKeyManager.SetHotKey(SaveButton, MenuSaveGesture);
    }
    
    public static string MenuQuitHeader => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? "Quit Text Bob"
        : "E_xit";
    
    public static string MenuOpenHeader => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? "Open/reload selected buffer"
        : "O_pen/reload selected buffer";
    
    public static string MenuSaveHeader => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? "Save text snapshot"
        : "S_ave text snapshot";
    
    public static KeyGesture MenuQuitGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
        new KeyGesture(Key.Q, KeyModifiers.Meta) :
        new KeyGesture(Key.F4, KeyModifiers.Alt);
    
    public static KeyGesture MenuOpenGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
        new KeyGesture(Key.O, KeyModifiers.Meta) :
        new KeyGesture(Key.O, KeyModifiers.Alt);
    
    public static KeyGesture MenuSaveGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
        new KeyGesture(Key.S, KeyModifiers.Meta) :
        new KeyGesture(Key.S, KeyModifiers.Control);
    

    #region event handlers
    
    public void OnCloseClicked(object sender, EventArgs args)
    {
        Close();
    }
    
    private void MainWindow_OnLoaded(object? sender, RoutedEventArgs e)
    {
        MainTextBox.Focus();
        
        var viewModel = DataContext as MainWindowViewModel;
        if (viewModel == null)
        {
            return;
        }
        
        // Binding to these properties does not work.
        // Something is not yet initialized inside the AvaloniaEdit during the XAML loading.
        MainTextBox.Options = viewModel.TextEditorOptions;
        MainTextBox.FontFamily = viewModel.FontFamily;
        
        // Set hyperlink color.
        MainTextBox.TextArea.TextView.LinkTextForegroundBrush = Brushes.Gray;

        UpdateBuffersList();
        UpdateInfoText();
    }


    private void CommandButtonClicked(object sender, RoutedEventArgs e)
    {
        MainTextBox.Focus();
    }
    
    #endregion
    
    
    #region ITextEditorHandler

    /// <inheritdoc />
    public int SelectionLength => MainTextBox?.SelectionLength ?? 0;

    /// <inheritdoc />
    public int SelectionStart => MainTextBox?.SelectionStart ?? 0;

    /// <inheritdoc />
    public string SelectedText => MainTextBox?.SelectedText ?? string.Empty;

    #endregion
    
    
    #region private

    private void UpdateBuffersList()
    {
        BuffersComboBox.Items.Clear();

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

        foreach (var snapshot in snapshotsList)
        {
            BuffersComboBox.Items.Add(new TextBuffer
            {
                Name = snapshot.Name,
                Path = snapshot.Path,
                IsReadOnly = snapshot.ReadOnly
            });
        }
        
        BuffersComboBox.SelectedIndex = 0;
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
    
    
    private void UpdateInfoText()
    {
        var viewModel = DataContext as MainWindowViewModel;
        if (viewModel == null)
        {
            return;
        }
        
        var document = MainTextBox.Document;
        var caret = MainTextBox.TextArea.Caret;
        var textLength = document.TextLength;
        
        var charAt = (-1, "EOF");
        if (caret.Offset < textLength)
        {
            var c = document.GetCharAt(caret.Offset);
            switch (c)
            {
                case ' ':
                    charAt = (c, "SPC");
                    break;
                
                case '\t':
                    charAt = (c, "TAB");
                    break;
                
                case '\n':
                    charAt = (c, "LF");
                    break;
                
                case '\r':
                    charAt = (c, "CR");
                    break;
                
                default:
                    charAt = (c, c.ToString());
                    break;
            }
        }
        
        var selection = "Nothing selected";
        if (viewModel.IsTextSelected)
        {
            selection = $"{SelectionLength} chars from {SelectionStart}";
        }
        
        viewModel.TextInfo =
            $"{textLength} chars, {document.LineCount} lines | {selection} | Line {caret.Line}, Column {caret.Column}, Offset {caret.Offset}, Char '{charAt.Item2}', UTF {charAt.Item1}";
    }
    
    #endregion
}
