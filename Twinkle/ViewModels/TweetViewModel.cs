using CoreTweet;
using Livet.Messaging;
using Livet.Messaging.IO;
using StatefulModel.EventListeners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Twinkle.Models;
using Twinkle.Mvvm;

namespace Twinkle.ViewModels
{
    public class TweetViewModel : TimerViewModel<Tweet>
    {
        public Entities Entities { get { return Model.OriginalStatus.Entities; } }

        public Uri UserProfileImageUrl { get { return new Uri(Model.OriginalStatus.User.ProfileImageUrl); } }

        public string UserScreenName { get { return Model.OriginalStatus.User.ScreenName; } }

        public string UserName { get { return Model.OriginalStatus.User.Name.Unescape(); } }

        public bool IsProtectedUser { get { return Model.OriginalStatus.User.IsProtected; } }

        public string Text { get { return Model.OriginalStatus.Text; } }

        public string Time
        {
            get
            {
                var t = DateTimeOffset.Now - Model.OriginalStatus.CreatedAt;
                if (t.TotalSeconds < 1)
                    return GlobalizationService.Instance.GetString("TimeNow");
                else if (t.TotalSeconds < 60)
                    return string.Format(GlobalizationService.Instance.GetString("TimeSecond"), t.Seconds);
                else if (t.TotalMinutes < 60)
                    return string.Format(GlobalizationService.Instance.GetString("TimeMinute"), t.Minutes);
                else if (t.TotalHours < 24)
                    return string.Format(GlobalizationService.Instance.GetString("TimeHour"), t.Hours);
                else
                    return string.Format(GlobalizationService.Instance.GetString("TimeDay"), t.Days);
            }
        }

        public string ClientName { get { return Model.OriginalClientName; } }

        public int FavoriteCount { get { return Model.OriginalStatus.FavoriteCount ?? 0; } }

        public int RetweetCount { get { return Model.OriginalStatus.RetweetCount ?? 0; } }

        public string RetweeterUserProfileImageUrl { get { return (Model.RetweetedUser == null) ? null : Model.RetweetedUser.ProfileImageUrl; } }

        public string RetweeterUserScreenName { get { return (Model.RetweetedUser == null) ? null : Model.RetweetedUser.ScreenName; } }

        public string RetweeterUserName { get { return (Model.RetweetedUser == null) ? null : Model.RetweetedUser.Name.Unescape(); } }

        public bool RetweeterIsProtectedUser { get { return (Model.RetweetedUser == null) ? false : Model.RetweetedUser.IsProtected; } }

        public string RetweeterClientName { get { return Model.RetweeterClientName; } }

        public bool MediaExists { get { return Model.MediaInfo.Count() > 0; } }

        public IEnumerable<MediaInfoViewModel> MediaInfo { get { return Model.MediaInfo.Select(m => new MediaInfoViewModel(m)); } }

        public bool IsRetweet { get { return Model.IsRetweet; } }

        public bool IsFavorited { get { return Model.IsFavorited; } }

        public bool IsRetweeted { get { return Model.IsRetweeted; } }

        public bool CanRetweet { get { return Model.CanRetweet; } }

        public bool IsDeleted { get { return Model.IsDeleted; } }

        public Brush BackgroundBrush { get { return IsDeleted ? Brushes.LightGray : Brushes.White; } }

        public string DefaultTweetFileName { get { return Model.DefaultTweetFileName; } }

        public TweetViewModel(Tweet model) : base(model)
        {
            CompositeDisposable.Add(new PropertyChangedEventListener(model, (sender, e) =>
            {
                if (e.PropertyName == "IsDeleted")
                    RaisePropertyChanged(() => BackgroundBrush);
                else if (e.PropertyName == "Status")
                {
                    RaisePropertyChanged(() => UserProfileImageUrl);
                    RaisePropertyChanged(() => UserScreenName);
                    RaisePropertyChanged(() => UserName);
                    RaisePropertyChanged(() => IsProtectedUser);
                    RaisePropertyChanged(() => Text);
                    RaisePropertyChanged(() => FavoriteCount);
                    RaisePropertyChanged(() => RetweetCount);
                    RaisePropertyChanged(() => RetweeterUserProfileImageUrl);
                    RaisePropertyChanged(() => RetweeterUserScreenName);
                    RaisePropertyChanged(() => RetweeterUserName);
                    RaisePropertyChanged(() => RetweeterIsProtectedUser);
                    RaisePropertyChanged(() => RetweeterClientName);
                    RaisePropertyChanged(() => IsRetweet);
                    RaisePropertyChanged(() => IsFavorited);
                    RaisePropertyChanged(() => IsRetweeted);
                    RaisePropertyChanged(() => CanRetweet);
                }
            }));

            CompositeDisposable.Add(new PropertyChangedEventListener(GlobalizationService.Instance, (sender, e) =>
            {
                RaisePropertyChanged(() => Time);
            }));
        }

        protected override void Tick()
        {
            base.Tick();
            RaisePropertyChanged(() => Time);
        }

        public void Reply()
        {
            if (Settings.Current.Accounts.Count == 0) return;
            var msg = new TransitionMessage(new ComposeWindowViewModel(Model), "OpenComposeWindowMessageKey");
            Messenger.Raise(msg);
        }

        public void Favorite()
        {
            if (!IsFavorited)
                Model.Favorite();
            else
                Model.Unfavorite();
        }

        public void Retweet()
        {
            Model.Retweet();
        }

        public void Save()
        {
            if (Settings.Current.CustomTweetSaveFolder)
            {
                Model.Save();
            }
            else
            {
                var msg = new SavingFileSelectionMessage("SaveDialogMessageKey");
                Messenger.Raise(msg);
            }
        }

        public void SaveFileDialogCallback(SavingFileSelectionMessage msg)
        {
            if (msg.Response == null || msg.Response.Count() == 0) return;
            Model.Save(msg.Response.First());
        }

        public void OpenInBrowser()
        {
            Process.Start(Model.StatusUrl);
        }
    }
}
