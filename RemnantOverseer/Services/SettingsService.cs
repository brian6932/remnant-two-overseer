using Avalonia.Controls;
using lib.remnant2.analyzer;
using System;
using System.IO;
using System.Text.Json;

namespace RemnantOverseer.Services;
public class SettingsService
{
    private readonly object _lock = new object();
    private readonly string path = Path.Combine(AppContext.BaseDirectory, "settings.json");
    private Settings _settings;
    private JsonSerializerOptions _options = new() { WriteIndented = true };

    public SettingsService()
    {
        _settings = new Settings();

        // XAML Designer support
        if (Design.IsDesignMode)
        {
            return;
        }

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _settings = JsonSerializer.Deserialize<Settings>(json)!;
        }
        if (_settings.SaveFilePath == null)
        {
            // Try to get a path
            try
            {
                _settings.SaveFilePath = Utils.GetSteamSavePath();
                Update(_settings);
            }
            catch
            {
                // TODO: send a message that auto detect failed
            }

            // TODO: send a toast message that we tried to get the path?
        }
    }

    // Application is simple enough to allow client to read the whole config.
    // Could implement more granular approach later
    public Settings Get()
    {
        // Return a clone?
        return _settings;
    }

    public void Update(Settings settings)
    {
        var json = JsonSerializer.Serialize(settings, options: _options);
        lock (_lock)
        {
            File.WriteAllText(path, json);
            _settings = settings;
        }
    }
}
