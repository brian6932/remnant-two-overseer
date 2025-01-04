using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using lib.remnant2.analyzer;
using lib.remnant2.analyzer.Model;
using RemnantOverseer.Models.Messages;
using System;
using System.IO;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace RemnantOverseer.Services;
public class SaveDataService
{
    private readonly SettingsService _settingsService;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    Subject<DateTime> _fileUpdateSubject = new Subject<DateTime>();

    private string[]? _profileSummaries;
    private Dataset? _dataset;
    private int _lastCharacterCount = 0;

    private string? FilePath => _settingsService.Get().SaveFilePath;

    private static readonly FileSystemWatcher FileWatcher = new()
    {
        Filter = "profile.sav",
    };

    public SaveDataService(SettingsService settingsService)
    {
        _settingsService = settingsService;

        FileWatcher.Changed += OnSaveFileChanged;
        FileWatcher.Created += OnSaveFileChanged;
        FileWatcher.Deleted += OnSaveFileChanged;

        // File watcher often raises multiple events for one file update
        _fileUpdateSubject.Throttle(TimeSpan.FromSeconds(1)).Subscribe(async events => await OnSaveFileChangedDebounced());
        WeakReferenceMessenger.Default.Register<SaveFilePathChangedMessage>(this, async (r, m) => await SaveFilePathChangedMessageHandler(m));
    }

    //public async Task<string[]> GetProfileSummaries()
    //{
    //    if (FilePath is null) return [];

    //    await _semaphore.WaitAsync();
    //    try
    //    {
    //        if (_profileSummaries == null)
    //        {
    //            _profileSummaries = await Task.Run(() => Analyzer.GetProfileStrings(FilePath));
    //        }
    //    }
    //    finally
    //    {
    //        _semaphore.Release();
    //    }
    //    return _profileSummaries;
    //}

    public async Task<Dataset?> GetSaveData()
    {
        if (FilePath is null) return null;

        // TODO: Add timeout?
        await _semaphore.WaitAsync();
        try
        {
            if (_dataset == null)
            {
                _dataset = await Task.Run(() => Analyzer.Analyze(FilePath));
            }
        }
        finally
        {
            _semaphore.Release();
        }
        return _dataset;
    }

    public void Reset()
    {
        _profileSummaries = null;
        _dataset = null;
    }

    public bool StartWatching()
    {
        if (Directory.Exists(FilePath))
        {
            FileWatcher.Path = FilePath;
            FileWatcher.EnableRaisingEvents = true;
            return true;
        }
        else
        {
            FileWatcher.EnableRaisingEvents = false;
            return false;
            // TODO: throw new ArgumentException($"Target directory [{FilePath}] doesn't exist");
        }
    }
    public void PauseWatching()
    {
        FileWatcher.EnableRaisingEvents = false;
    }
    public void ResumeWatching()
    {
        if (FileWatcher.Path == null) return;
        FileWatcher.EnableRaisingEvents = true;
    }

    private void OnSaveFileChanged(object sender, FileSystemEventArgs e)
    {
        _fileUpdateSubject.OnNext(DateTime.UtcNow);
    }

    private async Task OnSaveFileChangedDebounced()
    {
        _dataset = await Task.Run(() => Analyzer.Analyze(FilePath));
        // If the number of character changed, we can't rely on previous index anymore. There is no way to uniquely id  characters, so we will just reset
        var countChanged = _dataset.Characters.Count > _lastCharacterCount;
        _lastCharacterCount = _dataset.Characters.Count;
        WeakReferenceMessenger.Default.Send(new SaveFileChangedMessage(countChanged));
    }

    private async Task SaveFilePathChangedMessageHandler(SaveFilePathChangedMessage message)
    {
        PauseWatching();
        _dataset = await Task.Run(() => Analyzer.Analyze(FilePath));
        _lastCharacterCount = _dataset.Characters.Count;
        StartWatching();
        WeakReferenceMessenger.Default.Send(new SaveFileChangedMessage(true));
    }

    // TODO: Remove
    public async Task ExportAsJson()
    {
        if (FilePath is null) throw new ArgumentNullException("File path not set");
        await Task.Run(() => Exporter.Export(FilePath!, FilePath, false, false, true));
    }

    public void ParseSave()
    {
        if (FilePath is null) throw new ArgumentNullException("File path not set");

        var saves = lib.remnant2.analyzer.Analyzer.GetProfileStrings(FilePath);
        Debug.WriteLine(saves);

        var data = lib.remnant2.analyzer.Analyzer.Analyze(FilePath);
        Debug.WriteLine(data);
    }
}
