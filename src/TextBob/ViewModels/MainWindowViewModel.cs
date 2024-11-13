using System;
using System.IO;
using System.Text.Json;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using AvaloniaEdit;
using AvaloniaEdit.Document;

using MiniMvvm;
using TextBob.Models;


namespace TextBob.ViewModels;

/// <summary>
/// View model for the main window.
/// </summary>
internal class MainWindowViewModel : ViewModelBase
{
    #region properties
    
    private AppViewModel? _appViewModel;

    /// <summary>
    /// The app view model.
    /// </summary>
    public AppViewModel? AppViewModel
    {
        get => _appViewModel;

        set
        {
            if (_appViewModel == value)
            {
                return;
            }

            _appViewModel = value;
            
            RaisePropertyChanged();
        }
    }


    private string? _title;

    /// <summary>
    /// The main window title.
    /// </summary>
    public string? Title
    {
        get => _title;

        set
        {
            if (_title == value)
            {
                return;
            }

            _title = value;
            
            RaisePropertyChanged();
        }
    }
    
    
    private bool _isUiEnabled = true;

    /// <summary>
    /// Indicates, that the UI is enabled.
    /// </summary>
    public bool IsUiEnabled
    {
        get => _isUiEnabled;

        set
        {
            if (_isUiEnabled == value)
            {
                return;
            }

            _isUiEnabled = value;
            
            RaisePropertyChanged();
        }
    }

    
    private bool _isTextEditorEnabled = true;

    /// <summary>
    /// Indicates, that the main text editor is enabled.
    /// </summary>
    public bool IsTextEditorEnabled
    {
        get => _isTextEditorEnabled;

        set
        {
            if (_isTextEditorEnabled == value)
            {
                return;
            }

            _isTextEditorEnabled = value;
            
            RaisePropertyChanged();
        }
    }
    
    
    private bool _isSaveButtonEnabled = true;

    /// <summary>
    /// Indicates, that the save button is enabled.
    /// </summary>
    public bool IsSaveButtonEnabled
    {
        get => _isSaveButtonEnabled;

        set
        {
            if (_isSaveButtonEnabled == value)
            {
                return;
            }

            _isSaveButtonEnabled = value;
            
            RaisePropertyChanged();
        }
    }
    
    
    private bool _isDeleteButtonEnabled = true;

    /// <summary>
    /// Indicates, that the delete button is enabled.
    /// </summary>
    public bool IsDeleteButtonEnabled
    {
        get => _isDeleteButtonEnabled;

        set
        {
            if (_isDeleteButtonEnabled == value)
            {
                return;
            }

            _isDeleteButtonEnabled = value;
            
            RaisePropertyChanged();
        }
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

        set
        {
            if (_document == value)
            {
                return;
            }

            _document = value;
            
            RaisePropertyChanged();
        }
    }


    private TextEditorOptions? _textEditorOptions;
    
    /// <summary>
    /// Text editor options.
    /// </summary>
    public TextEditorOptions? TextEditorOptions
    {
        get => _textEditorOptions;

        set
        {
            if (_textEditorOptions == value)
            {
                return;
            }

            _textEditorOptions = value;

            RaisePropertyChanged();
        }
    }
    
    
    private int _fontSize;

    /// <summary>
    /// Gets or sets the size of the font in the text editor.
    /// </summary>
    public int FontSize
    {
        get => _fontSize;

        set
        {
            if (_fontSize == value)
            {
                return;
            }

            _fontSize = value;

            RaisePropertyChanged();
        }
    }


    private string _fontFamily = string.Empty;

    /// <summary>
    /// Gets or sets the font family of the font in the text editor.
    /// </summary>
    public string FontFamily
    {
        get => _fontFamily;

        set
        {
            if (_fontFamily == value)
            {
                return;
            }

            _fontFamily = value;

            RaisePropertyChanged();
        }
    }
    
    
    private bool _showLineNumbers;

    /// <summary>
    /// Shows or hides line numbers in the left column.
    /// </summary>
    public bool ShowLineNumbers
    {
        get => _showLineNumbers;

        set
        {
            if (_showLineNumbers == value)
            {
                return;
            }

            _showLineNumbers = value;

            RaisePropertyChanged();
        }
    }
    
    
    private bool _textChanged;

    /// <summary>
    /// Indicates, that the text has unsaved changes.
    /// </summary>
    public bool TextChanged
    {
        get => _textChanged;

        set
        {
            if (_textChanged == value)
            {
                return;
            }

            _textChanged = value;

            RaisePropertyChanged();
        }
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

        set
        {
            if (_textInfo == value)
            {
                return;
            }

            _textInfo = value;

            RaisePropertyChanged();
        }
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
            
            RaisePropertyChanged();
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

            RaisePropertyChanged();
        }
    }
    
    #endregion
    
    
    #region commands
    
    public MiniCommand OpenCommand { get; }
    public MiniCommand SaveCommand { get; }
    public MiniCommand ClearCommand { get; }
    public MiniCommand SettingsCommand { get; }
    public MiniCommand AboutCommand { get; }
    public MiniCommand ExitCommand { get; }
    
    #endregion
    
    
    #region ctor
    
    public MainWindowViewModel()
    {
        FontSize = Defaults.DefaultFontSize;
        FontFamily = Defaults.DefaultFontFamily;
        TextInfo = Defaults.AppVersionInfo;
        
        AboutCommand = MiniCommand.CreateFromTask(async () =>
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
            
            // Disable editing.
            IsTextEditorEnabled = false;
            IsSaveButtonEnabled = false;
            IsDeleteButtonEnabled = false;
        });
        
        SettingsCommand = MiniCommand.Create(() =>
        {
            if (AppViewModel == null)
            {
                return;
            }

            // TODO: Change this to a text buffer.
            
            var settingsFilePath = Program.GetSettingsFilePath();
            Document = new TextDocument(File.Exists(settingsFilePath)
                ? AppViewModel?.LoadTextSnapshot(settingsFilePath)
                : Program.Settings.ToJson());
            TextChanged = false;
            _settingsLoaded = true;
            
            // Enable editing.
            IsTextEditorEnabled = true;
            IsSaveButtonEnabled = true;
            IsDeleteButtonEnabled = false;
        });
        
        OpenCommand = MiniCommand.Create(LoadSelectedBuffer);
        SaveCommand = MiniCommand.Create(SaveCurrentBuffer);
        
        ClearCommand = MiniCommand.Create(() =>
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
        
        ExitCommand = MiniCommand.Create(() =>
        {
            (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Shutdown();
        });
    }
    
    #endregion
    
    
    #region public
    
    public void SelectionChanged()
    {
        RaisePropertyChanged(nameof(IsTextSelected));
    }
    
    #endregion
    
    
    #region private

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
        
        //TextChanged = false;
        //_settingsLoaded = false;
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

            return;
        }
    }
    
    #endregion
}
