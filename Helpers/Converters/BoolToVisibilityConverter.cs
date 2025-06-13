using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sound2Light.Helpers.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; } = false;
        public bool CollapseInsteadOfHide { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = value is bool b && b;

            if (Invert)
                flag = !flag;

            if (flag)
                return Visibility.Visible;

            return CollapseInsteadOfHide ? Visibility.Collapsed : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
            {
                if (Invert)
                    return v != Visibility.Visible;
                return v == Visibility.Visible;
            }
            return false;
        }
    }
}
