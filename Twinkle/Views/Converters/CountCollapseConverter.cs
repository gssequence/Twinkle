using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Twinkle.Views.Converters
{
    public class CountCollapseConverter : IValueConverter
    {
        public CountCollapseConverter()
        {
            IsInversed = false;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int && ((int)value) == 0)
                return IsInversed ? Visibility.Collapsed : Visibility.Visible;
            return IsInversed ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public bool IsInversed { get; set; }
    }
}
