// using MiniMvvm;
//
//
// namespace TextBob.ViewModels;
//
// /// <summary>
// /// View model for the settings window.
// /// </summary>
// internal class SettingsWindowViewModel : ViewModelBase
// {
//     private AppViewModel? _appViewModel;
//
//     /// <summary>
//     /// The app view model.
//     /// </summary>
//     public AppViewModel? AppViewModel
//     {
//         get => _appViewModel;
//
//         set
//         {
//             if (_appViewModel == value)
//             {
//                 return;
//             }
//
//             _appViewModel = value;
//             
//             RaisePropertyChanged();
//         }
//     }
//     
//
//     private string? _text;
//
//     /// <summary>
//     /// The text.
//     /// </summary>
//     public string? Text
//     {
//         get => _text;
//
//         set
//         {
//             if (_text == value)
//             {
//                 return;
//             }
//
//             _text = value;
//             
//             RaisePropertyChanged();
//         }
//     }
// }
