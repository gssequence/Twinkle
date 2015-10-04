using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Twinkle.ViewModels;

namespace Twinkle.Views.UserControls
{
    /// <summary>
    /// TimelineControl.xaml の相互作用ロジック
    /// </summary>
    public partial class TimelineControl : UserControl
    {
        public TimelineControl()
        {
            InitializeComponent();
        }

        public TimelineViewModel Timeline
        {
            get { return (TimelineViewModel)GetValue(TimelineProperty); }
            set { SetValue(TimelineProperty, value); }
        }

        public static readonly DependencyProperty TimelineProperty =
            DependencyProperty.Register("Timeline", typeof(TimelineViewModel), typeof(TimelineControl), new PropertyMetadata());
    }
}
