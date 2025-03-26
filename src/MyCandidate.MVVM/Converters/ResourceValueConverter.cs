using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;

namespace MyCandidate.MVVM.Converters;

public class ResourceValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return null;
            
        var sValue = value.ToString();
        if(File.Exists(sValue))
        {
            return Path.GetFileName(sValue);
        }
        else if(Uri.IsWellFormedUriString(sValue, UriKind.Absolute))
        {
            return new Uri(sValue).Host;
        }
        
        return sValue;
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }
}
