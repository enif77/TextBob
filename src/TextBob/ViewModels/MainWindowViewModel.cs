using System.Collections.Generic;
using System.Linq;

namespace TextBob.ViewModels;

using System;
using System.IO;
using System.Reactive;
using System.Text.Json;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using AvaloniaEdit;
using AvaloniaEdit.Document;

using ReactiveUI;

using TextBob.Models;


/// <summary>
/// View model for the main window.
/// </summary>
internal class MainWindowViewModel : ReactiveObject
{
    #region properties
    
    private AppViewModel? _appViewModel;

    /// <summary>
    /// The app view model.
    /// </summary>
    public AppViewModel? AppViewModel
    {
        get => _appViewModel;
        set => this.RaiseAndSetIfChanged(ref _appViewModel, value);
    }


    private string? _title;

    /// <summary>
    /// The main window title.
    /// </summary>
    public string? Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }
    
    
    private bool _isUiEnabled = true;

    /// <summary>
    /// Indicates, that the UI is enabled.
    /// </summary>
    public bool IsUiEnabled
    {
        get => _isUiEnabled;
        set => this.RaiseAndSetIfChanged(ref _isUiEnabled, value);
    }

    
    private bool _isTextEditorEnabled = true;

    /// <summary>
    /// Indicates, that the main text editor is enabled.
    /// </summary>
    public bool IsTextEditorEnabled
    {
        get => _isTextEditorEnabled;
        set => this.RaiseAndSetIfChanged(ref _isTextEditorEnabled, value);
    }
    
    
    private bool _isSaveButtonEnabled = true;

    /// <summary>
    /// Indicates, that the save button is enabled.
    /// </summary>
    public bool IsSaveButtonEnabled
    {
        get => _isSaveButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _isSaveButtonEnabled, value);
    }
    
    
    private bool _isDeleteButtonEnabled = true;

    /// <summary>
    /// Indicates, that the delete button is enabled.
    /// </summary>
    public bool IsDeleteButtonEnabled
    {
        get => _isDeleteButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _isDeleteButtonEnabled, value);
    }
    
    /// <summary>
    /// Flag, that indicates if a text is selected.
    /// </summary>
    public bool IsTextSelected
    {
        get
        {
            if (TextEditorHandler == null)
            {
                return false;
            }

            return TextEditorHandler.SelectionLength > 0;
        }
    }


    private IDocument? _document;

    /// <summary>
    /// The main text box text.
    /// </summary>
    public IDocument? Document
    {
        get => _document;
        set => this.RaiseAndSetIfChanged(ref _document, value);
    }


    private TextEditorOptions? _textEditorOptions;
    
    /// <summary>
    /// Text editor options.
    /// </summary>
    public TextEditorOptions? TextEditorOptions
    {
        get => _textEditorOptions;
        set => this.RaiseAndSetIfChanged(ref _textEditorOptions, value);
    }
    
    
    private int _fontSize;

    /// <summary>
    /// Gets or sets the size of the font in the text editor.
    /// </summary>
    public int FontSize
    {
        get => _fontSize;
        set => this.RaiseAndSetIfChanged(ref _fontSize, value);
    }


    private string _fontFamily = string.Empty;

    /// <summary>
    /// Gets or sets the font family of the font in the text editor.
    /// </summary>
    public string FontFamily
    {
        get => _fontFamily;
        set => this.RaiseAndSetIfChanged(ref _fontFamily, value);
    }
    
    
    private bool _showLineNumbers;

    /// <summary>
    /// Shows or hides line numbers in the left column.
    /// </summary>
    public bool ShowLineNumbers
    {
        get => _showLineNumbers;
        set => this.RaiseAndSetIfChanged(ref _showLineNumbers, value);
    }
    
    
    private bool _textChanged;

    /// <summary>
    /// Indicates, that the text has unsaved changes.
    /// </summary>
    public bool TextChanged
    {
        get => _textChanged;
        set => this.RaiseAndSetIfChanged(ref _textChanged, value);
    }
    
    
    /// <summary>
    /// Handler for the text editor.
    /// </summary>
    public ITextEditorHandler? TextEditorHandler { get; set; }
    
    
    private string _textInfo = string.Empty;

    /// <summary>
    /// Shows info about actual text and cursor state.
    /// </summary>
    public string TextInfo
    {
        get => _textInfo;
        set => this.RaiseAndSetIfChanged(ref _textInfo, value);
    }


    private TextBuffer? _currentTextBuffer = new ();
    
    /// <summary>
    /// The current text buffer.
    /// </summary>
    public TextBuffer? CurrentTextBuffer
    {
        get => _currentTextBuffer;

        set
        {
            if (_currentTextBuffer == value)
            {
                return;
            }

            _currentTextBuffer = value;
            LoadCurrentBuffer();
            
            this.RaisePropertyChanged();
        }
    }
    
    
    private TextBuffer? _selectedTextBuffer = new ();

    /// <summary>
    /// The selected text buffer.
    /// </summary>
    public TextBuffer? SelectedTextBuffer
    {
        get => _selectedTextBuffer;

        set
        {
            if (_selectedTextBuffer == value)
            {
                return;
            }

            _selectedTextBuffer = value;
            LoadSelectedBuffer();

            this.RaisePropertyChanged();
        }
    }
    
    #endregion
    
    
    #region commands
    
    public ReactiveCommand<Unit, Unit> OpenCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }
    public ReactiveCommand<Unit, Unit> SettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> AboutCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }
    
    #endregion
    
    
    #region ctor
    
    public MainWindowViewModel()
    {
        FontSize = Defaults.DefaultFontSize;
        FontFamily = Defaults.DefaultFontFamily;
        TextInfo = Defaults.AppVersionInfo;
        
        AboutCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (AppViewModel == null)
            {
                return;
            }

            var text = Defaults.AppName;

            // https://docs.avaloniaui.net/docs/basics/user-interface/assets
            using (var reader = new StreamReader(AssetLoader.Open(new Uri("avares://TextBob/Assets/Windows/about.txt"))))
            {
                text = await reader.ReadToEndAsync();
            }

            text = text.Replace("${version-info}", Defaults.AppVersionInfo);

            CurrentTextBuffer = new TextBuffer
            {
                Name = "About",
                Text = text,
                IsReadOnly = true
            };
            
            //  TODO: Disable editing as a method.
            
            // Disable editing.
            IsTextEditorEnabled = false;
            IsSaveButtonEnabled = false;
            IsDeleteButtonEnabled = false;
        });
        
        SettingsCommand = ReactiveCommand.Create(() =>
        {
            if (AppViewModel == null)
            {
                return;
            }

            // TODO: Change this to a text buffer. (Use CurrentTextBuffer like the About command)
            
            var settingsFilePath = Program.GetSettingsFilePath();
            Document = new TextDocument(File.Exists(settingsFilePath)
                ? AppViewModel?.LoadTextSnapshot(settingsFilePath)
                : Program.Settings.ToJson());
            TextChanged = false;
            _settingsLoaded = true;
            
            // TODO: Enable editing as a method.
            
            // Enable editing.
            IsTextEditorEnabled = true;
            IsSaveButtonEnabled = true;
            IsDeleteButtonEnabled = true;
        });
        
        OpenCommand = ReactiveCommand.Create(LoadSelectedBuffer);
        SaveCommand = ReactiveCommand.Create(SaveCurrentBuffer);
        
        ClearCommand = ReactiveCommand.Create(() =>
        {
            if (Document == null)
            {
                return;
            }
            
            var document = Document!;
            
            if (IsTextSelected)
            {
                document.Remove(TextEditorHandler!.SelectionStart, TextEditorHandler.SelectionLength);
            }
            else
            {
                document.Remove(0, document.TextLength);
            }
            
            //document.Replace(0, Document.TextLength, string.Empty);
        });
        
        ExitCommand = ReactiveCommand.Create(() =>
        {
            (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Shutdown();
        });
    }
    
    #endregion
    
    
    #region public
    
    public void SelectionChanged()
    {
        this.RaisePropertyChanged(nameof(IsTextSelected));
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
    
    #endregion
    
    
    #region private

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
    
    /// <summary>
    /// Flag, that indicates if the settings are loaded.
    /// </summary>
    private bool _settingsLoaded;
    
    
    private void LoadCurrentBuffer()
    {
        Document = new TextDocument((CurrentTextBuffer == null)
            ? string.Empty
            : CurrentTextBuffer.Text);
        
        // Reset the text changed flag.
        TextChanged = false;
        
        // Clear the settings loaded flag.
        _settingsLoaded = false;
    }
    
    
    private void LoadSelectedBuffer()
    {
        if (SelectedTextBuffer == null)
        {
            IsSaveButtonEnabled = false;
            IsDeleteButtonEnabled = false;
            IsTextEditorEnabled = true;
            
            return;
        }
        
        // Update the selected buffer with the contents of the snapshot.
        SelectedTextBuffer.Text = AppViewModel?.LoadTextSnapshot(SelectedTextBuffer.Path) ?? string.Empty;
        
        // Update the current buffer.
        CurrentTextBuffer = null;
        CurrentTextBuffer = SelectedTextBuffer;
        
        // Disable editing if the buffer is read only.
        IsTextEditorEnabled = SelectedTextBuffer.IsReadOnly == false;
        IsDeleteButtonEnabled = IsTextEditorEnabled;
        IsSaveButtonEnabled = IsTextEditorEnabled;
    }
    
    
    private void SaveCurrentBuffer()
    {
        if (Document == null)
        {
            return;
        }
        
        if (_settingsLoaded)
        {
            SaveSettings();
        }
        else
        {
            var currentTextBuffer = CurrentTextBuffer;
            if (currentTextBuffer != null && currentTextBuffer.IsReadOnly == false)
            {
                // Update the text buffer.
                currentTextBuffer.Text = Document.Text;
                
                // Save the text buffer to a snapshot.
                AppViewModel?.SaveTextSnapshot(
                    currentTextBuffer.Path,
                    currentTextBuffer.Text);
            }
        }
        
        TextChanged = false;
    }


    private void SaveSettings()
    {
        var settingsJson = Document!.Text;
        try
        {
            // Validate settings.
            _ = JsonSerializer.Deserialize<Settings>(settingsJson);
            
            // Save settings.
            File.WriteAllText(Program.GetSettingsFilePath(), settingsJson);

            // TODO: Reload settings.
        }
        catch (Exception)
        {
            // TODO: Log the exception.
        }
    }
    
    #endregion
}
