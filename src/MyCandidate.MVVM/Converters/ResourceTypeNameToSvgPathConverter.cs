using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Converters;

public class ResourceTypeNameToSvgPathConverter : IValueConverter
{
    const string basePath = "avares://MyCandidate.MVVM/Assets";
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var path = string.Empty;
        
        switch ((string)value)
        {
            case ResourceTypeNames.Mobile:
                path = "mobile-phone-app-svgrepo-com.svg";
                break;
            case ResourceTypeNames.Email:
                path = "mail-part-2-svgrepo-com.svg";
                break;
            case ResourceTypeNames.Url:
                path = "url-internet-svgrepo-com.svg";
                break;
            case ResourceTypeNames.Skype:
                path = "skype-svgrepo-com.svg";
                break;
            default://"Path"
                path = "fine-print-svgrepo-com.svg";
                break;
        }
        return Path.Combine(basePath, path);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var resourceTypeName = string.Empty;
        var fileName = Path.GetFileName((string)value);
        switch (fileName)
        {
            case "mobile-phone-app-svgrepo-com.svg":
                resourceTypeName = ResourceTypeNames.Mobile;
                break;
            case "mail-part-2-svgrepo-com.svg":
                resourceTypeName = ResourceTypeNames.Email;
                break;
            case "url-internet-svgrepo-com.svg":
                resourceTypeName = ResourceTypeNames.Url;
                break;
            case "skype-svgrepo-com.svg":
                resourceTypeName = ResourceTypeNames.Skype;
                break;
            default://"fine-print-svgrepo-com.svg"
                resourceTypeName = ResourceTypeNames.Path;
                break;
        }

        return resourceTypeName;
    }
}
