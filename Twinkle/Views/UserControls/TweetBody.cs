using CoreTweet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Twinkle.Models;
using Twinkle.ViewModels;

namespace Twinkle.Views.UserControls
{
    public class TweetBody : TextBlock
    {
        public Tweet Tweet
        {
            get { return (Tweet)GetValue(TweetProperty); }
            set { SetValue(TweetProperty, value); }
        }

        public static readonly DependencyProperty TweetProperty =
            DependencyProperty.Register("Tweet", typeof(TweetViewModel), typeof(TweetBody), new PropertyMetadata(null, TweetPropertyChanged));

        private static void TweetPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue) return;
            var instance = sender as TextBlock;
            if (instance == null) return;
            var tweet = e.NewValue as TweetViewModel;
            if (tweet == null) return;

            var text = tweet.Text;
            IEnumerable<Entity> entities = tweet.Entities.Urls;
            if (tweet.Entities.UserMentions != null)
                entities = entities.Concat(tweet.Entities.UserMentions);
            if (tweet.Entities.Media != null)
                entities = entities.Concat(tweet.Entities.Media);
            if (tweet.Entities.HashTags != null)
                entities = entities.Concat(tweet.Entities.HashTags);
            entities = entities.OrderByDescending(en => en.Indices[0]);

            instance.Inlines.Clear();
            List<Inline> content = new List<Inline>();

            foreach (var item in entities)
            {
                // うしろを切り取る
                var tail = text.Substring(min(item.Indices[1], text.Length));
                text = text.Substring(0, min(item.Indices[1], text.Length));
                if (!string.IsNullOrEmpty(tail))
                    content.Insert(0, new Run(tail.Unescape()));

                // リンクを作る
                var body = text.Substring(item.Indices[0]);
                text = text.Substring(0, item.Indices[0]);
                var link = new Hyperlink();
                link.Foreground = Brushes.CornflowerBlue;
                if (item is UrlEntity)
                {
                    var entity = item as UrlEntity;
                    link.Inlines.Add(new Run(entity.DisplayUrl.Unescape()));
                    link.Click += (_, __) => Process.Start(entity.Url);
                }
                else if (item is UserMentionEntity)
                {
                    var entity = item as UserMentionEntity;
                    link.Inlines.Add(new Run(body.Unescape()));
                }
                else if (item is MediaEntity)
                {
                    var entity = item as MediaEntity;
                    link.Inlines.Add(new Run(entity.DisplayUrl.Unescape()));
                    link.Click += (_, __) => Process.Start(entity.Url);
                }
                else if (item is HashtagEntity)
                {
                    var entity = item as HashtagEntity;
                    link.Inlines.Add(new Run(body.Unescape()));
                }
                content.Insert(0, link);
            }

            // 前を切り取る
            if (!string.IsNullOrEmpty(text))
                content.Insert(0, new Run(text.Unescape()));

            instance.Inlines.AddRange(content);
        }

        private static int min(int a, int b)
        {
            return a < b ? a : b;
        }
    }
}
