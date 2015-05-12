using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RedditUniversal.Utils;
using System.IO;

namespace RedditUniversal.Utils
{
    class RedditRequester
    {
        private string access_token;

        public RedditRequester(string access_token)
        {
            this.access_token = access_token;
        }

        public async Task<Dictionary<string, string>> GetSubreddits(string parameters)
        {
            RestClient listings_request = new RestClient("subreddits/mine/subscriber/", access_token);
            string result = await listings_request.MakeRequest(parameters);
            JsonTextReader reader = new JsonTextReader(new StringReader(result));
            Dictionary<string, string> subreddits = new Dictionary<string, string>();
            while (reader.Read())
            {
                if (reader.Value != null && reader.Value.Equals("id"))
                {
                    reader.Read();
                    string id = (string)reader.Value;

                    while (!reader.Value.Equals("display_name"))
                    {
                        reader.Read();
                    }

                    reader.Read();
                    string display_name = (string)reader.Value;
                    subreddits.Add(display_name, id);
                }
            }

            return subreddits;
        }
    }
}
