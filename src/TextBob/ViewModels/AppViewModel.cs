using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

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
        
        _aboutWindow = new AboutWindow();
        await _aboutWindow.ShowDialog(mainWindow);
        _aboutWindow = null;
    }
}
