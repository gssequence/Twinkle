using CoreTweet;
using CoreTweet.Core;
using CoreTweet.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Twinkle.Mvvm;

namespace Twinkle.Models
{
    [DataContract]
    public class Account : BindableModel
    {
        private Tokens _tokens;

        [DataMember(Name = "ConsumerKey")]
        private string _consumerKey;
        public string ConsumerKey { get { return _consumerKey; } }

        [DataMember(Name = "ConsumerSecret")]
        private string _consumerSecret;
        public string ConsumerSecret { get { return _consumerSecret; } }

        [DataMember(Name = "AccessToken")]
        private string _accessToken;
        public string AccessToken { get { return _accessToken; } }

        [DataMember(Name = "AccessTokenSecret")]
        private string _accessTokenSecret;
        public string AccessTokenSecret { get { return _accessTokenSecret; } }

        [DataMember(Name = "IsActive")]
        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        [DataMember(Name = "LoadHomeTimelineWhenStart")]
        private bool _loadHomeTimelineWhenStart = true;
        public bool LoadHomeTimelineWhenStart
        {
            get { return _loadHomeTimelineWhenStart; }
            set { SetProperty(ref _loadHomeTimelineWhenStart, value); }
        }

        [DataMember(Name = "LoadMentionsWhenStart")]
        private bool _loadMentionsWhenStart = true;
        public bool LoadMentionsWhenStart
        {
            get { return _loadMentionsWhenStart; }
            set { SetProperty(ref _loadMentionsWhenStart, value); }
        }

        [DataMember(Name = "LoadMyTweetsWhenStart")]
        private bool _loadMyTweetsWhenStart = false;
        public bool LoadMyTweetsWhenStart
        {
            get { return _loadMyTweetsWhenStart; }
            set { SetProperty(ref _loadMyTweetsWhenStart, value); }
        }

        [DataMember(Name = "LoadFavoritesWhenStart")]
        private bool _loadFavoritesWhenStart = false;
        public bool LoadFavoritesWhenStart
        {
            get { return _loadFavoritesWhenStart; }
            set { SetProperty(ref _loadFavoritesWhenStart, value); }
        }

        [DataMember(Name = "ConnectToStreamWhenStart")]
        private bool _connectToStreamWhenStart = true;
        public bool ConnectToStreamWhenStart
        {
            get { return _connectToStreamWhenStart; }
            set { SetProperty(ref _connectToStreamWhenStart, value); }
        }

        private UserResponse _userInfo;
        public UserResponse UserInfo
        {
            get { return _userInfo; }
            set { SetProperty(ref _userInfo, value); }
        }

        private bool _isConnectedToStream = false;
        public bool IsConnectedToStream
        {
            get { return _isConnectedToStream; }
            set { SetProperty(ref _isConnectedToStream, value); }
        }

        public Account(Tokens tokens) : this(tokens.ConsumerKey, tokens.ConsumerSecret, tokens.AccessToken, tokens.AccessTokenSecret) { }

        public Account(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _accessToken = accessToken;
            _accessTokenSecret = accessTokenSecret;
            start();
        }

        public void DeleteFromAccounts()
        {
            Settings.Current.RemoveAccount(this);
        }

