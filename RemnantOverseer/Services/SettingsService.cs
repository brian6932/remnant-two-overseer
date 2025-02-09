using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using lib.remnant2.analyzer.SaveLocation;
using RemnantOverseer.Models.Messages;
using RemnantOverseer.Utilities;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RemnantOverseer.Services;
public class SettingsService
{
    private readonly object _lock = new object();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly string path = Path.Combine(AppContext.BaseDirectory, "settings.json");
    private Settings _settings = new();
    private JsonSerializerOptions _options = new() { WriteIndented = true };

    public SettingsService()
    {
        // XAML Designer support
        if (Design.IsDesignMode)
        {
            return; // TODO?
        }        
    }

    public void Initialize()
    {
        if (File.Exists(path))
        {
            // Considering making a toast for this and remaking the file. But I think crashing is more educational
            string json = File.ReadAllText(path);
            _settings = JsonSerializer.Deserialize<Settings>(json)!;
        }

        if (_settings.SaveFilePath == null)
        {
            // Try to get a path
            try
            {
                _settings.SaveFilePath = SaveUtils.GetSaveFolder();
                Update(_settings);
                WeakReferenceMessenger.Default.Send(new NotificationInfoMessage(NotificationStrings.DefaultLocationFound));
                Log.Instance.Information(NotificationStrings.DefaultLocationFound);
            }
            catch
            {
                WeakReferenceMessenger.Default.Send(new NotificationWarningMessage(NotificationStrings.DefaultLocationNotFound));
                Log.Instance.Warning(NotificationStrings.DefaultLocationNotFound);
                return;
            }
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
        if (settings.SaveFilePath == null)
        {
            ;
        }
        try
        {
            var json = JsonSerializer.Serialize(settings, options: _options);
            lock (_lock)
            {
                File.WriteAllText(path, json);
                _settings = settings;
            }
        }
        catch (Exception ex)
        {
            var message = NotificationStrings.ErrorWhenUpdatingSettings + Environment.NewLine + ex.Message;
            WeakReferenceMessenger.Default.Send(new NotificationWarningMessage(message));
            Log.Instance.Warning(message);
        }
    }

    public async Task UpdateAsync(Settings settings)
    {
        if (settings.SaveFilePath == null)
        {
            ;
        }
        await _semaphore.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(settings, options: _options);
            await File.WriteAllTextAsync(path, json);
            _settings = settings;
        }
        catch (Exception ex)
        {
            var message = NotificationStrings.ErrorWhenUpdatingSettings + Environment.NewLine + ex.Message;
            WeakReferenceMessenger.Default.Send(new NotificationWarningMessage(message));
            Log.Instance.Warning(message);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
