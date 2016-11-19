using StatefulModel.EventListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinkle.Mvvm;
using Twinkle.Properties;

namespace Twinkle.Models
{
    public class ApplicationMessage : BindableModel
    {
        public DateTime DateTime { get; private set; }

        public MessageType Type { get; private set; }

        private string _titleKey;
        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_titleKey)) return "";
                var str = GlobalizationService.Instance.GetString(_titleKey);
                if (_titleArgs == null) return GlobalizationService.Instance.GetString(_titleKey);
                return string.Format(GlobalizationService.Instance.GetString(_titleKey), _titleArgs);
            }
        }

        private string _descriptionKey;
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_descriptionKey)) return "";
                var str = GlobalizationService.Instance.GetString(_descriptionKey);
                if (_descriptionArgs == null) return GlobalizationService.Instance.GetString(_descriptionKey);
                return string.Format(GlobalizationService.Instance.GetString(_descriptionKey), _descriptionArgs);
            }
        }

        private object[] _titleArgs;
        private object[] _descriptionArgs;

        public ApplicationMessage(MessageType type, string titleKey, string descriptionKey, object[] titleArgs = null, object[] descriptionArgs = null)
        {
            DateTime = DateTime.Now;
            Type = type;
            _titleKey = titleKey;
            _descriptionKey = descriptionKey;
            _titleArgs = titleArgs;
            _descriptionArgs = descriptionArgs;

            new PropertyChangedEventListener(GlobalizationService.Instance, (sender, e) =>
            {
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => Description);
            });
        }

        public static ApplicationMessage CreateExceptionMessage(Exception ex)
        {
            return new ApplicationMessage(MessageType.Error, "Format", "Format", new[] { ex.Message }, new[] { ex.StackTrace });
        }

        public static ApplicationMessage CreateDeleteMessage(Tweet tweet)
        {
            return new ApplicationMessage(MessageType.Deleted, "MessageDeletedTweet", "Format", new[] { tweet.Status.User.ScreenName }, new[] { tweet.Status.FullText });
        }

        public enum MessageType
        {
            Info,
            Warning,
            Error,
            Deleted,
            Favorited,
            Unfavorited
        }
    }
}
