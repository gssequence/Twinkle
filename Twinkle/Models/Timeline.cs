using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Twinkle.Mvvm;

namespace Twinkle.Models
{
    [DataContract]
    public class Timeline : BindableModel
    {
        [DataMember(Name = "Name")]
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        [DataMember(Name = "Script")]
        private string _script;
        public string Script
        {
            get { return _script; }
            set
            {
                if (SetProperty(ref _script, value))
                    evaluator.Compile(value);
            }
        }

        [DataMember(Name = "IsWindowed")]
        private bool _isWindowed = false;
        public bool IsWindowed
        {
            get { return _isWindowed; }
            set
            {
                if (SetProperty(ref _isWindowed, value))
                    Settings.Current.Save();
            }
        }
        
        private Evaluator _evaluator;
        private Evaluator evaluator
        {
            get
            {
                if (_evaluator == null)
                {
                    _evaluator = new Evaluator();
                    _evaluator.Compile(Script);
                }
                return _evaluator;
            }
        }

        private SortedObservableCollection<Tweet, long> _tweets;
        public SortedObservableCollection<Tweet, long> Tweets
        {
            get { return _tweets ?? (_tweets = new SortedObservableCollection<Tweet, long>(t => t.Status.Id, true)); }
        }

        private object _syncRoot;

        public Timeline()
        {

        }

        public void AddOrDelete(Tweet tweet)
        {
            if (_syncRoot == null) _syncRoot = new object();

            lock (_syncRoot)
            {
                if (evaluator.Eval(tweet))
                {
                    if (!Tweets.Any(t => t.Status.Id == tweet.Status.Id && t.Account == tweet.Account))
                        Tweets.Add(tweet);
                }
                else
                {
                    var d = Tweets.FirstOrDefault(t => t.Status.Id == tweet.Status.Id && t.Account == tweet.Account);
                    if (d != null)
                        Tweets.Remove(d);
                }
            }
        }

        public void Refresh()
        {
            Tweets.Clear();
            foreach (var tweet in TweetDataSource.Instance.Tweets)
                AddOrDelete(tweet);
        }

        public void DeleteFromTimelines()
        {
            Settings.Current.Timelines.Remove(this);
            Settings.Current.Save();
        }

        public ReadOnlyNotifyChangedCollection<T> CreateReadOnlyNotifyChangedCollection<T>(Func<Tweet, T> selector, SynchronizationContext context)
        {
            return Tweets.ToSyncedSynchronizationContextCollection(selector, context).ToSyncedReadOnlyNotifyChangedCollection();
        }
    }
}
