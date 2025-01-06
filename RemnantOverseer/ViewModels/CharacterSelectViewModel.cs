using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RemnantOverseer.Models;
using RemnantOverseer.Models.Enums;
using RemnantOverseer.Models.Messages;
using RemnantOverseer.Services;
using RemnantOverseer.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemnantOverseer.ViewModels;
public partial class CharacterSelectViewModel: ViewModelBase
{
    private readonly SaveDataService _saveDataService;

    [ObservableProperty]
    private List<Character> _characters = [];

    [ObservableProperty]
    private int _selectedCharacterIndex = -1; // Have to be set or else list will not update the binding

    //[ObservableProperty]
    //private Character _selectedCharacter;

    [ObservableProperty]
    private bool _isLoading = false;

    public CharacterSelectViewModel(SaveDataService saveDataService)
    {
        _saveDataService = saveDataService;

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

    //partial void OnSelectedCharacterIndexChanged(int value)
    //{
    //    Task.Run(async () => {
    //        // Feeling TOO snappy without a delay. Look into making it feel better later
    //        await Task.Delay(125);
    //        Messenger.Send(new CharacterSelectChangedMessage(value));
    //    });
    //}

    //partial void OnSelectedCharacterChanged(Character? oldValue, Character newValue)
    //{
    //    if (newValue is null) return;

    //    Task.Run(async () =>
    //    {
    //        // Feeling TOO snappy without a delay. Look into making it feel better later
    //        await Task.Delay(125);
    //        Messenger.Send(new CharacterSelectChangedMessage(newValue.Index));
    //    });
    //}

    [RelayCommand]
    public void CharacterSelected(Character selectedCharacter)
    {
        // Could remove the SelectedCharacterIndex field cmopletely and simply compare to itself
        if (selectedCharacter.Index == SelectedCharacterIndex) return;

        foreach (var character in Characters)
        {
            character.IsSelected = character.Index == selectedCharacter.Index;
        }
        SelectedCharacterIndex = selectedCharacter.Index;
        Task.Run(async () =>
        {
            // Feeling TOO snappy without a delay. Look into making it feel better later
            await Task.Delay(125);
            Messenger.Send(new CharacterSelectChangedMessage(SelectedCharacterIndex));
        });
    }

    private async Task ReadSave(bool setActiveCahracter)
    {
        IsLoading = true;

        var data = await _saveDataService.GetSaveData();
        if (data != null && data.Characters.Count > 0)
        {
            var mappedCharacters = DatasetMapper.MapCharacters(data.Characters).CharacterList;
#if DEBUG
            //mappedCharacters.Add(new Character() { ObjectCount = 0, Archetype = Archetypes.Unknown, Index = 2 });
            //mappedCharacters.Add(new Character() { ObjectCount = 10, Archetype = Archetypes.Invader, Index = 3, PowerLevel = 4, Playtime = TimeSpan.FromHours(10) });
#endif

            if (setActiveCahracter)
            {
                // Calling directly will switch to worldview
#pragma warning disable MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
                _selectedCharacterIndex = data.ActiveCharacterIndex;
#pragma warning restore MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
                OnPropertyChanged(nameof(SelectedCharacterIndex));

                //_selectedCharacter = Characters[data.ActiveCharacterIndex];
                //OnPropertyChanged(nameof(SelectedCharacter));
            }

            if (SelectedCharacterIndex >= 0)
            {
                foreach (var character in mappedCharacters)
                {
                    character.IsSelected = character.Index == SelectedCharacterIndex;
                }
            }

            Characters = mappedCharacters;
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
