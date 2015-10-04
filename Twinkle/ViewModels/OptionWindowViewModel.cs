using Livet.Messaging;
using Livet.Messaging.IO;
using StatefulModel.EventListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinkle.Mvvm;

namespace Twinkle.ViewModels
{
    public class OptionWindowViewModel : BindableViewModel
    {
        public SettingsViewModel SettingsViewModel { get; private set; }

        public GlobalizationServiceViewModel GlobalizationServiceViewModel { get; private set; }

        public bool IsEnabledTweetSaveFolderSelector { get { return SettingsViewModel.CustomTweetSaveFolder || SettingsViewModel.SaveTweetWhenDelete; } }

        public OptionWindowViewModel()
        {
            SettingsViewModel = new SettingsViewModel();
            CompositeDisposable.Add(SettingsViewModel);
            CompositeDisposable.Add(new PropertyChangedEventListener(SettingsViewModel, (sender, e) =>
            {
                if (e.PropertyName == "CustomTweetSaveFolder" || e.PropertyName == "SaveTweetWhenDelete")
                    RaisePropertyChanged(() => IsEnabledTweetSaveFolderSelector);
            }));

            GlobalizationServiceViewModel = new GlobalizationServiceViewModel();
            CompositeDisposable.Add(GlobalizationServiceViewModel);
        }

        public void OpenLoginWindow()
        {
            var msg = new TransitionMessage("OpenLoginWindowMessageKey");
            Messenger.Raise(msg);
        }

        public void CustomImageSaveFolderOpen(FolderSelectionMessage msg)
        {
            if (msg.Response == null) return;
            SettingsViewModel.CustomImageSaveFolderPath = msg.Response;
        }

        public void CustomTweetSaveFolderOpen(FolderSelectionMessage msg)
        {
            if (msg.Response == null) return;
            SettingsViewModel.CustomTweetSaveFolderPath = msg.Response;
        }
    }
}
