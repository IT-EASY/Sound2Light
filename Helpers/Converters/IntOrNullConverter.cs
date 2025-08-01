﻿#nullable disable
using System;
using System.Globalization;
using System.Windows.Data;

namespace Sound2Light.Helpers.Converters
{
    public class IntOrNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value as string;
            if (string.IsNullOrWhiteSpace(input))
                return null;

            return int.TryParse(input, out var result) ? result : null;
        }
    }
}
