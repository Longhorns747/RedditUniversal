using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RedditUniversal.Utils;
using RedditUniversal.Models;
using System.IO;

namespace RedditUniversal.Utils
{
    class RedditRequester
    {
        public string access_token { get; set; }

        public RedditRequester(string access_token)
        {
            this.access_token = access_token;
        }

        public async Task<List<Subreddit>> GetSubreddits(string parameters)
        {
            string url = "subreddits/mine/subscriber/";
            url = (parameters.Equals("")) ? url : url + "?" + parameters;
            RestClient listings_request = new RestClient(url, access_token);
            string result = await listings_request.MakeRequest(parameters);
            JsonTextReader reader = new JsonTextReader(new StringReader(result));
            List<Subreddit> subreddits = new List<Subreddit>();
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
                    subreddits.Add(new Subreddit(id, display_name));
                }
            }

            return subreddits;
        }

        public async Task<List<Link>> GetHot(string parameters)
        {
            return await GetHot(new Subreddit("", ""), parameters);
        }

        public async Task<List<Link>> GetHot(Subreddit target, string parameters)
        {
            string url = (target.id.Equals("")) ? "/hot" : "/r/" + target.display_name + "/hot";
            url = (parameters.Equals("")) ? url : url + "?" + parameters;
            RestClient listings_request = new RestClient(url, access_token);
            string result = await listings_request.MakeRequest(parameters);
            return GetLinkProperties(result);
        }

        private List<Subreddit> GetSubredditProperties(string json)
        {
            List<string> properties_to_get = Subreddit.GetTemplate();
            List<Dictionary<string, string>> properties = GetProperties(properties_to_get, json);
            List<Subreddit> res = new List<Subreddit>();

            foreach (Dictionary<string, string> subreddit in properties)
            {
                Subreddit curr_subreddit = new Subreddit(subreddit);
                res.Add(curr_subreddit);
            }

            return res;
        }

        private List<Link> GetLinkProperties(string json)
        {
            List<string> properties_to_get = Link.GetTemplate();
            List<Dictionary<string, string>> properties = GetProperties(properties_to_get, json);
            List<Link> res = new List<Link>();

            foreach (Dictionary<string, string> link in properties)
            {
                Link curr_link = new Link(link);
                res.Add(curr_link);
            }

            return res;
        }

        public async Task<bool> RetrieveUserAccessToken()
        {
            if (access_token.Equals(""))
            {
                access_token = await AppLogin();
                return false;
            }

            return true;
        }

        private async Task<String> AppLogin()
        {
            RestClient login = new RestClient("https://www.reddit.com/api/v1/access_token", HttpVerb.POST, "");
            string response = await login.MakeRequest();
            JsonTextReader reader = new JsonTextReader(new StringReader(response));
            string res = null;

            while(reader.Read())
            {
                if(reader.Value != null && reader.Value.Equals("access_token"))
                {
                    reader.Read();
                    res = (string)reader.Value;
                }
            }

            return res;
        }

        private List<Dictionary<string, string>> GetProperties(List<string> properties, string json)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            JsonTextReader reader = new JsonTextReader(new StringReader(json));

            while (reader.Read())
            {
                if (reader.Value != null && reader.Value.Equals(properties.First()))
                {
                    Dictionary<string, string> property_dic = new Dictionary<string, string>();

                    reader.Read();
                    property_dic[properties.First()] = (string)reader.Value;
                    List<string> except = new List<string>();
                    except.Add(properties.First());

                    foreach (string property in properties.Except(except))
                    {
                        while (reader.Read())
                        {
                            if (reader.Value != null && reader.Value.Equals(property))
                            {
                                reader.Read();
                                property_dic[property] = (string)reader.Value;
                                break;
                            }
                        }                        
                    }

                    result.Add(property_dic);
                }

                if(reader.Value != null && reader.Value.Equals("after"))
                {
                    reader.Read();
                    Dictionary<string, string> after = new Dictionary<string, string>();
                    after["after"] = (string)reader.Value;
                    result.Add(after);
                }
            }

            return result;
        }
    }
}
