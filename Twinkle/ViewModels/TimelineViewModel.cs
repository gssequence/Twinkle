using Livet;
using Livet.Messaging;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Twinkle.Models;
using Twinkle.Mvvm;

namespace Twinkle.ViewModels
{
    public class TimelineViewModel : BindableViewModel<Timeline>
    {
        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; }
        }

        public bool IsWindowed
        {
            get { return Model.IsWindowed; }
            set { Model.IsWindowed = value; }
        }

        public ReadOnlyNotifyChangedCollection<TweetViewModel> Tweets { get; private set; }

        private bool _isEditMode = false;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); }
        }

        private string _nameInput = "";
        public string NameInput
        {
            get { return _nameInput; }
            set { SetProperty(ref _nameInput, value); }
        }

        private Evaluator _evaluator = new Evaluator();
        private Subject<string> _scriptChanged = new Subject<string>();

        private string _scriptInput = "";
        public string ScriptInput
        {
            get { return _scriptInput; }
            set { if (SetProperty(ref _scriptInput, value)) _scriptChanged.OnNext(value); }
        }

        private bool _scriptError = false;
        public bool ScriptError
        {
            get { return _scriptError; }
            set { SetProperty(ref _scriptError, value); }
        }

        public FilterScriptTemplateServiceViewModel FilterScriptTemplateServiceViewModel { get; private set; }

        public TimelineViewModel(Timeline model) : base(model)
        {
            Tweets = Model.CreateReadOnlyNotifyChangedCollection(t => new TweetViewModel(t), new DispatcherSynchronizationContext(DispatcherHelper.UIDispatcher));
            CompositeDisposable.Add(Tweets);

            var disposable = _scriptChanged.Throttle(TimeSpan.FromSeconds(1)).Select(s => _evaluator.Compile(s)).Subscribe(b => ScriptError = !b);
            CompositeDisposable.Add(disposable);

            FilterScriptTemplateServiceViewModel = new FilterScriptTemplateServiceViewModel(this);
        }

        public void EnterEditMode()
        {
            NameInput = Model.Name;
            ScriptInput = Model.Script;
            ScriptError = false;
            IsEditMode = true;
        }

        public void ExitEditMode()
        {
            IsEditMode = false;
        }

        public void ExitEditModeWithSave()
        {
            Model.Name = NameInput;
            Model.Script = ScriptInput;
            Model.Refresh();
            Settings.Current.Save();
            ExitEditMode();
        }

        public void DeleteFromTimelines()
        {
            Messenger.Raise(new InteractionMessage("CloseMessageKey"));
            Model.DeleteFromTimelines();
            this.Dispose();
        }

        public void Detach()
        {
            if (IsWindowed) return;
            IsWindowed = true;
            ShowWindow();
        }

        public void Attach()
        {
            if (!IsWindowed) return;
            IsWindowed = false;
            Messenger.Raise(new InteractionMessage("CloseMessageKey"));
        }

        public void ShowWindow()
        {
            Messenger.RaiseAsync(new InteractionMessage("DetachMessageKey"));
        }

        public void WindowClosing()
        {
            IsWindowed = false;
        }
    }
}
