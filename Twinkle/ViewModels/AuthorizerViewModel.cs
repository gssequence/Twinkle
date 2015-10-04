using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinkle.Models;

namespace Twinkle.ViewModels
{
    public class AuthorizerViewModel
    {
        private Authorizer _model;

        public bool IsCompleted { get { return _model.IsCompleted; } }

        public AuthorizerViewModel()
        {
            _model = new Authorizer();
        }

        public async Task StartAuthentication()
        {
            await _model.StartAuthentication();
        }

        public async Task StartAuthentication(string consumerKey, string consumerSecret)
        {
            await _model.StartAuthentication(consumerKey, consumerSecret);
        }

        public async Task ConfirmPinCode(string pin)
        {
            await _model.ConfirmPinCode(pin);
        }
    }
}
