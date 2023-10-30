using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace TextBob.ViewModels;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    
    protected bool RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        RaisePropertyChanged(propertyName);
            
        return true;

    }
    
    
    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

// Based on: https://github.com/AvaloniaUI/Avalonia/blob/master/samples/MiniMvvm/ViewModelBase.cs
