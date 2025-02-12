namespace TextBob.ViewModels;

using System.Collections.ObjectModel;
using System.Reactive;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

using AvaloniaEdit;
using AvaloniaEdit.Document;

using ReactiveUI;

using TextBob.Services;


internal class MainWindowViewModel : ReactiveObject
{
    private readonly IAppService _appService;


    #region properties

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
    /// Indicates, that the SaveButton is enabled.
    /// </summary>
    public bool IsSaveButtonEnabled
    {
        get => _isSaveButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _isSaveButtonEnabled, value);
    }


    private bool _isDeleteButtonEnabled = true;

    /// <summary>
    /// Indicates, that the DeleteButton is enabled.
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
        set
        {
            if (CurrentTextBuffer != null && CurrentTextBuffer != _emptyTextBuffer)
            {
                CurrentTextBuffer.IsModified = value;
            }
            
            if (value != _textChanged)
            {
                UpdateWindowTitle();
            }
            
            this.RaiseAndSetIfChanged(ref _textChanged, value);
        }
    }


    /// <summary>
    /// Handler for the text editor.
    /// </summary>
    public ITextEditorHandler? TextEditorHandler { get; set; }


    private string _textInfo = string.Empty;

    /// <summary>
    /// Shows information related to the actual cursor position and text.
    /// </summary>
    public string TextInfo
    {
        get => _textInfo;
        set => this.RaiseAndSetIfChanged(ref _textInfo, value);
    }


    private TextBuffer? _currentTextBuffer = new ();

    /// <summary>
    /// The currently edited text buffer.
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

            var previousTextBuffer = _currentTextBuffer;
            _currentTextBuffer = value;
            LoadCurrentTextBuffer(previousTextBuffer);

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
            LoadSelectedTextBuffer();

            this.RaisePropertyChanged();
        }
    }


    private ObservableCollection<TextBuffer>? _textBuffers = new();

    /// <summary>
    /// The list of text buffers.
    /// </summary>
    public ObservableCollection<TextBuffer>? TextBuffers
    {
        get => _textBuffers;
        set => this.RaiseAndSetIfChanged(ref _textBuffers, value);
    }
    
    #endregion


    #region commands

    public ReactiveCommand<Unit, Unit> OpenTextBuffersListCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveAllModifiedCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }
    public ReactiveCommand<Unit, Unit> SettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> AboutCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }

    #endregion


    #region ctor

    public MainWindowViewModel(IAppService appService)
    {
        _appService = appService;

        FontSize = Defaults.DefaultFontSize;
        FontFamily = Defaults.DefaultFontFamily;
        TextInfo = Defaults.AppVersionInfo;

        AboutCommand = ReactiveCommand.Create(() =>
        {
            // TODO: Create special About text buffer.

            CurrentTextBuffer = new TextBuffer
            {
                Name = "About",
                Text = _appService.GetAboutText(),
                IsReadOnly = true,
                TextLoaded = true
            };

            DisableEditing();
        });

        SettingsCommand = ReactiveCommand.Create(() =>
        {
            // TODO: Create special Settings text buffer.

            CurrentTextBuffer = new TextBuffer
            {
                Name = "Settings",
                Text = _appService.GetSettingsJson(),
                TextLoaded = true
            };

            EnableEditing();
        });

        OpenTextBuffersListCommand = ReactiveCommand.Create(() =>
        {
            // TODO: Implement OpenTextBuffersListCommand.
        });

        OpenCommand = ReactiveCommand.Create(() =>
        {
            // If Settings or About is loaded, do not reload the selected buffer from the snapshots store.
            LoadSelectedTextBuffer(CurrentTextBuffer != null && CurrentTextBuffer.Name != "Settings" && CurrentTextBuffer.Name != "About");
        });

        SaveCommand = ReactiveCommand.Create(SaveCurrentTextBuffer);

        SaveAllModifiedCommand = ReactiveCommand.Create(SaveAllModifiedTextBuffers);
        
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
                
                //Document?.Replace(0, Document.TextLength, string.Empty);
            },

            // https://docs.avaloniaui.net/docs/concepts/reactiveui/reactive-command

            this.WhenAnyValue(
                x => x.IsDeleteButtonEnabled,
                x => x == true));
            
            ExitCommand = ReactiveCommand.Create(() =>
            {
                (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Shutdown();
            });
    }
    
    #endregion


    #region public
    
    public void SelectionChanged()
    {
        UpdateInfoText();
        
        this.RaisePropertyChanged(nameof(IsTextSelected));
    }
    
    
    public void MainTextBoxCaretMoved()
    {
        UpdateInfoText();
    }


    public void UpdateTextBuffersList()
    {
        TextBuffers ??= [];

        TextBuffers.Clear();

        foreach (var snapshot in _appService.GetSnapshotsList())
        {
            TextBuffers.Add(new TextBuffer
            {
                Name = snapshot.Name,
                Path = snapshot.Path,
                IsReadOnly = snapshot.ReadOnly
            });
        }
    }
    
    #endregion


    #region private

    private readonly TextBuffer _emptyTextBuffer = new()
    {
        Name = "Empty",
        Text = string.Empty,
        IsReadOnly = true,
        TextLoaded = true
    };


    private void EnableEditing()
    {
        IsTextEditorEnabled = true;
        IsSaveButtonEnabled = true;
        IsDeleteButtonEnabled = true;
    }


    private void DisableEditing()
    {
        IsTextEditorEnabled = false;
        IsSaveButtonEnabled = false;
        IsDeleteButtonEnabled = false;
    }


    private void UpdateWindowTitle()
    {
        if (CurrentTextBuffer == null)
        {
            return;
        }
        
        Title = CurrentTextBuffer.IsModified
            ? $"{Defaults.AppName} - {CurrentTextBuffer.Name} *"
            : $"{Defaults.AppName} - {CurrentTextBuffer.Name}";
    }
    
    
    private void UpdateInfoText()
    {
        var textEditorHandler = TextEditorHandler;
        if (textEditorHandler == null)
        {
            return;
        }
        
        var document = Document;
        if (document == null)
        {
            return;
        }
        
        var textLength = document.TextLength;
        
        var charAt = (-1, "EOF");
        if (textEditorHandler.CaretOffset < textLength)
        {
            var c = document.GetCharAt(textEditorHandler.CaretOffset);
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
        if (IsTextSelected)
        {
            selection = $"{textEditorHandler.SelectionLength} chars from {textEditorHandler.SelectionStart}";
        }
        
        TextInfo =
            $"{textLength} chars, {document.LineCount} lines | {selection} | Line {textEditorHandler.CaretLine}, Column {textEditorHandler.CaretColumn}, Offset {textEditorHandler.CaretOffset}, Char '{charAt.Item2}', UTF {charAt.Item1}";
    }

    /// <summary>
    /// Loads the current buffer to UI.
    /// </summary>
    private void LoadCurrentTextBuffer(TextBuffer? previousTextBuffer)
    {
        // Get the previous TextBuffer and store its state.
        if (previousTextBuffer != null && previousTextBuffer != _emptyTextBuffer)
        {
            // TODO: Store the Document in the TextBuffer to keep its state.

            previousTextBuffer.Text = Document?.Text ?? string.Empty;
            previousTextBuffer.IsModified = TextChanged;
        }
        
        if (CurrentTextBuffer == null)
        {
            _currentTextBuffer = _emptyTextBuffer;
        }

        // Restore the current buffer state.
        Document = new TextDocument(CurrentTextBuffer!.Text);

        // We are updating the UI state from the TB state, we do not want to update the TB state.
        // See the TextChanged property setter.
        _textChanged = CurrentTextBuffer.IsModified;
        this.RaisePropertyChanged(nameof(TextChanged));
        
        // Update UI.
        UpdateWindowTitle();
        UpdateInfoText();
        
        // Disable editing if the buffer is read-only.
        IsSaveButtonEnabled = CurrentTextBuffer.IsReadOnly == false;
        IsDeleteButtonEnabled = IsSaveButtonEnabled;
        IsTextEditorEnabled = IsSaveButtonEnabled;
    }

    /// <summary>
    /// Loads the selected buffer.
    /// </summary>
    /// <param name="forceReload">Used by the OpenCommand - forces reloading the selected snapshot from the snapshots store.</param>
    private void LoadSelectedTextBuffer(bool forceReload = false)
    {
        // If nothing is selected, use the read only empty buffer.
        if (SelectedTextBuffer == null)
        {
            CurrentTextBuffer = _emptyTextBuffer;

            return;
        }

        // Invalidate the current buffer.
        CurrentTextBuffer = null;

        // Do not load the buffer, if it is already loaded.
        if (SelectedTextBuffer.TextLoaded == false || forceReload)
        {
            // Update the selected buffer with the contents of the snapshot file.
            SelectedTextBuffer.Text = _appService.LoadTextFromSnapshot(SelectedTextBuffer.Path);
            SelectedTextBuffer.TextLoaded = true;

            // Any changes in the buffer were lost by loading contents from the snapshot.
            SelectedTextBuffer.IsModified = false;
        }

        // Update the current buffer.
        CurrentTextBuffer = SelectedTextBuffer;
    }

    /// <summary>
    /// Saves the selected buffer.
    /// </summary>
    private void SaveCurrentTextBuffer()
    {
        if (Document == null)
        {
            return;
        }

        var currentTextBuffer = CurrentTextBuffer;
        if (currentTextBuffer == null || currentTextBuffer.IsReadOnly)
        {
            return;
        }

        currentTextBuffer.Text = Document.Text;

        if (currentTextBuffer.Name == "Settings")
        {
            if (_appService.SaveSettings(currentTextBuffer.Text))
            {
                // TODO: Reload settings.
            }
        }
        else
        {
            _appService.SaveTextToSnapshot(
                currentTextBuffer.Path,
                currentTextBuffer.Text);
        }
        
        TextChanged = false;
    }
    
    
    private void SaveAllModifiedTextBuffers()
    {
        if (TextBuffers == null)
        {
            return;
        }
        
        var wasModified = false;
        foreach (var textBuffer in TextBuffers)
        {
            if (textBuffer.IsModified == false)
            {
                continue;
            }

            _appService.SaveTextToSnapshot(
                textBuffer.Path,
                textBuffer.Text);
            
            textBuffer.IsModified = false;
            
            wasModified = true;
        }
        
        if (wasModified)
        {
            TextChanged = false;
        }
    }

    #endregion
}
