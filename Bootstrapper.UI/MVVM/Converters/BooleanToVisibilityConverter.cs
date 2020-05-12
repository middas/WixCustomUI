using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Bootstrapper.UI.MVVM.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool HiddenInsteadOfCollapsed { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility result = Visibility.Visible;

            if (value is bool b)
            {
                result = b ? Visibility.Visible : HiddenInsteadOfCollapsed ? Visibility.Hidden : Visibility.Collapsed;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;

            if (value is Visibility v)
            {
                result = v == Visibility.Visible;
            }

            return result;
        }
    }
}