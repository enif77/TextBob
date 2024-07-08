

namespace TextBob;

/// <summary>
/// Describes a text editor handler.
/// </summary>
public interface ITextEditorHandler
{
    /// <summary>
    /// The length of the selected text.
    /// </summary>
    int SelectionLength { get; }

    /// <summary>
    /// The start position of the selected text.
    /// </summary>
    int SelectionStart { get; }

    /// <summary>
    /// The selected text.
    /// </summary>
    string SelectedText { get; }
}