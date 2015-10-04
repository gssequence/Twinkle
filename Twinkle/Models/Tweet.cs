using CoreTweet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twinkle.Mvvm;

namespace Twinkle.Models
{
    public class Tweet : BindableModel
    {
        private static Regex _clientNameRegex = new Regex("<a.*?>(?<name>.*?)</a>");

        private Status _status;
        public Status Status
        {
            get { return _status; }
            set
            {
                if (SetProperty(ref _status, value))
                {
                    refreshTimelines();
                    RaisePropertyChanged(() => OriginalStatus);
                    RaisePropertyChanged(() => RetweetedUser);
                    RaisePropertyChanged(() => IsRetweet);
                }
            }
        }

        public Status OriginalStatus
        {
            get
            {
                return Status.RetweetedStatus ?? Status;
            }
        }

        public User RetweetedUser
        {
            get
            {
                return IsRetweet ? Status.User : null;
            }
        }

        public bool IsRetweet
        {
            get { return Status.RetweetedStatus != null; }
        }

        public bool IsFavorited
        {
            get { return Status.IsFavorited ?? false; }
        }

        public bool IsRetweeted
        {
            get { return Status.IsRetweeted ?? false; }
        }

        public bool CanRetweet
        {
            get
            {
                if (OriginalStatus.User.IsProtected) return false;
                if (Account.UserInfo.Id == OriginalStatus.User.Id) return false;
                if (IsRetweeted) return false;
                return true;
            }
        }

        public Account Account { get; private set; }

        public string OriginalClientName
        {
            get
            {
                var source = OriginalStatus.Source;
                var match = _clientNameRegex.Match(source);
                if (match.Success)
                    return match.Groups["name"].Value;
                else
                    return source;
            }
        }

        public string RetweeterClientName
        {
            get
            {
                var source = Status.Source;
                var match = _clientNameRegex.Match(source);
                if (match.Success)
                    return match.Groups["name"].Value;
                else
                    return source;
            }
        }

        public IEnumerable<MediaInfo> MediaInfo
        {
            get
            {
                if (OriginalStatus.Entities.Media != null)
                {
                    foreach (var item in OriginalStatus.ExtendedEntities.Media)
                    {
                        yield return new MediaInfo(new Uri(item.MediaUrlHttps + ":thumb"), new Uri(item.MediaUrlHttps + ":orig"));
                    }
                }
            }
        }

        public string StatusUrl
        {
            get { return "https://twitter.com/" + OriginalStatus.User.ScreenName + "/status/" + OriginalStatus.Id.ToString(); }
        }

        private bool _isDeleted = false;
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                if (SetProperty(ref _isDeleted, value))
                    refreshTimelines();
            }
        }

        public string DefaultTweetFileName
        {
            get
            {
                return Status.User.ScreenName + "-" + Status.Id.ToString() + ".txt";
            }
        }

        public Tweet(Status status, Account account)
        {
            _status = status;
            Account = account;
        }

        private void refreshTimelines()
        {
            TweetDataSource.Instance.Add(this);
        }

        public void Favorite()
        {
            Account.Favorite(this.Status.Id);
        }

        public void Unfavorite()
        {
            Account.Unfavorite(this.Status.Id);
        }

        public void Retweet()
        {
            Account.Retweet(this.Status.Id);
        }

        public void Save()
        {
            Save(Path.Combine(Settings.Current.CustomTweetSaveFolderPath, DefaultTweetFileName));
        }

        public void Save(string path)
        {
            var text = formattedString();
            var ob = Observable.Using(() => new StreamWriter(path, false, new UTF8Encoding(false)),
                sw => sw.WriteAsync(text).ToObservable());
            ob.Subscribe(u => ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageTweetSaved", "Format", null, new[] { OriginalStatus.Text })),
                ex => ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex)));
        }

        private string formattedString()
        {
            IEnumerable<UrlEntity> entities = OriginalStatus.Entities.Urls;
            if (OriginalStatus.Entities.Media != null)
                entities = entities.Concat(OriginalStatus.ExtendedEntities.Media);

            var text = OriginalStatus.User.Name + " @" + OriginalStatus.User.ScreenName + " " + OriginalStatus.User.Id.ToString() + Environment.NewLine;
            text += Environment.NewLine;
            text += OriginalStatus.Text + Environment.NewLine;
            text += Environment.NewLine;
            if (entities.Count() > 0)
            {
                text += "Entities:" + Environment.NewLine;
                foreach (var entity in entities)
                {
                    text += entity.Url + " => ";
                    if (entity is MediaEntity)
                        text += (entity as MediaEntity).MediaUrlHttps + ":orig";
                    else
                        text += entity.ExpandedUrl;
                    text += Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            text += (OriginalStatus.RetweetCount ?? 0) + " Retweets, " + (OriginalStatus.FavoriteCount ?? 0) + " Favs" + Environment.NewLine;
            text += "via " + OriginalClientName + Environment.NewLine;
            text += "Created at " + OriginalStatus.CreatedAt.UtcDateTime.ToString("R") + Environment.NewLine;
            text += "Saved at " + DateTime.UtcNow.ToString("R") + Environment.NewLine;
            if (IsRetweet)
            {
                text += Environment.NewLine;
                text += "Retweeted by " + Status.User.Name + " @" + Status.User.ScreenName + " " + Status.User.Id.ToString() + Environment.NewLine;
                text += "via " + RetweeterClientName + Environment.NewLine;
                text += "Retweeted at " + Status.CreatedAt.UtcDateTime.ToString("R") + Environment.NewLine;
            }

            return text;
        }
    }
}
