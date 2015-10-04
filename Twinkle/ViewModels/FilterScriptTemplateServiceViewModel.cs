using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinkle.Models;

namespace Twinkle.ViewModels
{
    public class FilterScriptTemplateServiceViewModel
    {
        private FilterScriptTemplateService _model;
        private TimelineViewModel _tl;

        public IEnumerable<FilterScriptTemplateViewModel> Templates { get { return _model.Templates.Select(t => new FilterScriptTemplateViewModel(t, _tl)); } }

        public FilterScriptTemplateServiceViewModel(TimelineViewModel tl)
        {
            _model = FilterScriptTemplateService.Instance;
            _tl = tl;
        }
    }
}
