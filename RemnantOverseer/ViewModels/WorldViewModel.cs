using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json.Linq;
using RemnantOverseer.Models;
using RemnantOverseer.Models.Messages;
using RemnantOverseer.Services;
using RemnantOverseer.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace RemnantOverseer.ViewModels;

public partial class WorldViewModel : ViewModelBase
{
    private readonly SaveDataService _saveDataService;

    private MappedZones _mappedZones = new();

    [ObservableProperty]
    private ObservableCollection<Zone> _filteredZones = [];

    [ObservableProperty]
    private bool _isLoading = false;

    public WorldViewModel(SaveDataService saveDataService, SettingsService settingsService)
    {
        _saveDataService = saveDataService;
        Task.Run(async () => { await ReadSave(0, true); this.IsActive = true; });

        _filterTextSubject
          .Throttle(TimeSpan.FromMilliseconds(400))
          .Subscribe(OnFilterTextChangedDebounced);

        //IsActive = true; // Activate messenger on the first load
    }

    [ObservableProperty]
    private bool _isCampaignSelected = true; // TODO: add disabling when no adventure

    // What?
    // https://devblogs.microsoft.com/ifdef-windows/announcing-net-community-toolkit-v8-0-0-preview-3/#partial-property-changed-methods
    partial void OnIsCampaignSelectedChanged(bool value)
    {
        // ResetLocationToggles();
        ApplyFilter();
    }

    [ObservableProperty]
    private bool _isGlobalExpandOn = true;

    [RelayCommand]
    private void ExpandTreeNodes()
    {
        IsGlobalExpandOn = !IsGlobalExpandOn;
    }

    // TODO: add to settings
    [ObservableProperty]
    private bool _hideDuplicates = true;

    partial void OnHideDuplicatesChanged(bool value)
    {
        ApplyFilter();
    }

    #region Filtering

    [ObservableProperty]
    private bool _isNerudFilterChecked = false;

    [RelayCommand]
    public void NerudFilterToggled()
    {
        if (IsNerudFilterChecked)
        {
            IsYaeshaFilterChecked = false;
            IsLosomnFilterChecked = false;
        }
        ApplyFilter();
    }

    [ObservableProperty]
    private bool _isYaeshaFilterChecked = false;

    [RelayCommand]
    public void YaeshaFilterToggled()
    {
        if (IsYaeshaFilterChecked)
        {
            IsNerudFilterChecked = false;
            IsLosomnFilterChecked = false;
        }
        ApplyFilter();
    }

    [ObservableProperty]
    private bool _isLosomnFilterChecked = false;

    [RelayCommand]
    public void LosomnFilterToggled()
    {
        if (IsLosomnFilterChecked)
        {
            IsYaeshaFilterChecked = false;
            IsNerudFilterChecked = false;
        }
        ApplyFilter();
    }

    [RelayCommand]
    public void ResetFilters()
    {
        ResetLocationToggles();
        if (FilterText == null) ApplyFilter(); // to avoid filtering twice
        FilterText = null;
    }

    [ObservableProperty]
    private string? _filterText = null;

    private Subject<string?> _filterTextSubject = new Subject<string?>();

    partial void OnFilterTextChanged(string? value)
    {
        _filterTextSubject.OnNext(value);
        //ApplyFilter(value);
    }
    private void OnFilterTextChangedDebounced(string? value)
    {
        ApplyFilter(value);
    }

    private void ApplyFilter()
    {
        ApplyFilter(FilterText);
    }

    // TODO: Optimize this. How is it so slow? Removing string comparisons does nothing
    private void ApplyFilter(string? value)
    {
        //optimization, not compatible with hide dupes
        //if (string.IsNullOrEmpty(value)) { FilteredZones = Zones; return; }

        var tempZones = IsCampaignSelected ? _mappedZones.CampaignZoneList : _mappedZones.AdventureZoneList;
        var tempFilteredZones = new List<Zone>();
        foreach (var zone in tempZones)
        {
            // Toggles only applicable to campaign
            if (IsCampaignSelected)
            {
                if (IsNerudFilterChecked && zone.Name != LocationStrings.Nerud) continue;
                if (IsYaeshaFilterChecked && zone.Name != LocationStrings.Yaesha) continue;
                if (IsLosomnFilterChecked && zone.Name != LocationStrings.Losomn) continue;
            }

            var tempZone = zone.ShallowCopy(); // new Zone() { Name = zone.Name, Locations = [] };
            tempZone.Locations = [];

            foreach (var location in zone.Locations)
            {
                var tempLocation = location.ShallowCopy(); // new Location() { Name = location.Name, Items = [] };
                tempLocation.Items = [];

                // Add more processing if necessary. Remove special characters?
                List<Item> tempItems = [];
                if (!string.IsNullOrEmpty(value))
                {
                    tempItems = location.Items.Where(i => i.Name.Contains(value, StringComparison.OrdinalIgnoreCase) || i.OriginName.Contains(value, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else
                {
                    tempItems.AddRange(location.Items);
                }
                if (HideDuplicates)
                {
                    tempItems = tempItems.Where(i => !i.IsDuplicate).ToList();
                }
                if (tempItems.Count != 0) { tempLocation.Items = tempItems; tempZone.Locations.Add(tempLocation); }
            }
            if (tempZone.Locations.Count != 0) { tempFilteredZones.Add(tempZone); }
        }

        FilteredZones = new(tempFilteredZones);
    }
    #endregion Filtering

#pragma warning disable MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
    private async Task ReadSave(int characterIndex, bool isCharacterCountChanged = false)
    {
        IsLoading = true;

        var dataset = await _saveDataService.GetSaveData();
        if (dataset == null)
        {
            IsLoading = false;
            return;
        }

        if (isCharacterCountChanged)
        {
            characterIndex = dataset.ActiveCharacterIndex;
        }

        _mappedZones = DatasetMapper.MapCharacterToZones(dataset.Characters[characterIndex]);
        // Call private field to avoid filtering on every assignment
        if (dataset.Characters[characterIndex].ActiveWorldSlot == lib.remnant2.analyzer.Enums.WorldSlot.Campaign)
        {
            _isCampaignSelected = true;
            OnPropertyChanged(nameof(IsCampaignSelected));
        }
        else
        {
            _isCampaignSelected = false;
            OnPropertyChanged(nameof(IsCampaignSelected));
        }

        ResetLocationToggles();
        _filterText = null;
        OnPropertyChanged(nameof(FilterText));
        ApplyFilter();

        IsLoading = false;
    }
#pragma warning restore MVVMTK0034 // Direct field reference to [ObservableProperty] backing field

    private void ResetLocationToggles()
    {
        IsNerudFilterChecked = false;
        IsYaeshaFilterChecked = false;
        IsLosomnFilterChecked = false;
    }

    #region Messages
    protected override void OnActivated()
    {
        Messenger.Register<WorldViewModel, CharacterSelectChangedMessage>(this, (r, m) => {
            IsLoading = true; // Look into it later, sometimes task starts just a moment too late and the old stuff still can be seen
            Task.Run(async () => await ReadSave(m.Value));
        });

        Messenger.Register<WorldViewModel, SaveFileChangedMessage>(this, (r, m) => {
            IsLoading = true;
            Task.Run(async () => await ReadSave(0, m.CharacterCountChanged));
        });
    }
    #endregion Messages
}
