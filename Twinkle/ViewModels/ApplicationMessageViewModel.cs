using StatefulModel.EventListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Twinkle.Models;
using Twinkle.Mvvm;

namespace Twinkle.ViewModels
{
    public class ApplicationMessageViewModel : BindableViewModel<ApplicationMessage>
    {
        public DateTime DateTime { get { return Model.DateTime; } }

        public MessageType Type { get { return (MessageType)Model.Type; } }

        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case MessageType.Info:
                        return "INFO";
                    case MessageType.Warning:
                        return "WARNING";
                    case MessageType.Error:
                        return "ERROR";
                    case MessageType.Deleted:
                        return "DELETED";
                    case MessageType.Favorited:
                        return "FAVORITED";
                    case MessageType.Unfavorited:
                        return "UNFAVORITED";
                    default:
                        return "";
                }
            }
        }

        public Brush BackgroundBrush
        {
            get
            {
                switch (Type)
                {
                    case MessageType.Info:
                        return new SolidColorBrush(new Color() { R = 17, G = 158, B = 218, A = 204 });
                    case MessageType.Warning:
                        return Brushes.Orange;
                    case MessageType.Error:
                        return Brushes.Red;
                    case MessageType.Deleted:
                        return Brushes.Gray;
                    case MessageType.Favorited:
                        return Brushes.Orange;
                    case MessageType.Unfavorited:
                        return Brushes.Black;
                    default:
                        return Brushes.Black;
                }
            }
        }

        public Brush ForegroundBrush
        {
            get
            {
                switch (Type)
                {
                    case MessageType.Unfavorited:
                        return Brushes.Orange;
                    default:
                        return Brushes.White;
                }
            }
        }

        public string Title { get { return Model.Title; } }

        public string Description { get { return Model.Description; } }

        public ApplicationMessageViewModel(ApplicationMessage model) : base(model)
        {

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