        [OnDeserialized]
        private void start(StreamingContext context = new StreamingContext())
        {
            _tokens = Tokens.Create(_consumerKey, _consumerSecret, _accessToken, _accessTokenSecret);
            _tokens.Account.VerifyCredentialsAsync().ToObservable().Subscribe(
                u => UserInfo = u,
                ex => ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex)),
                () =>
                {
                    _tokens.Help.ConfigurationAsync().ToObservable().Subscribe(c => TwitterConfigurationService.Instance.Configurations = c);
                    if (LoadHomeTimelineWhenStart) GetLatestTweets();
                    if (LoadMyTweetsWhenStart) GetMyTweets();
                    if (LoadMentionsWhenStart) GetMentions();
                    if (LoadFavoritesWhenStart) GetFavorites();
                    if (ConnectToStreamWhenStart) ConnectToStream();
                });
        }

        private IDisposable _stream = null;

        public void ConnectToStream()
        {
            DisconnectFromStream();

            _stream = Observable.Defer(() => _tokens.Streaming.UserAsObservable()).Retry().Subscribe(msg =>
            {
                switch (msg.Type)
                {
                    case MessageType.Create:
                        {
                            TweetDataSource.Instance.Add(new Tweet(((StatusMessage)msg).Status, this));
                            break;
                        }
                    case MessageType.DeleteStatus:
                        {
                            var id = ((DeleteMessage)msg).Id;
                            var tweet = TweetDataSource.Instance.Tweets.FirstOrDefault(t => t.Status.Id == id && t.Account == this);
                            if (tweet != null)
                            {
                                if (Settings.Current.SaveTweetWhenDelete) tweet.Save();
                                tweet.IsDeleted = true;
                                ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateDeleteMessage(tweet));
                            }
                            break;
                        }
                    case MessageType.Event:
                        {
                            var eventmsg = (EventMessage)msg;
                            switch (eventmsg.Event)
                            {
                                case EventCode.Favorite:
                                    if (UserInfo == null || eventmsg.Source.ScreenName != UserInfo.ScreenName)
                                        ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Favorited, "MessageFavorited", "Format", new[] { eventmsg.Source.ScreenName }, new[] { eventmsg.TargetStatus.FullText }));
                                    break;
                                case EventCode.Unfavorite:
                                    if (UserInfo == null || eventmsg.Source.ScreenName != UserInfo.ScreenName)
                                        ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Unfavorited, "MessageUnfavorited", "Format", new[] { eventmsg.Source.ScreenName }, new[] { eventmsg.TargetStatus.FullText }));
                                    break;
                            }
                            TweetDataSource.Instance.Add(new Tweet(eventmsg.TargetStatus, this));
                            break;
                        }
                    default:
                        System.Diagnostics.Debug.WriteLine(msg);
                        System.Diagnostics.Debug.WriteLine(msg.Json);
                        break;
                }
            });

            IsConnectedToStream = true;
        }

        public void DisconnectFromStream()
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }

            IsConnectedToStream = false;
        }

        public void GetLatestTweets()
        {
            int c = 200;
            addFromObservable(_tokens.Statuses.HomeTimelineAsync(count: c, include_entities: true, tweet_mode: TweetMode.extended).ToObservable(),
                () => ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageCompleteHomeReceiving", "", new object[] { c })));
        }

        public void GetMyTweets()
        {
            int c = 50;
            addFromObservable(_tokens.Statuses.UserTimelineAsync(count: c, include_rts: true, tweet_mode: TweetMode.extended).ToObservable(),
                () => ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageCompleteMyTweetReceiving", "", new object[] { c })));
        }

        public void GetMentions()
        {
            int c = 50;
            addFromObservable(_tokens.Statuses.MentionsTimelineAsync(count: c, include_entities: true, tweet_mode: TweetMode.extended).ToObservable(),
                () => ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageCompleteMentionsReceiving", "", new object[] { c })));
        }

        public void GetFavorites()
        {
            int c = 200;
            addFromObservable(_tokens.Favorites.ListAsync(count: c, include_entities: true, tweet_mode: TweetMode.extended).ToObservable(),
                () => ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageCompleteFavoriteReceiving", "", new object[] { c })));
        }

        private void addFromObservable(IObservable<ListedResponse<Status>> observable, Action onCompleted)
        {
            observable
                .SelectMany(r => r)
                .Select(s => new Tweet(s, this))
                .Subscribe(t => TweetDataSource.Instance.Add(t),
                ex => ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex)),
                onCompleted);
        }

        public void Update(string status, Action<bool> completion, long? inReplyToStatusId = null, IEnumerable<long> media_ids = null)
        {
            ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageSendingTweet", ""));
            _tokens.Statuses.UpdateAsync(status, inReplyToStatusId, media_ids: media_ids).ToObservable()
                .Subscribe(_ => { ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageSentTweet", "")); completion(true); },
                (Exception ex) => { ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex)); completion(false); });
        }

        public void UpdateWithMedia(string status, IEnumerable<IEnumerable<byte>> data, Action<bool> completion, long? inReplyToStatusId = null)
        {
            int count = data.Count();
            if (count == 0)
            {
                Update(status, completion, inReplyToStatusId);
            }
            else
            {
                data.Select((d, i) => Observable.Defer(() =>
                {
                    ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageUploading", "", new[] { (i + 1).ToString(), count.ToString() }));
                    return _tokens.Media.UploadAsync(d).ToObservable();
                })) .Aggregate((s, o) => s.Concat(o))
                    .ToArray()
                    .Subscribe(results => Update(status, completion, inReplyToStatusId, results.Select(x => x.MediaId)),
                    (Exception ex) => { ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex)); completion(false); });
            }
        }

        public void Favorite(long id)
        {
            _tokens.Favorites.CreateAsync(id, true).ToObservable()
                .Select(s => new Tweet(s, this))
                .Subscribe(t => TweetDataSource.Instance.Add(t),
                ex => ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex)));
        }

        public void Unfavorite(long id)
        {
            _tokens.Favorites.DestroyAsync(id, true).ToObservable()
                .Select(s => new Tweet(s, this))
                .Subscribe(t => TweetDataSource.Instance.Add(t),
                ex => ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex)));
        }

        public void Retweet(long id)
        {
            _tokens.Statuses.RetweetAsync(id).ToObservable()
                .Select(s => new Tweet(s, this))
                .Subscribe(t => { TweetDataSource.Instance.Add(t); Show(id); },
                ex => ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex)));
        }

        public void Show(long id)
        {
            _tokens.Statuses.ShowAsync(id, include_my_retweet: true, include_entities: true).ToObservable()
                .Select(s => new Tweet(s, this))
                .Subscribe(t => TweetDataSource.Instance.Add(t),
                ex => ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex)));
        }

        public static bool operator ==(Account l, Account r)
        {
            if (object.ReferenceEquals(l, null))
                return object.ReferenceEquals(r, null);
            return l.Equals(r);
        }

        public static bool operator !=(Account l, Account r)
        {
            return !(l == r);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            var r = (Account)obj;
            return this.ConsumerKey == r.ConsumerKey &&
                this.ConsumerSecret == r.ConsumerSecret &&
                this.AccessToken == r.AccessToken &&
                this.AccessTokenSecret == r.AccessTokenSecret;
        }

        public override int GetHashCode()
        {
            return ConsumerKey.GetHashCode() ^ ConsumerSecret.GetHashCode() ^ AccessToken.GetHashCode() ^ AccessTokenSecret.GetHashCode();
        }

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var ret = base.SetProperty<T>(ref storage, value, propertyName);
            if (ret) Settings.Current.Save();
            return ret;
        }
    }
}
