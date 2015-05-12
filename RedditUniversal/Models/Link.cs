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
        public string title { get; set; }
        public string url { get; set; }

        public Link(string id, string author, string thumbnail, string title, string url)
        {
            this.id = id;
            this.author = author;
            this.thumbnail = thumbnail;
            this.title = title;
            this.url = url;
        }

        public Link(Dictionary<string, string> properties)
        {
            this.id = properties["id"];
            this.author = properties["author"];
            this.thumbnail = properties["thumbnail"];
            this.title = properties["title"];
            this.url = properties["url"];
        }

        public static List<string> GetTemplate()
        {
            List<string> properties_to_get = new List<string>();
            properties_to_get.Add("id");
            properties_to_get.Add("author");
            properties_to_get.Add("thumbnail");
            properties_to_get.Add("url");
            properties_to_get.Add("title");

            return properties_to_get;
        }
    }
}
