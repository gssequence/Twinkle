using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinkle.Models
{
    public class TwitterConfigurationService
    {
        private static TwitterConfigurationService _instance = new TwitterConfigurationService();
        public static TwitterConfigurationService Instance { get { return _instance; } }

        public Configurations Configurations { get; set; }

        private TwitterConfigurationService()
        {

        }
    }
}
