using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditUniversal.Models
{
    class Link
    {
        public string id { get; set; }
        public string author { get; set; }
        public string thumbnail { get; set; }
        public string caption { get; set; }
        public string link { get; set; }

        public Link(string id, string author, string thumbnail, string caption, string link)
        {
            this.id = id;
            this.author = author;
            this.thumbnail = thumbnail;
            this.caption = caption;
            this.link = link;
        }
    }
}
