using System;

namespace RemnantOverseer.Models.Enums;
public enum WeaponSubtypes
{
    LongGun,
    HandGun,
    MeleeWeapon
}

public static class WeaponSubtypesExtensions
{
    public static WeaponSubtypes? FromFriendlyString(this string subtype)
    {
        return subtype switch
        {
            "Long Gun" => WeaponSubtypes.LongGun,
            "Hand Gun" => WeaponSubtypes.HandGun,
            "Melee Weapon" => WeaponSubtypes.MeleeWeapon,
            _ => null
        };
    }

    public static string ToFriendlyString(this WeaponSubtypes subtype)
    {
        return subtype switch
        {
            WeaponSubtypes.LongGun => "Long Gun",
            WeaponSubtypes.HandGun => "Hand Gun",
            WeaponSubtypes.MeleeWeapon => "Melee Weapon",
            _ => throw new NotImplementedException()
        };
    }
}