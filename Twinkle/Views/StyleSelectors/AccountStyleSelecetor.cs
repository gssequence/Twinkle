using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Twinkle.ViewModels;

namespace Twinkle.Views.StyleSelectors
{
    public class AccountStyleSelecetor : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is AccountViewModel)
            {
                var f = (FrameworkElement)container;
                return (Style)f.FindResource("accountStyle");
            }
            return base.SelectStyle(item, container);
        }
    }
}
