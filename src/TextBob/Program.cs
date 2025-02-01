namespace TextBob;

using System;
using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

using Avalonia;
using Avalonia.ReactiveUI;

using TextBob.Models;


internal static class Program
{
    /// <summary>
    /// Global application settings.
    /// </summary>
    public static Settings Settings { get; private set; } = Settings.DefaultSettings;


    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var configFileRootPath = GetSettingsFileRootPath();
        var configPath = GetSettingsFilePath();
        
        // If config file does not exist, create it with default values.
        if (File.Exists(configPath) == false)
        {
            try
            {
                File.WriteAllText(configPath, Settings.DefaultSettings.ToJson());
            }
            catch (IOException)
            {
                // We are OK with an IO related exception.
            }
        }
        
        var builder = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                new PhysicalFileProvider(      // This provider allows us to load config from an absolute path.
                    configFileRootPath,
                    ExclusionFilters.System),  // We are reading from an hidden file.
                Defaults.ConfigFileName,
                optional: true,
                reloadOnChange: false);  // This slows down app startup. Also - not sure, how to react to config reloads.

        var config = builder.Build();

        Settings = config.GetSection("Settings")
            .Get<Settings>() ?? Settings.DefaultSettings;

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }
    
    /// <summary>
    /// Returns a path to the settings file.
    /// </summary>
    /// <returns>A path to the settings file.</returns>
    public static string GetSettingsFilePath()
    {
        
        return Path.Combine(
            GetSettingsFileRootPath(),
            Defaults.ConfigFileName);
    }
    
    
    private static string GetSettingsFileRootPath()
    {
        // We are reading and writing config file in user's home directory.
        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }
    

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
