using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinkle.Models
{
    public class FilterScriptTemplateService
    {
        private static FilterScriptTemplateService _instance = new FilterScriptTemplateService();
        public static FilterScriptTemplateService Instance { get { return _instance; } }

        public IEnumerable<FilterScriptTemplate> Templates { get; private set; }

        private FilterScriptTemplateService()
        {
            List<FilterScriptTemplate> templates = new List<FilterScriptTemplate>();
            templates.Add(new FilterScriptTemplate("All Received Tweets", "true"));
            templates.Add(new FilterScriptTemplate("User", "tweet.Status.User.ScreenName == \"username\""));
            templates.Add(new FilterScriptTemplate("Mentions", "tweet.OriginalStatus.Text.Contains(\"@username\")"));
            templates.Add(new FilterScriptTemplate("Favorites", "tweet.IsFavorited"));
            templates.Add(new FilterScriptTemplate("Deleted Tweets", "tweet.IsDeleted"));
            Templates = templates;
        }
    }
}
