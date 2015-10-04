using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Twinkle.Mvvm
{
    public class TimerViewModel<T> : BindableViewModel<T> where T : INotifyPropertyChanged
    {
        private static readonly IConnectableObservable<long> _timer;

        static TimerViewModel()
        {
            _timer = Observable.Interval(TimeSpan.FromSeconds(1)).Publish();
            _timer.Connect();
        }

        public TimerViewModel(T model, bool createDefaultEventListener = true) : base(model, createDefaultEventListener)
        {
            var disposable = _timer.Subscribe(_ => Tick());
            CompositeDisposable.Add(disposable);
        }

        protected virtual void Tick()
        {

        }
    }
}
