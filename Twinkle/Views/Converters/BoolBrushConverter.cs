using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Twinkle.Views.Converters
{
    public class BoolBrushConverter : FrameworkElement, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                return ((bool)value) ? BrushWhenTrue : BrushWhenFalse;
            throw new InvalidOperationException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public Brush BrushWhenFalse
        {
            get { return (Brush)GetValue(BrushWhenFalseProperty); }
            set { SetValue(BrushWhenFalseProperty, value); }
        }

        public static readonly DependencyProperty BrushWhenFalseProperty =
            DependencyProperty.Register("BrushWhenFalse", typeof(Brush), typeof(BoolBrushConverter), new PropertyMetadata(null));

        public Brush BrushWhenTrue
        {
            get { return (Brush)GetValue(BrushWhenTrueProperty); }
            set { SetValue(BrushWhenTrueProperty, value); }
        }

        public static readonly DependencyProperty BrushWhenTrueProperty =
            DependencyProperty.Register("BrushWhenTrue", typeof(Brush), typeof(BoolBrushConverter), new PropertyMetadata(null));
    }
}
