using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;

namespace MyCandidate.MVVM.Converters;

public class PathToFileNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Path.GetFileName((string)value);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }
}
