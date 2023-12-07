//using Avalonia;

using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

using TextBob.ViewModels;


namespace TextBob.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        MainTextBox.TextArea.Caret.PositionChanged += (sender, args) => UpdateInfoText();
    }
    
    public static string MenuQuitHeader => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? "Quit Text Bob"
        : "E_xit";
    
    public static KeyGesture MenuQuitGesture => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
        new KeyGesture(Key.Q, KeyModifiers.Meta) :
        new KeyGesture(Key.F4, KeyModifiers.Alt);
    

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
        
        var options = MainTextBox.Options;

        options.ConvertTabsToSpaces = viewModel.ConvertTabsToSpaces;
        options.EnableEmailHyperlinks = viewModel.EnableEmailHyperlinks;
        options.EnableHyperlinks = viewModel.EnableHyperlinks;
        options.HighlightCurrentLine = viewModel.HighlightCurrentLine;
        options.IndentationSize = viewModel.IndentationSize;
        
        UpdateInfoText();
    }


    private void AboutButtonClicked(object sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;

        viewModel?.AppViewModel?.ShowAboutWindow();
        
        MainTextBox.Focus();
    }
    

    private void SaveButtonClicked(object sender, RoutedEventArgs e)
    {
        MainTextBox.Focus();
    }
    
    
    private void ClearButtonClicked(object sender, RoutedEventArgs e)
    {
        // This clears all text and clears undo history.
        //MainTextBox.Clear();
        
        // This keeps undo history.
        MainTextBox.SelectAll();
        MainTextBox.Delete();
        
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
    
    
    #region private
    
    private void UpdateInfoText()
    {
        var document = MainTextBox.Document;
        var caret = MainTextBox.TextArea.Caret;
        InfoTextBlock.Text = $"Length {document.TextLength}, Lines {document.LineCount} | Line {caret.Line}, Column {caret.Column}";
    }
    
    #endregion
}
