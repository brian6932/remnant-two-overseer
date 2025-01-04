using RemnantOverseer.Models.Enums;
using System.Collections.Generic;

namespace RemnantOverseer.Models;
public class Location
{
    public string Name { get; set; } = string.Empty;
    public List<Item> Items { get; set; } = [];
    public bool IsTraitBookPresent { get; set; }
    public bool IsSimulacrumPresent { get; set; }
    public bool IsTraitBookLooted { get; set; }
    public bool IsSimulacrumLooted { get; set; }
    public bool IsBloodmoon { get; set; }

    public bool IsRespawnLocation { get; set; }
    public RespawnPointType RespawnPointType { get; set; }
    public string RespawnPointName { get; set; } = string.Empty;
    public string? FormattedRespawnPointName
    {
        get
        {
            if (!IsRespawnLocation) return null;
            return RespawnPointType switch
            {
                RespawnPointType.WorldStone => $"World Stone: {RespawnPointName}",
                RespawnPointType.Checkpoint => $"Checkpoint: {RespawnPointName}",
                RespawnPointType.ZoneTransition => GetFormattedZoneTransition(),
                _ => null
            };
        }
    }

    private string GetFormattedZoneTransition()
    {
        var split = RespawnPointName.Split('/');
        return $"Transition between {split[0]} and {split[1]}";
    }


    public Location ShallowCopy()
    {
        return (Location)MemberwiseClone();
    }
}
