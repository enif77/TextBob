using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

using TextBob.Views;


namespace TextBob.ViewModels;

public class AppViewModel : ViewModelBase
{ 
    private AboutWindow? _aboutWindow;
    private string? _name;


    /// <summary>
    /// The application name.
    /// </summary>
    public string? Name
    {
        get => _name;

        set
        {
            if (_name == value)
            {
                return;
            }

            _name = value;

            // We call RaisePropertyChanged() to notify the UI about changes.
            // We can omit the property name here because [CallerMemberName] will provide it for us.
            RaisePropertyChanged();
        }
    }
    
    
    private readonly string? _versionInfo;

    /// <summary>
    /// The version info.
    /// </summary>
    public string? VersionInfo
    {
        get => _versionInfo;

        init
        {
            if (_versionInfo == value)
            {
                return;
            }

            _versionInfo = value;
            
            RaisePropertyChanged();
        }
    }


    /// <summary>
    /// Loads text from the snapshot.
    /// </summary>
    /// <returns>A text from the snapshot.</returns>
    public string LoadTextSnapshot()
    {
        try
        {
            var snapshotFilePath = GetSnapshotFilePath();
                
            // Load text from the snapshot.
            return (File.Exists(snapshotFilePath))
                ? File.ReadAllText(snapshotFilePath)
                : string.Empty;
        }
        catch (IOException)
        {
            // We are OK with all IO exceptions here.
            return string.Empty;
        }
    }

    /// <summary>
    /// Saves text to the snapshot.
    /// </summary>
    /// <param name="text">A text to be saved to the snapshot file.</param>
    public void SaveTextSnapshot(string text)
    {
        var snapshotFilePath = GetSnapshotFilePath();
        if (string.IsNullOrEmpty(snapshotFilePath))
        {
            return;
        }
        
        try
        {
            File.WriteAllText(snapshotFilePath, text);
        }
        catch (IOException)
        {
            // We are OK with all IO exceptions here.
        }
    }


    private string GetSnapshotFilePath()
    {
        var snapshotFilePath = Program.Settings.SnapshotFilePath ?? string.Empty;
        if (File.Exists(snapshotFilePath))
        {
            return snapshotFilePath;
        }

        if (snapshotFilePath.StartsWith("~/"))
        {
            snapshotFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                snapshotFilePath[2..]);
        }

        return snapshotFilePath;
    }


    public async void ShowAboutWindow()
    {
        if (_aboutWindow is not null)
        {
            _aboutWindow.Activate();
            
            return;
        }
     
        var applicationLifeTime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var mainWindow = applicationLifeTime?.MainWindow;
        if (mainWindow is null)
        {
            return;
        }
        
        _aboutWindow = new AboutWindow()
        {
            DataContext = new AboutWindowViewModel()
            {
                AppViewModel = this,
                VersionInfo = VersionInfo,
                Text = @"Keyboard shortcuts
------------------

CTRL/CMD + C - Copy
CTRL/CMD + X - Cut
CTRL/CMD + V - Paste
CTRL/CMD + A - Select all
CTRL/CMD + Z - Undo
CTRL/CMD + Y - Redo
CTRL/CMD + F - Find
CTRL + H | CMD + ALT + F - Replace
Delete - Delete 
CTRL + Delete - Delete next word
CTRL/CMD + D - Delete line
Backspace - Delete previous character or selection 
CTRL + Backspace - Delete previous word
Enter - Enter paragraph break (a new line)
SHIFT + Enter - Enter line break
Tab - Tab forward
SHIFT + Tab - Tab backward 
Left - Move left by character
SHIFT + Left - Select left by character
ALT + SHIFT + Left - Box select left by character
Right - Move right by character
SHIFT + Right - Select right by character
ALT + SHIFT + Right - Box select right ry character
CTRL + Left - Move left by word
SHIFT + CTRL + Left - Select left by word
CTRL + Right - Move right by word
SHIFT + CTRL + Right - Select right by word
Up - Move up by line
SHIFT + Up - Select up by line
Down - Move down by line
SHIFT + Down - Select down by line
Page Down - Move down by page
SHIFT + Page Down - Select down by page
Page Up - Move up by page
SHIFT + Page Up - Select up by page
Home - Move to line start
SHIFT + Home - Select to line start
End - Move to line end
SHIFT + End - Select to line end
CTRL + Home - Move to document start
SHIFT + CTRL + Home - Select to document start
CTRL + End - Move to document end
SHIFT + CTRL + End - Select to document end
CTRL/CMD + I - Indent selection"
            }
        };
        await _aboutWindow.ShowDialog(mainWindow);
        _aboutWindow = null;
    }
}
