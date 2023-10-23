using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace TextBob.ViewModels;

public class AppViewModel : ViewModelBase
{ 
    private AboutWindow? _aboutWindow;
    
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