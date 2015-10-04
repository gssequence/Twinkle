using Livet.Commands;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinkle.Mvvm;

namespace Twinkle.ViewModels
{
    public class LoginWindowViewModel : BindableViewModel
    {
        private bool _useCustomKeys = false;
        public bool UseCustomKeys
        {
            get { return _useCustomKeys; }
            set
            {
                if (SetProperty(ref _useCustomKeys, value))
                    StartAuthCommand.RaiseCanExecuteChanged();
            }
        }

        private string _consumerKey = "";
        public string ConsumerKey
        {
            get { return _consumerKey; }
            set
            {
                if (SetProperty(ref _consumerKey, value))
                    StartAuthCommand.RaiseCanExecuteChanged();
            }
        }

        private string _consumerSecret = "";
        public string ConsumerSecret
        {
            get { return _consumerSecret; }
            set
            {
                if (SetProperty(ref _consumerSecret, value))
                    StartAuthCommand.RaiseCanExecuteChanged();
            }
        }

        private string _pinCode = "";
        public string PinCode
        {
            get { return _pinCode; }
            set
            {
                if (SetProperty(ref _pinCode, value))
                    ConfirmCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isEnabledStep1 = true;
        public bool IsEnabledStep1
        {
            get { return _isEnabledStep1; }
            set { SetProperty(ref _isEnabledStep1, value); }
        }

        private bool _isEnabledStep2 = true;
        public bool IsEnabledStep2
        {
            get { return _isEnabledStep2; }
            set { SetProperty(ref _isEnabledStep2, value); }
        }

        public AuthorizerViewModel AuthorizerViewModel { get; private set; }
        public ApplicationMessageServiceViewModel ApplicationMessageService { get; private set; }

        public LoginWindowViewModel()
        {
            AuthorizerViewModel = new AuthorizerViewModel();
            ApplicationMessageService = new ApplicationMessageServiceViewModel();
            CompositeDisposable.Add(ApplicationMessageService);
        }

        #region StartAuthCommand
        private ViewModelCommand _StartAuthCommand;

        public ViewModelCommand StartAuthCommand
        {
            get
            {
                return _StartAuthCommand ?? (_StartAuthCommand = new ViewModelCommand(StartAuth, CanStartAuth));
            }
        }

        public bool CanStartAuth()
        {
            if (!UseCustomKeys) return true;
            if (string.IsNullOrEmpty(ConsumerKey) || string.IsNullOrEmpty(ConsumerSecret))
                return false;
            return true;
        }

        public async void StartAuth()
        {
            IsEnabledStep1 = IsEnabledStep2 = false;
            if (UseCustomKeys)
                await AuthorizerViewModel.StartAuthentication(ConsumerKey, ConsumerSecret);
            else
                await AuthorizerViewModel.StartAuthentication();
            IsEnabledStep1 = IsEnabledStep2 = true;
        }
        #endregion

        #region ConfirmCommand
        private ViewModelCommand _ConfirmCommand;

        public ViewModelCommand ConfirmCommand
        {
            get
            {
                return _ConfirmCommand ?? (_ConfirmCommand = new ViewModelCommand(Confirm, CanConfirm));
            }
        }

        public bool CanConfirm()
        {
            return !string.IsNullOrEmpty(PinCode);
        }

        public async void Confirm()
        {
            IsEnabledStep1 = IsEnabledStep2 = false;
            await AuthorizerViewModel.ConfirmPinCode(PinCode);
            if (AuthorizerViewModel.IsCompleted)
            {
                var msg = new InteractionMessage("CloseWindowKey");
                Messenger.Raise(msg);
            }
            IsEnabledStep1 = IsEnabledStep2 = true;
        }
        #endregion

        public void OpenMessageWindow()
        {
            var msg = new TransitionMessage("OpenMessageWindowMessageKey");
            Messenger.Raise(msg);
        }
    }
}
