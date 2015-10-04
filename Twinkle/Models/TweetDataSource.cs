using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Twinkle.Models
{
    public class TweetDataSource
    {
        private static TweetDataSource _instance = new TweetDataSource();
        public static TweetDataSource Instance { get { return _instance; } }

        public ReadOnlyNotifyChangedCollection<Tweet> Tweets { get; private set; }

        private SortedObservableCollection<Tweet, long> _tweetsInternal;

        private TweetDataSource()
        {
            _tweetsInternal = new SortedObservableCollection<Tweet, long>(t => t.Status.Id, true);
            Tweets = _tweetsInternal.ToSyncedReadOnlyNotifyChangedCollection();
        }

        private object _syncRoot = new object();

        public void Add(Tweet tweet)
        {
            lock (_syncRoot)
            {
                var exist = _tweetsInternal.FirstOrDefault(t => t.Status.Id == tweet.Status.Id && t.Account == tweet.Account);
                if (exist == null)
                {
                    _tweetsInternal.Add(tweet);
                }
                else
                    exist.Status = tweet.Status;

                foreach (var timeline in Settings.Current.Timelines)
                    timeline.AddOrDelete(tweet);
            }
        }

        public void AddRange(IEnumerable<Tweet> tweets)
        {
            foreach (var tweet in tweets)
                Add(tweet);
        }

        public ReadOnlyNotifyChangedCollection<T> CreateReadOnlyNotifyChangedCollection<T>(Func<Tweet, T> selector, SynchronizationContext context)
        {
            return _tweetsInternal.ToSyncedSynchronizationContextCollection(selector, context).ToSyncedReadOnlyNotifyChangedCollection();
        }
    }
}
