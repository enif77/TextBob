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


    private void MainTextBoxOn_GotFocus(object sender, GotFocusEventArgs e)
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

    
    private void UpdateInfoText()
    {
        if (MainTextBox.Text == null)
        {
            return;
        }

        var linesCount = 1;
        for (var i = 0; i < MainTextBox.CaretIndex; i++)
        {
            if (MainTextBox.Text[i] == '\n')
            {
                linesCount++;
            }
        }
        
        var charsCount = 1;
        for (var j = MainTextBox.CaretIndex - 1; j >= 0; j--)
        {
            if (MainTextBox.Text[j] == '\n')
            {
                break;
            }
        
            charsCount++;
        }
        
        InfoTextBlock.Text = $"Length: {MainTextBox.Text.Length}, caret index: {MainTextBox.CaretIndex}, line: {linesCount}, column: {charsCount}";
    }
}
