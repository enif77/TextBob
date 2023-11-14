//using Avalonia;
using Avalonia.Controls;
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

    #region event handlers
    
    private void MainWindow_OnLoaded(object? sender, RoutedEventArgs e)
    {
        MainTextBox.Focus();

        UpdateInfoText();
    }


    private void AboutButtonClicked(object sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        if (viewModel == null)
        {
            return;
        }

        viewModel.AppViewModel?.ShowAboutWindow();
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
