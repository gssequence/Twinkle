using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using Twinkle.Models;
using Twinkle.Mvvm;

namespace Twinkle.ViewModels
{
    public class MainWindowViewModel : BindableViewModel
    {
        public ApplicationMessageServiceViewModel ApplicationMessageService { get; private set; }

        public SettingsViewModel SettingsViewModel { get; private set; }

        public MainWindowViewModel()
        {
            ApplicationMessageService = new ApplicationMessageServiceViewModel();
            CompositeDisposable.Add(ApplicationMessageService);

            SettingsViewModel = new SettingsViewModel();
            CompositeDisposable.Add(SettingsViewModel);
        }

        public void Initialize()
        {
            // HACK: WTF
            foreach (var tl in SettingsViewModel.Timelines.Where(t => t.IsWindowed))
                tl.ShowWindow();
        }

        public void OpenOptionsWindow()
        {
            var msg = new TransitionMessage("OpenOptionsWindowMessageKey");
            Messenger.Raise(msg);
        }

        public void OpenMessageWindow()
        {
            var msg = new TransitionMessage("OpenMessageWindowMessageKey");
            Messenger.Raise(msg);
        }

        public void OpenLoginWindow()
        {
            var msg = new TransitionMessage("OpenLoginWindowMessageKey");
            Messenger.Raise(msg);
        }

        public void OpenComposeWindow()
        {
            if (Settings.Current.Accounts.Count == 0) return;
            var msg = new TransitionMessage("OpenComposeWindowMessageKey");
            Messenger.Raise(msg);
        }

        public void OpenAboutWindow()
        {
            var msg = new TransitionMessage("OpenAboutWindowMessageKey");
            Messenger.Raise(msg);
        }

        public void AddTimeline()
        {
            Settings.Current.Timelines.Add(new Timeline() { Name = "(Untitled)", Script = "false" });
            Settings.Current.Save();
        }
    }
}
