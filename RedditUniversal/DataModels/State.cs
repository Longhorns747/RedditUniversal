using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditUniversal.DataModels
{
    class State
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public bool logged_in { get; set; }
        public Link current_link { get; set; }
        public Subreddit current_subreddit { get; set; }
        public DateTime expire_time { get; set; }
    }
}
