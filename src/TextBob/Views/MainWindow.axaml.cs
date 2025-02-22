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
            =>
        {
            if (DataContext is not MainWindowViewModel viewModel)
            {
                return;
            }
            
            viewModel.MainTextBoxCaretMoved();
        };

        MainTextBox.TextArea.SelectionChanged += (sender, args) =>
        {
            if (DataContext is not MainWindowViewModel viewModel)
            {
                return;
            }
            
            viewModel.SelectionChanged();
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
    
    public static KeyGesture MenuFocusTextEditorGesture => new KeyGesture(Key.F3);
    public static KeyGesture MenuOpenTextBuffersListGesture => new KeyGesture(Key.F2);
    
    public static KeyGesture MenuAboutGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? new KeyGesture(Key.OemQuestion, KeyModifiers.Meta)
        : new KeyGesture(Key.F1);
    
    public static KeyGesture MenuSettingsGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? new KeyGesture(Key.OemComma, KeyModifiers.Meta)
        : new KeyGesture(Key.OemComma, KeyModifiers.Control);
    
    public static KeyGesture MenuQuitGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? new KeyGesture(Key.Q, KeyModifiers.Meta)
        : new KeyGesture(Key.F4, KeyModifiers.Alt);
    
    public static KeyGesture MenuOpenGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? new KeyGesture(Key.O, KeyModifiers.Meta)
        : new KeyGesture(Key.O, KeyModifiers.Control);

    public static KeyGesture MenuSaveGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? new KeyGesture(Key.S, KeyModifiers.Meta)
        : new KeyGesture(Key.S, KeyModifiers.Control);
    
    public static KeyGesture MenuSaveAllModifiedGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? new KeyGesture(Key.S, KeyModifiers.Shift | KeyModifiers.Meta)
        : new KeyGesture(Key.S, KeyModifiers.Shift | KeyModifiers.Control);

    
    #region event handlers

    public void OnCloseClicked(object sender, EventArgs args)
    {
        Close();
    }


    private void MainWindow_OnLoaded(object? sender, RoutedEventArgs e)
    {
        FocusTextEditor();

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
    }
    

    private void CommandButtonClicked(object sender, RoutedEventArgs e)
    {
        FocusTextEditor();
    }

    #endregion


    #region ITextEditorHandler
    
    public int SelectionLength => MainTextBox?.SelectionLength ?? 0;
    public int SelectionStart => MainTextBox?.SelectionStart ?? 0;
    public string SelectedText => MainTextBox?.SelectedText ?? string.Empty;

    public int CaretOffset => MainTextBox?.TextArea.Caret.Offset ?? 0;
    public int CaretLine => MainTextBox?.TextArea.Caret.Line ?? 0;
    public int CaretColumn => MainTextBox?.TextArea.Caret.Column ?? 0;


    public void FocusTextEditor()
    {
        BuffersComboBox.IsDropDownOpen = false;
        MainTextBox.Focus();
    }


    public void FocusTextBuffersList()
    {
        BuffersComboBox.Focus();
        BuffersComboBox.IsDropDownOpen = true;
    }

    #endregion
}
