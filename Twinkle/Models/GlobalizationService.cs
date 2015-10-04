using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinkle.Mvvm;
using Twinkle.Properties;

namespace Twinkle.Models
{
    public class GlobalizationService : BindableModel
    {
        private static GlobalizationService _instance = new GlobalizationService();
        public static GlobalizationService Instance { get { return _instance; } }

        private static CultureInfo defaultCulture = CultureInfo.CurrentCulture;

        public IEnumerable<CultureInfo> Cultures { get; private set; }

        private Resources _resources = new Resources();
        public Resources Resources
        {
            get { return _resources; }
            set { SetProperty(ref _resources, value); }
        }

        private GlobalizationService()
        {
            var cultures = new string[] { "en-US", "ja-JP" };
            Cultures = cultures.Select(s =>
            {
                try { return CultureInfo.GetCultureInfo(s); }
                catch { return null; }
            }).Where(c => c != null);

            if (!string.IsNullOrEmpty(Settings.Current.Locale))
                ChangeCulture(Settings.Current.Locale);
        }

        public void ChangeCulture(string name)
        {
            var culture = getCultureInfo(name);
            if (culture != null)
            {
                Resources.Culture = culture;
                RaisePropertyChanged(() => Resources);
                Settings.Current.Locale = name;
            }
        }

        public string GetString(string name)
        {
            return Resources.ResourceManager.GetString(name, getCultureInfo(Settings.Current.Locale));
        }

        private CultureInfo getCultureInfo(string name)
        {
            if (string.IsNullOrEmpty(name)) return defaultCulture;
            else return CultureInfo.GetCultureInfo(name);
        }
    }
}
