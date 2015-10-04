using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Twinkle.Views.UserControls
{
    public class DropDownButton : Button
    {
        public ContextMenu DropDownMenu
        {
            get { return (ContextMenu)GetValue(DropDownMenuProperty); }
            set { SetValue(DropDownMenuProperty, value); }
        }

        public static readonly DependencyProperty DropDownMenuProperty =
            DependencyProperty.Register("DropDownMenu", typeof(ContextMenu), typeof(DropDownButton), new UIPropertyMetadata(null));

        protected override void OnClick()
        {
            base.OnClick();
            
            if (DropDownMenu == null) return;
            DropDownMenu.PlacementTarget = this;
            DropDownMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            DropDownMenu.IsOpen = !DropDownMenu.IsOpen;
        }
    }
}
