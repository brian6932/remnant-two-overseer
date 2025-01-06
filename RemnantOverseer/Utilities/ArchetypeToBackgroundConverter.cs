using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using RemnantOverseer.Models.Enums;
using System;
using System.Globalization;

namespace RemnantOverseer.Utilities;
public class ArchetypeToBackgroundConverter: IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || value.GetType() != typeof(Archetypes))
        {
            return AvaloniaProperty.UnsetValue;
        }
        else
        {
            return new SolidColorBrush(Color.Parse(ArchetypeColors.Map[(Archetypes)value]));
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
