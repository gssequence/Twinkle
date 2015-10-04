using Livet;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Twinkle.Models;
using Twinkle.Mvvm;

namespace Twinkle.ViewModels
{
    public class SettingsViewModel : BindableViewModel<Settings>
    {
        public string Locale
        {
            get { return Model.Locale; }
            set { Model.Locale = value; }
        }

        public int TimelineWidth
        {
            get { return Model.TimelineWidth; }
            set { Model.TimelineWidth = value; }
        }

        public int TextSize
        {
            get { return Model.TextSize; }
            set { Model.TextSize = value; }
        }

        public bool CustomImageSaveFolder
        {
            get { return Model.CustomImageSaveFolder; }
            set { Model.CustomImageSaveFolder = value; }
        }

        public string CustomImageSaveFolderPath
        {
            get { return Model.CustomImageSaveFolderPath; }
            set { Model.CustomImageSaveFolderPath = value; }
        }

        public bool CustomTweetSaveFolder
        {
            get { return Model.CustomTweetSaveFolder; }
            set { Model.CustomTweetSaveFolder = value; }
        }

        public string CustomTweetSaveFolderPath
        {
            get { return Model.CustomTweetSaveFolderPath; }
            set { Model.CustomTweetSaveFolderPath = value; }
        }

        public bool SaveTweetWhenDelete
        {
            get { return Model.SaveTweetWhenDelete; }
            set { Model.SaveTweetWhenDelete = value; }
        }

        public ReadOnlyNotifyChangedCollection<AccountViewModel> Accounts { get; private set; }

        public ReadOnlyNotifyChangedCollection<TimelineViewModel> Timelines { get; private set; }

        public SettingsViewModel() : base(Settings.Current)
        {
            Accounts = Model.Accounts.ToSyncedSynchronizationContextCollection(a => new AccountViewModel(a), new DispatcherSynchronizationContext(DispatcherHelper.UIDispatcher)).ToSyncedReadOnlyNotifyChangedCollection();
            CompositeDisposable.Add(Accounts);

            Timelines = Model.Timelines.ToSyncedSynchronizationContextCollection(t => new TimelineViewModel(t), new DispatcherSynchronizationContext(DispatcherHelper.UIDispatcher)).ToSyncedReadOnlyNotifyChangedCollection();
            CompositeDisposable.Add(Timelines);
        }
    }
}
