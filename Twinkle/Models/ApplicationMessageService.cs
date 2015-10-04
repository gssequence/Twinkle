using StatefulModel;
using StatefulModel.EventListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinkle.Mvvm;

namespace Twinkle.Models
{
    public class ApplicationMessageService : BindableModel
    {
        private static volatile ApplicationMessageService _instance = new ApplicationMessageService();
        public static ApplicationMessageService Instance { get { return _instance; } }

        public ObservableSynchronizedCollection<ApplicationMessage> Messages { get; private set; }

        private ApplicationMessage _latestMessage;
        public ApplicationMessage LatestMessage
        {
            get { return _latestMessage; }
            private set { SetProperty(ref _latestMessage, value); }
        }

        private ApplicationMessageService()
        {
            Messages = new ObservableSynchronizedCollection<ApplicationMessage>();
            new CollectionChangedEventListener(Messages, (sender, e) =>
            {
                var latest = Messages.LastOrDefault();
                if (latest != LatestMessage) LatestMessage = latest;
            });

            Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageApplicationStart", null));
            if (Settings.Current.Accounts.Count() == 0)
                Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Warning, "MessageNoAccounts", "MessageNoAccountsDetail"));
        }
    }
}
