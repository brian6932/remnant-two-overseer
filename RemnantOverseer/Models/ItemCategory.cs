using RemnantOverseer.Models.Enums;
using System.Collections.Generic;

namespace RemnantOverseer.Models;
public class ItemCategory
{
    public ItemTypes Type { get; set; }
    public List<Item> Items { get; set; } = [];

    public string Name
    {
        get
        {
            return Type switch
            {
                ItemTypes.Unknown => "Unknown",
                ItemTypes.Amulet => "Amulets",
                ItemTypes.Armor => "Armors",
                ItemTypes.Concoction => "Concoctions",
                ItemTypes.Consumable => "Consumables",
                ItemTypes.Dream => "Dreams",
                ItemTypes.Engram => "Engrams",
                ItemTypes.Mod => "Mods",
                ItemTypes.Mutator => "Mutators",
                ItemTypes.QuestItem => "Quest Items",
                ItemTypes.Relic => "Relics",
                ItemTypes.Ring => "Rings",
                ItemTypes.Trait => "Traits",
                ItemTypes.Weapon => "Weapons",
                ItemTypes.Prism => "Prisms",
                _ => throw new System.NotImplementedException(),
            };
        }
    }

    public ItemCategory ShallowCopy()
    {
        return (ItemCategory)MemberwiseClone();
    }
}

