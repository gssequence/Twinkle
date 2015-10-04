using StatefulModel;
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
    /// TweetList.xaml の相互作用ロジック
    /// </summary>
    public partial class TweetList : UserControl
    {
        public TweetList()
        {
            InitializeComponent();
        }

        public ReadOnlyNotifyChangedCollection<TweetViewModel> Tweets
        {
            get { return (ReadOnlyNotifyChangedCollection<TweetViewModel>)GetValue(TweetsProperty); }
            set { SetValue(TweetsProperty, value); }
        }

        public static readonly DependencyProperty TweetsProperty =
            DependencyProperty.Register("Tweets", typeof(ReadOnlyNotifyChangedCollection<TweetViewModel>), typeof(TweetList), new UIPropertyMetadata());


        
    }
}
