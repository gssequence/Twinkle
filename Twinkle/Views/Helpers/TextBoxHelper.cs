using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Twinkle.Views.Helpers
{
    public static class TextBoxHelper
    {
        public static int GetSelectionStart(DependencyObject obj)
        {
            return (int)obj.GetValue(SelectionStartProperty);
        }

        public static void SetSelectionStart(DependencyObject obj, int value)
        {
            obj.SetValue(SelectionStartProperty, value);
        }

        public static readonly DependencyProperty SelectionStartProperty =
            DependencyProperty.RegisterAttached("SelectionStart", typeof(int), typeof(TextBoxHelper), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectionStartChanged));

        private static void SelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = d as TextBox;
            if (tb != null)
            {
                if (e.OldValue == null && e.NewValue != null)
                    tb.SelectionChanged += tb_SelectionChanged;
                else if (e.OldValue != null && e.NewValue == null)
                    tb.SelectionChanged -= tb_SelectionChanged;

                if (e.NewValue is int)
                    tb.SelectionStart = (int)e.NewValue;
            }
        }

        static void tb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
                SetSelectionStart(tb, tb.SelectionStart);
        }
    }
}
