using StatefulModel.EventListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinkle.Models;
using Twinkle.Mvvm;

namespace Twinkle.ViewModels
{
    public class AccountViewModel : BindableViewModel<Account>
    {
        public bool IsActive
        {
            get { return Model.IsActive; }
            set { Model.IsActive = value; }
        }

        public bool LoadHomeTimelineWhenStart
        {
            get { return Model.LoadHomeTimelineWhenStart; }
            set { Model.LoadHomeTimelineWhenStart = value; }
        }

        public bool LoadMentionsWhenStart
        {
            get { return Model.LoadMentionsWhenStart; }
            set { Model.LoadMentionsWhenStart = value; }
        }

        public bool LoadMyTweetsWhenStart
        {
            get { return Model.LoadMyTweetsWhenStart; }
            set { Model.LoadMyTweetsWhenStart = value; }
        }

        public bool LoadFavoritesWhenStart
        {
            get { return Model.LoadFavoritesWhenStart; }
            set { Model.LoadFavoritesWhenStart = value; }
        }

        public bool ConnectToStreamWhenStart
        {
            get { return Model.ConnectToStreamWhenStart; }
            set { Model.ConnectToStreamWhenStart = value; }
        }

        public string UserName { get { return Model.UserInfo == null ? null : Model.UserInfo.Name; } }

        public string UserScreenNameWithAtSign { get { return Model.UserInfo == null ? null : "@" + Model.UserInfo.ScreenName; } }

        public Uri UserProfileImageUrl { get { return new Uri(Model.UserInfo.ProfileImageUrl); } }

        public bool IsConnectedToStream { get { return Model.IsConnectedToStream; } }

        public AccountViewModel(Account model) : base(model, false)
        {
            CompositeDisposable.Add(new PropertyChangedEventListener(Model, (sender, e) =>
            {
                if (e.PropertyName == "UserInfo")
                {
                    RaisePropertyChanged(() => UserName);
                    RaisePropertyChanged(() => UserScreenNameWithAtSign);
                    RaisePropertyChanged(() => UserProfileImageUrl);
                }
                else
                    RaisePropertyChanged(e.PropertyName);
            }));
        }

        public void DeleteFromAccounts()
        {
            Model.DeleteFromAccounts();
        }

        public void GetLatestTweets()
        {
            Model.GetLatestTweets();
        }

        public void GetMyTweets()
        {
            Model.GetMyTweets();
        }

        public void GetMentions()
        {
            Model.GetMentions();
        }

        public void GetFavorites()
        {
            Model.GetFavorites();
        }

        public void ConnectToStream()
        {
            Model.ConnectToStream();
        }

        public void DisconnectFromStream()
        {
            Model.DisconnectFromStream();
        }

        public void Update(string status, Action<bool> completion)
        {
            Model.Update(status, completion);
        }
    }
}
