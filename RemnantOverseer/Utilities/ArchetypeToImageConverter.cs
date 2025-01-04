using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using RemnantOverseer.Models.Enums;
using System;
using System.Globalization;

namespace RemnantOverseer.Utilities;
public class ArchetypeToImageConverter: IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || value.GetType() != typeof(Archetypes))
        {
            throw new ArgumentException("Unsupported");
        }
        else
        {
            return new Bitmap(AssetLoader.Open(new Uri($"avares://RemnantOverseer/Assets/Images/Archetypes/T_UI_Icon_Archetype_{value}.png")));
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
