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
    
    /// <summary>
    /// Caret offset.
    /// </summary>
    int CaretOffset { get; }
    
    /// <summary>
    /// Caret line.
    /// </summary>
    int CaretLine { get; }
    
    /// <summary>
    /// Caret column.
    /// </summary>
    int CaretColumn { get; }


    /// <summary>
    /// Sets UI focus to the text editor.
    /// </summary>
    void FocusTextEditor();
    
    /// <summary>
    /// Sets UI focus to the text buffers list.
    /// </summary>
    void FocusTextBuffersList();
}
