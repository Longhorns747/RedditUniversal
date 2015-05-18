using RedditUniversal.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditUniversal.ParameterModels
{
    class CommentViewParameters
    {
        public string access_token { get; set; }
        public Link current_link { get; set; }
        public bool logged_in { get; set; }
        public string subreddit { get; set; }

        public CommentViewParameters(string access_token, Link current_link, bool logged_in, string subreddit)
        {
            this.access_token = access_token;
            this.current_link = current_link;
            this.logged_in = logged_in;
            this.subreddit = subreddit;
        }
    }
}
