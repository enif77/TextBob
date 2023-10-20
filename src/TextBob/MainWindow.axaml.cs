using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace TextBob;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    #region event handlers
    
    private void MainWindow_OnLoaded(object? sender, RoutedEventArgs e)
    {
        MainTextBox.Focus();

        UpdateInfoText();
    }
    

    private void ClearButtonClicked(object sender, RoutedEventArgs e)
    {
        MainTextBox.Clear();
        MainTextBox.Focus();

        UpdateInfoText();
    }


    private void MainTextBox_OnGotFocus(object sender, GotFocusEventArgs e)
    {
        UpdateInfoText();
    }


    private void MainTextBox_OnKeyUp(object sender, KeyEventArgs e)
    {
        UpdateInfoText();
    }

    
    private void MainTextBox_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        UpdateInfoText();
    }
    
    #endregion


    #region settings

    public void SetShowLineNumbers(bool showLineNumbers)
    {
        MainTextBox.ShowLineNumbers = showLineNumbers;
    }

    #endregion
    
    
    #region private
    
    private void UpdateInfoText()
    {
        // TODO: CaretOffset se aktualizuje jen při psaní textu. Klik myši ho neaktualizuje.

        var caret = MainTextBox.TextArea.Caret;
        InfoTextBlock.Text = $"Length {MainTextBox.Document.TextLength}, Lines {MainTextBox.Document.LineCount} | Line {caret.Line}, Column {caret.Column}";
    }
    
    #endregion
}
