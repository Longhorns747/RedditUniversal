using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace RedditUniversal.Controllers
{
    class ListingDisplayParameters
    {
        public string subreddit { get; set; }
        public bool logged_in { get; set; }
        public string access_token { get; set; }

        public ListingDisplayParameters(string subreddit, bool logged_in, string access_token)
        {
            this.subreddit = subreddit;
            this.logged_in = logged_in;
            this.access_token = access_token;
        }

        public ListingDisplayParameters()
        {

        }
    }
}
