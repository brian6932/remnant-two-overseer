using RemnantOverseer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemnantOverseer.Models;
public class Item
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ItemTypes Type { get; set; }
    public string Description { get; set; }
    public OriginTypes OriginType { get; set; }
    public string OriginName { get; set; }
    public bool IsDuplicate { get; set; }
    public bool IsLooted { get; set; }
    public bool IsCoop { get; set; }

    // We are only interested in a couple of types to display
    public string? OriginNameFormatted
    {
        get
        {
            return string.IsNullOrEmpty(OriginName) ? null :
            OriginType switch
            {
                // Extra spaces are a temporary workaround to https://github.com/AvaloniaUI/Avalonia/issues/17862, remove when fixed
                // It's not fixed yet, but I moved this text out of the tooltip. Removing spaces, keeping comment
                OriginTypes.Injectable or OriginTypes.Dungeon or OriginTypes.Vendor => $"{OriginType}: {OriginName}",
                _ => null
            };
        }
    }

    public Item ShallowCopy()
    {
        return (Item)MemberwiseClone();
    }
}
