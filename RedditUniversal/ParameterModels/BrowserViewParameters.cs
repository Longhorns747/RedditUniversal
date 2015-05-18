using RedditUniversal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditUniversal.Controllers
{
    class BrowserViewParameters
    {
        public Link current_link { get; set; }
        public string access_token { get; set; }
        public bool logged_in { get; set; }
        public string subreddit { get; set; }

        public BrowserViewParameters(Link current_link, string access_token, string subreddit, bool logged_in)
        {
            this.current_link = current_link;
            this.access_token = access_token;
            this.logged_in = logged_in;
            this.subreddit = subreddit;
        }

        public BrowserViewParameters() { }
    }
}
