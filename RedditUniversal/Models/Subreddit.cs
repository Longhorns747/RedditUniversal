using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditUniversal.Models
{
    class Subreddit
    {
        public string id { get; }
        public string display_name { get; }

        public Subreddit(string id, string display_name)
        {
            this.id = id;
            this.display_name = display_name;
        }
    }
}
