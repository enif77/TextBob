using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using MiniMvvm;

using TextBob.Views;


namespace TextBob.ViewModels;

public class AppViewModel : ViewModelBase
{ 
    private AboutWindow? _aboutWindow;
    
    
    #region properties

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
    
    #endregion
    
    
    #region commands
    
    public MiniCommand ShowCommand { get; }
    public MiniCommand ExitCommand { get; }
    
    #endregion

    
    #region ctor
    
    public AppViewModel()
    {
        ShowCommand = MiniCommand.Create(() =>
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.MainWindow?.Activate();
            }
        });
        
        ExitCommand = MiniCommand.Create(() =>
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.Shutdown();
            }
        });
    }
    
    #endregion
    
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
            return File.Exists(snapshotFilePath)
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
        var snapshotFilePath = Program.Settings.Snapshots.FirstOrDefault()?.Path ?? Defaults.DefaultSnapshotFilePath;
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


    public async Task ShowAboutWindow()
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
        
        // https://docs.avaloniaui.net/docs/basics/user-interface/assets
        var uri = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? new Uri("avares://TextBob/Assets/macOS/about.txt")
            : new Uri("avares://TextBob/Assets/Windows/about.txt");
        
        string aboutText;
        using (var reader = new StreamReader(AssetLoader.Open(uri), System.Text.Encoding.UTF8))
        {
            aboutText = await reader.ReadToEndAsync();
        }
        
        _aboutWindow = new AboutWindow()
        {
            DataContext = new AboutWindowViewModel()
            {
                AppViewModel = this,
                VersionInfo = VersionInfo,
                Text = aboutText
            }
        };
        await _aboutWindow.ShowDialog(mainWindow);
        _aboutWindow = null;
    }
}
