using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;

using ReactiveUI;
using TextBob.Views;


namespace TextBob.ViewModels;

/// <summary>
/// View model for the whole application.
/// </summary>
public class AppViewModel : ReactiveObject
{ 
    #region properties

    private string? _name;
    
    /// <summary>
    /// The application name.
    /// </summary>
    public string? Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    
    
    private readonly string? _versionInfo;

    /// <summary>
    /// The version info.
    /// </summary>
    public string? VersionInfo
    {
        get => _versionInfo;
        init => this.RaiseAndSetIfChanged(ref _versionInfo, value);
    }
    
    #endregion
    
    
    #region commands
    
    public ReactiveCommand<Unit, Unit> ShowCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }
    
    #endregion

    
    #region ctor
    
    public AppViewModel()
    {
        ShowCommand = ReactiveCommand.Create(() =>
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                var mainWindow = lifetime.MainWindow;
                if (mainWindow == null)
                {
                    return;
                }

                // This makes the main window active/focused.
                mainWindow.Activate();
                
                // This ensures main window to be visible.
                mainWindow.Topmost = true;
                mainWindow.Topmost = false;
            }
        });
        
        ExitCommand = ReactiveCommand.Create(() =>
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
    /// <param name="snapshotFilePath">A path to the snapshot file.</param>
    /// <returns>A text from the snapshot.</returns>
    public string LoadTextSnapshot(string snapshotFilePath)
    {
        if (string.IsNullOrEmpty(snapshotFilePath))
        {
            return string.Empty;
        }
        
        try
        {
            snapshotFilePath = GetSnapshotFilePath(snapshotFilePath);
            
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
    /// <param name="snapshotFilePath">A path to the snapshot file.</param>
    /// <param name="text">A text to be saved to the snapshot file.</param>
    public void SaveTextSnapshot(string snapshotFilePath, string text)
    {
        if (string.IsNullOrEmpty(snapshotFilePath))
        {
            return;
        }
        
        try
        {
            File.WriteAllText(GetSnapshotFilePath(snapshotFilePath), text);
        }
        catch (IOException)
        {
            // We are OK with all IO exceptions here.
        }
    }
    
    
    private string GetSnapshotFilePath(string snapshotFilePath)
    {
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
    

    // public async Task ShowAboutWindow()
    // {
    //     if (_aboutWindow is not null)
    //     {
    //         _aboutWindow.Activate();
    //         
    //         return;
    //     }
    //  
    //     var applicationLifeTime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
    //     var mainWindow = applicationLifeTime?.MainWindow;
    //     if (mainWindow is null)
    //     {
    //         return;
    //     }
    //     
    //     // https://docs.avaloniaui.net/docs/basics/user-interface/assets
    //     var uri = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
    //         ? new Uri("avares://TextBob/Assets/macOS/about.txt")
    //         : new Uri("avares://TextBob/Assets/Windows/about.txt");
    //     
    //     string aboutText;
    //     using (var reader = new StreamReader(AssetLoader.Open(uri), System.Text.Encoding.UTF8))
    //     {
    //         aboutText = await reader.ReadToEndAsync();
    //     }
    //     
    //     _aboutWindow = new AboutWindow()
    //     {
    //         DataContext = new AboutWindowViewModel()
    //         {
    //             AppViewModel = this,
    //             VersionInfo = VersionInfo,
    //             Text = aboutText
    //         }
    //     };
    //     await _aboutWindow.ShowDialog(mainWindow);
    //     _aboutWindow = null;
    // }
}
