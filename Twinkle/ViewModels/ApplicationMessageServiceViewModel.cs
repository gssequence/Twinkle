using Livet;
using StatefulModel;
using StatefulModel.EventListeners;
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
    public class ApplicationMessageServiceViewModel : BindableViewModel<ApplicationMessageService>
    {
        public ReadOnlyNotifyChangedCollection<ApplicationMessageViewModel> Messages { get; private set; }

        public ApplicationMessageViewModel LatestMessage { get { return new ApplicationMessageViewModel(Model.LatestMessage); } }

        public ApplicationMessageServiceViewModel() : base(ApplicationMessageService.Instance)
        {
            Messages = Model.Messages.ToSyncedSynchronizationContextCollection(m => new ApplicationMessageViewModel(m), new DispatcherSynchronizationContext(DispatcherHelper.UIDispatcher)).ToSyncedReadOnlyNotifyChangedCollection();
            CompositeDisposable.Add(Messages);
        }
    }
}
