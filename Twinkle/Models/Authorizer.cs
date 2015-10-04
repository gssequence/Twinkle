using CoreTweet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinkle.Models
{
    public class Authorizer
    {
        private OAuth.OAuthSession _lastSession;

        public bool IsCompleted { get; private set; }

        public Authorizer()
        {
            IsCompleted = false;
        }

        public async Task StartAuthentication()
        {
            await StartAuthentication(Constants.DefaultConsumerKey, Constants.DefaultConsumerSecret);
        }

        public async Task StartAuthentication(string consumerKey, string consumerSecret)
        {
            try
            {
                ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageStartAuthentication", null));
                _lastSession = await OAuth.AuthorizeAsync(consumerKey, consumerSecret);
                Process.Start(_lastSession.AuthorizeUri.AbsoluteUri);
                ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageEnterPinCode", ""));
            }
            catch (Exception ex)
            {
                ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex));
            }
        }

        public async Task ConfirmPinCode(string pin)
        {
            if (_lastSession == null)
            {
                ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Error, "MessagePressStartAuthentication", ""));
                return;
            }

            try
            {
                ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageVerifyingPinCode", ""));
                var tokens = await _lastSession.GetTokensAsync(pin);
                if (Settings.Current.Accounts.Any(a => a.ConsumerKey == tokens.ConsumerKey && a.ConsumerSecret == tokens.ConsumerSecret && a.AccessToken == tokens.AccessToken && a.AccessTokenSecret == tokens.AccessTokenSecret))
                {
                    ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Error, "MessageAlreadyRegistered", ""));
                    return;
                }
                var account = new Account(tokens);
                Settings.Current.AddAccount(account);
                IsCompleted = true;
                ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageAccountRegeistered", ""));
            }
            catch (Exception ex)
            {
                ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex));
            }
        }
    }
}
