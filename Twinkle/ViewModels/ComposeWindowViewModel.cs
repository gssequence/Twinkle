using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using StatefulModel;
using StatefulModel.EventListeners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Twinkle.Models;
using Twinkle.Mvvm;

namespace Twinkle.ViewModels
{
    public class ComposeWindowViewModel : BindableViewModel
    {
        public SettingsViewModel SettingsViewModel { get; private set; }

        private string _text = "";
        public string Text
        {
            get { return _text; }
            set
            {
                if (SetProperty(ref _text, value))
                {
                    RaisePropertyChanged(() => CharCountLeft);
                    TweetCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableSynchronizedCollection<UploadMediaInfoViewModel> Media { get; private set; }

        public bool IsMediaAttached { get { return Media.Count > 0 && CanEdit; } }

        public bool CanMediaAttach { get { return Media.Count < 4 && CanEdit; } }

        public bool CanMediaAttachFromClipboard { get { return CanMediaAttach && Clipboard.ContainsImage(); } }

        private bool _canEdit = true;
        public bool CanEdit
        {
            get { return _canEdit; }
            set
            {
                if (SetProperty(ref _canEdit, value))
                {
                    RaisePropertyChanged(() => CanMediaAttach);
                    RaisePropertyChanged(() => CanMediaAttachFromClipboard);
                }
            }
        }

        public int CharCountLeft
        {
            get
            {
                var source = Text.Replace("\r\n", "\n");
                int len;
                if (TwitterConfigurationService.Instance.Configurations == null) len = source.Length;
                else len = Constants.UrlRegex.Replace(source, "*".Repeat(TwitterConfigurationService.Instance.Configurations.ShortUrlLengthHttps)).Length;
                if (IsMediaAttached)
                {
                    if (TwitterConfigurationService.Instance.Configurations == null) len += 24;
                    else len += TwitterConfigurationService.Instance.Configurations.CharactersReservedPerMedia;
                }
                return 140 - len;
            }
        }

        private long _inReplyToStatusId = 0;
        public string InReplyToStatusText { get; private set; }

        private bool _inReplyToStatus = false;
        public bool InReplyToStatus
        {
            get { return _inReplyToStatus; }
            set { SetProperty(ref _inReplyToStatus, value); }
        }

        private int _selectionStart = 0;
        public int SelectionStart
        {
            get { return _selectionStart; }
            set { SetProperty(ref _selectionStart, value); }
        }

        public ComposeWindowViewModel()
        {
            SettingsViewModel = new SettingsViewModel();
            CompositeDisposable.Add(SettingsViewModel);

            Media = new ObservableSynchronizedCollection<UploadMediaInfoViewModel>();
            CompositeDisposable.Add(new CollectionChangedEventListener(Media, (sender, e) =>
            {
                RaisePropertyChanged(() => IsMediaAttached);
                RaisePropertyChanged(() => CharCountLeft);
                TweetCommand.RaiseCanExecuteChanged();
            }));
        }

        public ComposeWindowViewModel(Tweet inReplyTo) : this()
        {
            var screenNames = new[] { inReplyTo.OriginalStatus.User.ScreenName }.AsEnumerable();
            if (inReplyTo.OriginalStatus.Entities.UserMentions != null)
            {
                screenNames = screenNames.Concat(inReplyTo.OriginalStatus.Entities.UserMentions.Select(e => e.ScreenName)).Distinct();
                if (inReplyTo.Status.User.ScreenName != inReplyTo.Account.UserInfo.ScreenName)
                    screenNames = screenNames.Where(s => s != inReplyTo.Account.UserInfo.ScreenName);
            }
            Text = screenNames.Select(s => "@" + s + " ").Aggregate((l, r) => l + r);
            SelectionStart = Text.Length;
            _inReplyToStatusId = inReplyTo.OriginalStatus.Id;
            InReplyToStatusText = "@" + inReplyTo.OriginalStatus.User.ScreenName + ": " + inReplyTo.OriginalStatus.Text;
            InReplyToStatus = true;
        }

        #region TweetCommand
        private ViewModelCommand _TweetCommand;

        public ViewModelCommand TweetCommand
        {
            get
            {
                if (_TweetCommand == null)
                {
                    _TweetCommand = new ViewModelCommand(Tweet, CanTweet);
                }
                return _TweetCommand;
            }
        }

        public bool CanTweet()
        {
            return Text.Length > 0 || Media.Count > 0;
        }

        public void Tweet()
        {
            if (!CanTweet()) return;
            if (!CanEdit) return;
            CanEdit = false;

            var status = Text;
            long? inReplyToStatusId = InReplyToStatus ? ((long?)_inReplyToStatusId) : null;
            var accounts = Settings.Current.Accounts.Where(a => a.IsActive);
            var count = accounts.Count();
            int t = 0, f = 0;
            Action<bool> comp = b =>
            {
                if (b) t++;
                else f++;
                if (t + f == count)
                {
                    if (t == count)
                    {
                        var msg = new InteractionMessage("CloseWindowKey");
                        Messenger.Raise(msg);
                    }
                    CanEdit = true;
                }
            };
            if (count == 0)
                return;
            foreach (var item in accounts)
            {
                if (Media.Count > 0)
                {
                    item.UpdateWithMedia(status, Media.Select(x => x.GetBytes()), comp, InReplyToStatus ? ((long?)_inReplyToStatusId) : null);
                }
                else
                {
                    item.Update(status, comp, InReplyToStatus ? ((long?)_inReplyToStatusId) : null);
                }
            }
        }

        #endregion

        public void DeleteInReplyToStatus()
        {
            InReplyToStatus = false;
        }

        public void OpenFileDialog()
        {
            Messenger.Raise(new InteractionMessage("OpenFileDialogMessageKey"));
        }

        public void OpenFileDialogCallback(OpeningFileSelectionMessage msg)
        {
            if (msg.Response == null || msg.Response.Count() == 0) return;
            foreach (var item in msg.Response)
            {
                if (Media.Count >= 4) return;
                try
                {
                    Media.Add(new UploadMediaInfoViewModel(Media, item));
                }
                catch (Exception ex)
                {
                    ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex));
                }
            }
        }

        public void AttachMediaFromClipboard()
        {
            if (Clipboard.ContainsImage())
            {
                if (Media.Count >= 4) return;
                try
                {
                    var enc = new PngBitmapEncoder();
                    var ms = new MemoryStream();
                    enc.Frames.Add(BitmapFrame.Create(Clipboard.GetImage()));
                    enc.Save(ms);
                    ms.Close();
                    Media.Add(new UploadMediaInfoViewModel(Media, ms.GetBuffer()));
                }
                catch (Exception ex)
                {
                    ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex));
                }
            }
        }

        public void RaiseCanMediaAttachFromClipboardPropertyChanged()
        {
            RaisePropertyChanged(() => CanMediaAttachFromClipboard);
        }

        public void LaunchSnippingTool()
        {
            try
            {
                Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "SnippingTool.exe"));
            }
            catch (Exception ex)
            {
                ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex));
            }
        }
    }
}
