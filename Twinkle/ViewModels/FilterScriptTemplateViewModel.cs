using Livet.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinkle.Models;

namespace Twinkle.ViewModels
{
    public class FilterScriptTemplateViewModel
    {
        private FilterScriptTemplate _model;
        private TimelineViewModel _tl;

        public string Name { get { return _model.Name; } }

        public string Script { get { return _model.Script; } }

        public FilterScriptTemplateViewModel(FilterScriptTemplate model, TimelineViewModel tl)
        {
            _model = model;
            _tl = tl;
        }


        #region ApplyCommand
        private ViewModelCommand _ApplyCommand;

        public ViewModelCommand ApplyCommand
        {
            get
            {
                if (_ApplyCommand == null)
                {
                    _ApplyCommand = new ViewModelCommand(Apply);
                }
                return _ApplyCommand;
            }
        }

        public void Apply()
        {
            _tl.ScriptInput = Script;
        }
        #endregion

    }
}
