using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinkle.ViewModels
{
    public class CultureInfoViewModel
    {
        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public CultureInfoViewModel(CultureInfo model)
        {
            Name = model.Name;
            DisplayName = string.Format("{0} [{1}]", model.EnglishName, Name);
        }

        public CultureInfoViewModel(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }
    }
}
