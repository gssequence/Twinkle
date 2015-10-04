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
    /// TweetControl.xaml の相互作用ロジック
    /// </summary>
    public partial class TweetControl : UserControl
    {
        public TweetControl()
        {
            InitializeComponent();
        }

        public TweetViewModel Tweet
        {
            get { return (TweetViewModel)GetValue(TweetProperty); }
            set { SetValue(TweetProperty, value); }
        }

        public static readonly DependencyProperty TweetProperty =
            DependencyProperty.Register("Tweet", typeof(TweetViewModel), typeof(TweetControl), new UIPropertyMetadata());
    }
}
