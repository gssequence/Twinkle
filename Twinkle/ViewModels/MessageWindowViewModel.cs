using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinkle.Mvvm;

namespace Twinkle.ViewModels
{
    public class MessageWindowViewModel : BindableViewModel
    {
        public ApplicationMessageServiceViewModel ApplicationMessageServiceViewModel { get; private set; }

        public MessageWindowViewModel()
        {
            ApplicationMessageServiceViewModel = new ApplicationMessageServiceViewModel();
            CompositeDisposable.Add(ApplicationMessageServiceViewModel);
        }
    }
}
