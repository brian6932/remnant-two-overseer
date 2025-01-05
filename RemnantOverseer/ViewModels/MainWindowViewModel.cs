using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RemnantOverseer.Models.Messages;
using RemnantOverseer.Services;
using System;

namespace RemnantOverseer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly SettingsService _settingsService;
    private readonly SaveDataService _saveDataService;

    [ObservableProperty]
    private ViewModelBase _contentViewModel;

    [ObservableProperty]
    private bool _isCharacterViewSelected;

    [ObservableProperty]
    private bool _isWorldViewSelected;

    [ObservableProperty]
    private bool _isMissingItemsViewSelected;

    [ObservableProperty]
    private bool _isSettingsViewSelected;

    public WindowNotificationManager? NotificationManager { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable. (Is set in SwitchToWorldView)
    public MainWindowViewModel(SettingsService settingsService, SaveDataService saveDataService)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        _settingsService = settingsService;
        _saveDataService = saveDataService;
        SwitchToWorldView();
        IsActive = true; // Turn on the messenger https://github.com/CommunityToolkit/MVVM-Samples/issues/37
        _saveDataService.StartWatching();
    }

    [RelayCommand]
    public void SwitchToWorldView()
    {
        if (ContentViewModel is WorldViewModel) return;
        ContentViewModel = App.Resolve<WorldViewModel>();
        IsWorldViewSelected = true;
    }

    [RelayCommand]
    public void SwitchToCharacterView()
    {
        if (ContentViewModel is CharacterSelectViewModel) return;
        ContentViewModel = App.Resolve<CharacterSelectViewModel>();
        IsCharacterViewSelected = true;
    }

    [RelayCommand]
    public void SwitchToMissingItemsView()
    {
        if (ContentViewModel is MissingItemsViewModel) return;
        ContentViewModel = App.Resolve<MissingItemsViewModel>();
        IsMissingItemsViewSelected = true;
    }

    [RelayCommand]
    public void SwitchToSettingsView()
    {
        if (ContentViewModel is SettingsViewModel) return;
        ContentViewModel = App.Resolve<SettingsViewModel>();
        IsSettingsViewSelected = true;
    }

    #region Messages
    protected override void OnActivated()
    {
        // This should be replaced by a navigator in the future
        Messenger.Register<MainWindowViewModel, CharacterSelectChangedMessage>(this, (r, m) => {
            SwitchToWorldViewCommand.Execute(null);
        });

        // TODO: Do I want to switch views automatically?
        Messenger.Register<MainWindowViewModel, SaveFileChangedMessage>(this, (r, m) => {
            ;
        });

        Messenger.Register<MainWindowViewModel, NotificationErrorMessage>(this, (r, m) => {
            NotificationManager?.Show(new Notification("Error", m.Value, NotificationType.Error));
        });

        Messenger.Register<MainWindowViewModel, NotificationWarningMessage>(this, (r, m) => {
            NotificationManager?.Show(new Notification("Warning", m.Value, NotificationType.Warning));
        });

        Messenger.Register<MainWindowViewModel, NotificationInfoMessage>(this, (r, m) => {
            NotificationManager?.Show(new Notification("Information", m.Value, NotificationType.Information));
        });
    }

    public void Dispose()
    {
        _saveDataService.PauseWatching();
    }
    #endregion Messages
}
