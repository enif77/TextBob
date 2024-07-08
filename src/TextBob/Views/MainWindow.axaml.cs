//using Avalonia;

using System;
using System.Runtime.InteropServices;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

using TextBob.ViewModels;


namespace TextBob.Views;

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
        };

        HotKeyManager.SetHotKey(SaveButton, MenuSaveGesture);
    }
    
    public static string MenuQuitHeader => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? "Quit Text Bob"
        : "E_xit";
    
    public static string MenuSaveHeader => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? "Save text snapshot"
        : "S_ave text snapshot";
    
    public static KeyGesture MenuQuitGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
        new KeyGesture(Key.Q, KeyModifiers.Meta) :
        new KeyGesture(Key.F4, KeyModifiers.Alt);
    
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

        UpdateInfoText();
    }


    private void CommandButtonClicked(object sender, RoutedEventArgs e)
    {
        MainTextBox.Focus();
    }
    
    #endregion


    #region settings

    // public static readonly DirectProperty<MainWindow, bool> ShowLineNumbersProperty =
    //     AvaloniaProperty.RegisterDirect<MainWindow, bool>(
    //         nameof(ShowLineNumbers),
    //         o => o.ShowLineNumbers,
    //         (o, v) => o.ShowLineNumbers = v);

    // private bool _showLineNumbers = true;

    // public bool ShowLineNumbers
    // {
    //     get { return _showLineNumbers; }
    //     set { SetAndRaise(ShowLineNumbersProperty, ref _showLineNumbers, value); }
    // }

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
        
        viewModel.TextInfo =
            $"Length {textLength}, Lines {document.LineCount} | Line {caret.Line}, Column {caret.Column}, Offset {caret.Offset}, Char '{charAt.Item2}', UTF {charAt.Item1}";
    }
    
    #endregion
}
