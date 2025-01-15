using RemnantOverseer.Models.Enums;

namespace RemnantOverseer.Models;
public class Item
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ItemTypes Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public OriginTypes OriginType { get; set; }
    public string OriginName { get; set; } = string.Empty;
    public bool IsDuplicate { get; set; }
    public bool IsLooted { get; set; }
    public bool IsPrerequisiteMissing { get; set; }
    public bool IsCoop { get; set; }
    public bool IsAccountAward { get; set; }

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

    public string? WikiLink => $"https://remnant.wiki/{Name}";

    public Item ShallowCopy()
    {
        return (Item)MemberwiseClone();
    }
}
