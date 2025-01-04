using lib.remnant2.analyzer.Model;
using RemnantOverseer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RemnantOverseer.Utilities;
internal class DatasetMapper
{
    public static MappedZones MapCharacterToZones(Character characterData)
    {
        var missingItemIds = characterData.Profile.MissingItems.Select(x => x["Id"]).ToList();

        var result = new MappedZones
        {
            CampaignZoneList = MapZonesToZones([.. characterData.Save.Campaign.Zones, characterData.Save.Campaign.Ward13], missingItemIds, characterData.Save.Campaign.RespawnPoint)
        };
        if (characterData.Save.Adventure != null)
        {
            result.AdventureZoneList = MapZonesToZones([.. characterData.Save.Adventure.Zones, characterData.Save.Adventure.Ward13], missingItemIds, characterData.Save.Adventure.RespawnPoint);
        }
        return result;
    }

    public static MappedCharacters MapCharacters(List<Character> characters)
    {
        var result = new MappedCharacters();
        foreach (var character in characters)
        {
            var mappedCharacter = new Models.Character();
            mappedCharacter.Index = character.Index;
            mappedCharacter.Archetype = Enum.Parse<Archetypes>(character.Profile.Archetype);
            mappedCharacter.SubArchetype = Enum.Parse<Archetypes>(character.Profile.SecondaryArchetype);
            mappedCharacter.ObjectCount = character.Profile.AcquiredItems;
            mappedCharacter.PowerLevel = character.Profile.ItemLevel; // Yes.
            mappedCharacter.ActiveWorld = Enum.Parse<WorldTypes>(character.ActiveWorldSlot.ToString(), true);
            mappedCharacter.IsHardcore = character.Profile.IsHardcore;
            mappedCharacter.Playtime = character.Save.Playtime ?? TimeSpan.Zero;

            result.CharacterList.Add(mappedCharacter);
        }

        // Just in case
        result.CharacterList = result.CharacterList.OrderBy((m) => m.Index).ToList();

        return result;
    }

    private static List<Models.Zone> MapZonesToZones(List<Zone> zones, List<string> missingItemIds, RespawnPoint? respawnPoint)
    {
        //var locnames = new List<string>();

        var result = new List<Models.Zone>();
        foreach (var zone in zones)
        {
            var zoneModel = new Models.Zone
            {
                Name = zone.Name,
                Story = zone.Story,
                Locations = []
            };

            // Map Locations
            // TODO: Consider nesting locations
            foreach (var location in zone.Locations)
            {
                //locnames.Add(location.Name);
                var locationModel = new Models.Location
                {
                    Name = location.Name,
                    Items = [],
                    IsSimulacrumPresent = location.Simulacrum,
                    IsSimulacrumLooted = location.SimulacrumLooted,
                    IsTraitBookPresent = location.TraitBook,
                    IsTraitBookLooted = location.TraitBookLooted,

                    // not supported yet
                    IsBloodmoon = location.Bloodmoon
                };

                // Map Items
                foreach (var lootGroup in location.LootGroups)
                {
                    foreach (var item in lootGroup.Items)
                    {
                        Enum.TryParse<ItemTypes>(item.Type.Replace("_", ""), true, out var itemType); // If false, will default to default value in enum, aka Unknown
                        Enum.TryParse<OriginTypes>(lootGroup.Type.Replace(" ", ""), true, out var originType);
                        var itemModel = new Models.Item
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Description = item.ItemNotes,
                            IsLooted = item.IsLooted,
                            Type = itemType,
                            OriginType = originType,
                            OriginName = lootGroup.Name ?? string.Empty,

                            IsDuplicate = !missingItemIds.Contains(item.Id),
                            IsCoop = item.Properties.ContainsKey("Coop") && item.Properties["Coop"] == "True"
                        };
                        if (itemModel.OriginName.StartsWith("Monster in the"))
                        {
                            ;
                        }
                        locationModel.Items.Add(itemModel);
                    }
                }

                // Respawn point checks
                if (respawnPoint != null && respawnPoint.Type != lib.remnant2.analyzer.Enums.RespawnPointType.None)
                {
                    switch (respawnPoint.Type)
                    {
                        case lib.remnant2.analyzer.Enums.RespawnPointType.WorldStone:
                            if (location.WorldStones.Contains(respawnPoint.Name))
                            {
                                SetAsRespawnLocation(locationModel, respawnPoint);
                            }
                            break;
                        case lib.remnant2.analyzer.Enums.RespawnPointType.Checkpoint:
                            if (location.Name == respawnPoint.Name)
                            {
                                SetAsRespawnLocation(locationModel, respawnPoint);
                            }
                            break;
                        case lib.remnant2.analyzer.Enums.RespawnPointType.ZoneTransition:
                            var name = respawnPoint.Name.Split("/")[0];
                            if (location.Name == name)
                            {
                                SetAsRespawnLocation(locationModel, respawnPoint);
                            }
                            break;
                    }
                }

                zoneModel.Locations.Add(locationModel);
            }

            result.Add(zoneModel);
        }

        //var t = locnames;
        return result;
    }

    private static void SetAsRespawnLocation(Models.Location locationModel, RespawnPoint respawnPoint)
    {
        locationModel.IsRespawnLocation = true;
        locationModel.RespawnPointName = respawnPoint.Name;
        locationModel.RespawnPointType = Enum.Parse<RespawnPointType>(respawnPoint.Type.ToString(), true);
    }
}

    internal class MappedZones
{
    public List<Models.Zone> CampaignZoneList { get; set; } = [];
    public List<Models.Zone> AdventureZoneList { get; set; } = [];
}

internal class MappedCharacters
{
    public List<Models.Character> CharacterList { get; set; } = [];
}