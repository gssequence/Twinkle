using StatefulModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Twinkle.Mvvm;

namespace Twinkle.Models
{
    [DataContract]
    [KnownType(typeof(Account))]
    public class Settings : BindableModel
    {
        private static Settings _current = initialize();

        private static Settings initialize()
        {
            if (File.Exists(Constants.SettingsPath))
                return load();
            else
            {
                var s = new Settings();
                s.Save();
                return s;
            }
        }

        public static Settings Current { get { return _current; } }

        private Settings()
        {
            Timelines.Add(new Timeline() { Name = "All", Script = "true", IsWindowed = false });
        }

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var ret = base.SetProperty(ref storage, value, propertyName);
            if (ret) Save();
            return ret;
        }

        private static Settings load()
        {
            var serializer = new DataContractSerializer(typeof(Settings));
            using (var reader = XmlReader.Create(Constants.SettingsPath))
                return (Settings)serializer.ReadObject(reader);
        }

        private object _syncRoot;

        public void Save()
        {
            if (_syncRoot == null) _syncRoot = new object();
            lock (_syncRoot)
            {
                var serializer = new DataContractSerializer(typeof(Settings));
                using (var writer = XmlWriter.Create(Constants.SettingsPath, new XmlWriterSettings() { Encoding = new UTF8Encoding(false) }))
                    serializer.WriteObject(writer, this);
            }
        }

        [DataMember(Name = "Locale")]
        private string _locale = "";
        public string Locale
        {
            get { return _locale; }
            set { SetProperty(ref _locale, value); }
        }

        [DataMember(Name = "TimelineWidth")]
        private int _timelineWidth = 400;
        public int TimelineWidth
        {
            get { return _timelineWidth; }
            set { SetProperty(ref _timelineWidth, value); }
        }

        [DataMember(Name = "TextSize")]
        private int _textSize = 12;
        public int TextSize
        {
            get { return _textSize; }
            set { SetProperty(ref _textSize, value); }
        }

        [DataMember(Name = "CustomImageSaveFolder")]
        private bool _customImageSaveFolder = false;
        public bool CustomImageSaveFolder
        {
            get { return _customImageSaveFolder; }
            set { SetProperty(ref _customImageSaveFolder, value); }
        }

        [DataMember(Name = "CustomImageSaveFolderPath")]
        private string _customImageSaveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        public string CustomImageSaveFolderPath
        {
            get { return _customImageSaveFolderPath; }
            set { SetProperty(ref _customImageSaveFolderPath, value); }
        }

        [DataMember(Name = "CustomTweetSaveFolder")]
        private bool _customTweetSaveFolder = false;
        public bool CustomTweetSaveFolder
        {
            get { return _customTweetSaveFolder; }
            set { SetProperty(ref _customTweetSaveFolder, value); }
        }

        [DataMember(Name = "CustomTweetSaveFolderPath")]
        private string _customTweetSaveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public string CustomTweetSaveFolderPath
        {
            get { return _customTweetSaveFolderPath; }
            set { SetProperty(ref _customTweetSaveFolderPath, value); }
        }

        [DataMember(Name = "SaveTweetWhenDelete")]
        private bool _saveTweetWhenDelete = false;
        public bool SaveTweetWhenDelete
        {
            get { return _saveTweetWhenDelete; }
            set { SetProperty(ref _saveTweetWhenDelete, value); }
        }

        [DataMember(Name = "Accounts")]
        private ObservableSynchronizedCollection<Account> _accounts = new ObservableSynchronizedCollection<Account>();
        public ObservableSynchronizedCollection<Account> Accounts
        {
            get { return _accounts; }
        }

        public void AddAccount(Account account)
        {
            _accounts.Add(account);
            Save();
        }

        public void RemoveAccount(Account account)
        {
            _accounts.Remove(account);
            Save();
        }

        [DataMember(Name = "Timelines")]
        private ObservableSynchronizedCollection<Timeline> _timelines = new ObservableSynchronizedCollection<Timeline>();
        public ObservableSynchronizedCollection<Timeline> Timelines
        {
            get { return _timelines; }
        }
    }
}
