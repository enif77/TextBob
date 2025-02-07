namespace TextBob.ViewModels;

using System.Reactive;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

using ReactiveUI;


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
                if (mainWindow != null)
                {
                    mainWindow.Activate();      // Activate the window
                    mainWindow.Topmost = true;  // Set the window as topmost
                    mainWindow.Topmost = false; // Reset the topmost property
                }
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
