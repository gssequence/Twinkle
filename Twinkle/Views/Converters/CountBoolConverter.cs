using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Twinkle.Views.Converters
{
    public class CountBoolConverter : IValueConverter
    {
        public CountBoolConverter()
        {
            IsInversed = false;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int && ((int)value) == 0)
                return false ^ IsInversed;
            return true ^ IsInversed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public bool IsInversed { get; set; }
    }
}
