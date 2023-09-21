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


    private void ClearButtonClicked(object sender, RoutedEventArgs e)
    {
        mainTextBox.Clear();
        mainTextBox.Focus();

        UpdateInfoText();
    }


    private void MainTextBoxGotFocus(object sender, GotFocusEventArgs e)
    {
        UpdateInfoText();
    }


    private void MainTextBoxKeyUp(object sender, KeyEventArgs e)
    {
        UpdateInfoText();
    }


    private void MainTextBoxPointerMoved(object sender, PointerEventArgs e)
    {
        UpdateInfoText();
    }


    private void UpdateInfoText()
    {
        if (mainTextBox.Text == null)
        {
            return;
        }

        // https://stackoverflow.com/questions/15577464/how-to-count-of-sub-string-occurrences-within-a-string-not-char-occurrences
        var pos = 0;
        var linesCount = 0;
        while ((pos < mainTextBox.CaretIndex) && (pos = mainTextBox.Text.IndexOf(mainTextBox.NewLine, pos)) != -1)
        {
            linesCount++;
            pos += mainTextBox.NewLine.Length;
        }

        infoTextBlock.Text = $"Length: {mainTextBox.Text.Length}, position: {mainTextBox.CaretIndex}, line: {linesCount}";
    }
}
