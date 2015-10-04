using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Twinkle.Views.UserControls
{
    public class WebImage : Image
    {
        public WebImage()
        {

        }

        public Uri SourceUri
        {
            get { return (Uri)GetValue(SourceUriProperty); }
            set { SetValue(SourceUriProperty, value); }
        }

        public static readonly DependencyProperty SourceUriProperty =
            DependencyProperty.Register("SourceUri", typeof(Uri), typeof(WebImage), new PropertyMetadata(null, sourceUriPropertyChanged));

        private static void sourceUriPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue) return;
            var instance = sender as WebImage;
            if (instance == null) return;
            instance.update();
        }

        private Uri _prev;

        private void update()
        {
            if (SourceUri == _prev) return;
            _prev = SourceUri;
            this.Source = null;
            if (SourceUri == null) return;

            var img = new BitmapImage(SourceUri) { CacheOption = BitmapCacheOption.OnLoad };
            try
            {
                this.Source = img;
            }
            catch { }
        }
    }
}
