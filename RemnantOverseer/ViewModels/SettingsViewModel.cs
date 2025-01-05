using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RemnantOverseer.Models.Messages;
using RemnantOverseer.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RemnantOverseer.ViewModels;
public partial class SettingsViewModel: ViewModelBase
{
    private readonly SettingsService _settingsService;

    [ObservableProperty]
    private string? _filePath;

    public SettingsViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;
        FilePath = _settingsService.Get()?.SaveFilePath ?? null;

        if(Design.IsDesignMode)
        {
            FilePath = @"C:\Remnant\Remnant 2: I'm Beginning to Remn";
        }
    }

    [RelayCommand]
    public async Task OpenFile()
    {
        var topLevel = FileDialogManager.GetTopLevelForContext(this);
        if (topLevel != null)
        {
            var storageFiles = await topLevel.StorageProvider.OpenFilePickerAsync(
                            new FilePickerOpenOptions()
                            {
                                FileTypeFilter = [Saves],
                                AllowMultiple = false,
                                Title = "Select the profile file"
                            });

            if (storageFiles.Count > 0)
            {
                var selectedFile = storageFiles.First();
                var settings = _settingsService.Get();
                var newPath = Path.GetDirectoryName(selectedFile.TryGetLocalPath());

                if (newPath == settings.SaveFilePath) return;
                
                settings.SaveFilePath = newPath;
                _settingsService.Update(settings);
                FilePath = newPath;
                WeakReferenceMessenger.Default.Send(new SaveFilePathChangedMessage(FilePath!));
            }
        }
    }

    public static FilePickerFileType Saves { get; } = new("Remnant 2 save files")
    {
        Patterns = ["profile.sav"],
    };
}
