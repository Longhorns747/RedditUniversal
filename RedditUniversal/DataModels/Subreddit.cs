using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditUniversal.DataModels
{
    /// <summary>
    /// Data model for a subreddit
    /// </summary>
    class Subreddit
    {
        public string id { get; set; }
        public string display_name { get; set; }

        public Subreddit(string id, string display_name)
        {
            this.id = id;
            this.display_name = display_name;
        }

        public Subreddit(Dictionary<string, string> properties)
        {
            this.id = properties["id"];
            this.display_name = properties["display_name"];
        }

        /// <summary>
        /// Gets a List of properties to retrieve from a JSON response to create Subreddit instances
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTemplate()
        {
            List<string> properties_to_get = new List<string>();
            properties_to_get.Add("id");
            properties_to_get.Add("display_name");

            return properties_to_get;
        }
    }
}
