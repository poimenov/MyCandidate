using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MyCandidate.MVVM.Converters;

public class RectGeometryConverter : IMultiValueConverter
{
    public static readonly RectGeometryConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count == 2 && values[0] is Rect borderBounds && values[1] is Rect headerBounds)
        {
            return new CombinedGeometry(GeometryCombineMode.Exclude, new RectangleGeometry(borderBounds), new RectangleGeometry(headerBounds));
        }
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }
}