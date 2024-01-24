using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;

namespace MyCandidate.MVVM.Converters;

public class CandidateResourceExtValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var sValue = (string)value;
        if(File.Exists(sValue))
        {
            return Path.GetFileName(sValue);
        }
        else if(Uri.IsWellFormedUriString(sValue, UriKind.Absolute))
        {
            return new Uri((string)value).Host;
        }
        
        return sValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }
}
