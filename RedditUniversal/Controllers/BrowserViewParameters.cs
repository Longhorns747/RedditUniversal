using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditUniversal.Controllers
{
    class BrowserViewParameters
    {
        public string url { get; set; }
        public string access_token { get; set; }
        public bool logged_in { get; set; }
        public string subreddit { get; set; }

        public BrowserViewParameters(string url, string access_token, string subreddit, bool logged_in)
        {
            this.url = url;
            this.access_token = access_token;
            this.logged_in = logged_in;
            this.subreddit = subreddit;
        }

        public BrowserViewParameters() { }
    }
}
