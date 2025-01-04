using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RemnantOverseer.Models;
using RemnantOverseer.Models.Enums;
using RemnantOverseer.Models.Messages;
using RemnantOverseer.Services;
using RemnantOverseer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RemnantOverseer.ViewModels;
public partial class CharacterSelectViewModel: ViewModelBase
{
    private readonly SaveDataService _saveDataService;
    private readonly SettingsService _settingsService;

    public CharacterSelectViewModel(SaveDataService saveDataService, SettingsService settingsService)
    {
        _saveDataService = saveDataService;
        _settingsService = settingsService;

        if(Design.IsDesignMode)
        {
            Characters =
            [
                new Character() { ObjectCount = 500, Archetype = Archetypes.Gunslinger, SubArchetype = Archetypes.Archon, Index = 0 },
                new Character() { ObjectCount = 123, Archetype = Archetypes.Medic, Index = 1 },
                new Character() { ObjectCount = 0, Archetype = Archetypes.Unknown, Index = 2 },
                new Character() { ObjectCount = 10, Archetype = Archetypes.Invader, Index = 3 },
            ];
            return;
        }

        Task.Run(async () => { await ReadSave(true); this.IsActive = true; });
    }

    [ObservableProperty]
    private List<Character> _characters = [];

    //[ObservableProperty]
    //private Character? _activeCharacter = null;

    [ObservableProperty]
    private int _selectedCharacterIndex = -1; // Have to be set or else list will not update the binding

    partial void OnSelectedCharacterIndexChanged(int value)
    {
        Task.Run(async () => { 
            await Task.Delay(125);
            Messenger.Send(new CharacterSelectChangedMessage(value));
        });
    }

    [ObservableProperty]
    private bool _isLoading = false;
    
    // consider using new handler
    //partial void OnActiveCharacterChanged(Character? value)
    //{
    //    // Technically overwrites save the first time character is set from it with the same value. Look into it when free time exists
    //    if (value != null)
    //    {
    //        // Feeling TOO snappy without a delay. Also need to figure out how to call this only on pointer release instead of on pointer down
    //        Task.Run(async () => { await Task.Delay(125); Messenger.Send(new CharacterSelectChangedMessage(value)); });
    //    }
    //}

    private async Task ReadSave(bool setActiveCahracter)
    {
        IsLoading = true;

        var data = await _saveDataService.GetSaveData(); //await _saveDataService.GetProfileSummaries();
        if (data != null && data.Characters.Count > 0)
        {
            var mappedCharacters = DatasetMapper.MapCharacters(data.Characters).CharacterList;
#if DEBUG
            //mappedCharacters.Add(new Character() { ObjectCount = 0, Archetype = Archetypes.Unknown, Index = 2 });
            //mappedCharacters.Add(new Character() { ObjectCount = 10, Archetype = Archetypes.Invader, Index = 3, PowerLevel = 4, Playtime = TimeSpan.FromHours(10) });
#endif
            Characters = mappedCharacters;

            // TODO? Ensure this is only called on save load
            if (setActiveCahracter)
            {
                // Calling directly will switch to worldview
#pragma warning disable MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
                //_activeCharacter = mappedCharacters[data.ActiveCharacterIndex];
                _selectedCharacterIndex = data.ActiveCharacterIndex;
#pragma warning restore MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
                OnPropertyChanged(nameof(SelectedCharacterIndex));
            }
        }
        // TODO: Sometimes char screen is empty. I think this might be the cause.
        if (data is null)
        {
            // throw new ArgumentNullException(nameof(data));
        }

        IsLoading = false;
    }

    #region Messages
    protected override void OnActivated()
    {
        Messenger.Register<CharacterSelectViewModel, SaveFileChangedMessage>(this, (r, m) => {
            IsLoading = true;
            Task.Run(async () => await ReadSave(m.CharacterCountChanged));
        });
    }
    #endregion Messages
}
