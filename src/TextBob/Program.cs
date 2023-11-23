using System;
using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

using Avalonia;


namespace TextBob;

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
        const string configFileName = ".text-bob.json";
        
        var configFileRootPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        
        var configPath = Path.Combine(
            configFileRootPath,
            configFileName);
        if (File.Exists(configPath) == false)
        {
            File.WriteAllText(configPath, Settings.DefaultSettings.ToJson());
        }
        
        var builder = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                new PhysicalFileProvider(
                    configFileRootPath,
                    ExclusionFilters.None),
                configFileName,
                optional: false,
                reloadOnChange: false);  // This slows down app startup. Also - not sure, how to react to config reloads.

        var config = builder.Build();

        Settings = config.GetSection("Settings")
            .Get<Settings>() ?? Settings.DefaultSettings;

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
