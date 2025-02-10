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

        MainTextBox.TextArea.Caret.PositionChanged += (sender, args)
            => UpdateInfoText();

        MainTextBox.TextArea.SelectionChanged += (sender, args) =>
        {
            if (DataContext is not MainWindowViewModel viewModel)
            {
                return;
            }
            
            viewModel.SelectionChanged();
            UpdateInfoText();
        };

        // All menu keyboard shortcuts are bound to the main window.
        
        //HotKeyManager.SetHotKey(OpenButton, MenuOpenGesture);
    }


    public static string MenuQuitHeader => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? "Quit Text Bob"
        : "E_xit";
    
    public static string MenuOpenHeader => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? "Open/reload selected buffer"
        : "O_pen/reload selected buffer";

    public static string MenuSaveHeader => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? "Save the current buffer"
        : "S_ave the current buffer";
    
    public static string MenuSaveAllModifiedHeader => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? "Save all modified buffers"
        : "S_ave all modified buffers";
    
    public static KeyGesture MenuQuitGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
        new KeyGesture(Key.Q, KeyModifiers.Meta) :
        new KeyGesture(Key.F4, KeyModifiers.Alt);
    
    public static KeyGesture MenuOpenGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
        new KeyGesture(Key.O, KeyModifiers.Meta) :
        new KeyGesture(Key.O, KeyModifiers.Control);

    public static KeyGesture MenuSaveGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
        new KeyGesture(Key.S, KeyModifiers.Meta) :
        new KeyGesture(Key.S, KeyModifiers.Control);
    
    public static KeyGesture MenuSaveAllModifiedGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
        new KeyGesture(Key.S, KeyModifiers.Shift | KeyModifiers.Meta) :
        new KeyGesture(Key.S, KeyModifiers.Shift | KeyModifiers.Control);

    
    #region event handlers

    public void OnCloseClicked(object sender, EventArgs args)
    {
        Close();
    }


    private void MainWindow_OnLoaded(object? sender, RoutedEventArgs e)
    {
        MainTextBox.Focus();

        // TODO: Figure out, how to bind to options.

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
        MainTextBox.TextArea.TextView.LinkTextForegroundBrush = Brushes.LightBlue;

        // Load the first text buffer, if any.
        if (viewModel.TextBuffers != null && viewModel.TextBuffers.Count > 0)
        {
            BuffersComboBox.SelectedIndex = 0;
        }
        
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
}
