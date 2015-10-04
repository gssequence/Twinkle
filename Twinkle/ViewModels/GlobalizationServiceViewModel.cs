using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinkle.Models;
using Twinkle.Mvvm;

namespace Twinkle.ViewModels
{
    public class GlobalizationServiceViewModel : BindableViewModel<GlobalizationService>
    {
        public IEnumerable<CultureInfoViewModel> Cultures { get; private set; }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (SetProperty(ref _selectedIndex, value))
                {
                    Settings.Current.Locale = Cultures.ElementAt(value).Name;
                    GlobalizationService.Instance.ChangeCulture(Settings.Current.Locale);
                }
            }
        }

        public GlobalizationServiceViewModel() : base(GlobalizationService.Instance)
        {
            Cultures = new[] { new CultureInfoViewModel("", "(auto)") }.Concat(Model.Cultures.Select(c => new CultureInfoViewModel(c)));
            int i = 0;
            foreach (var item in Cultures)
            {
                if (item.Name == Settings.Current.Locale) break;
                i++;
            }
            _selectedIndex = i;
        }

        public void ChangeCulture(string name)
        {
            Model.ChangeCulture(name);
        }
    }
}
