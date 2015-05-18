using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RedditUniversal.Utils;
using System.IO;
using RedditUniversal.DataModels;

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

        public async Task<Tuple<List<Link>, string>> GetHot(string parameters)
        {
            return await GetHot(new Subreddit("", ""), parameters);
        }

        public async Task<Tuple<List<Link>, string>> GetHot(Subreddit target, string parameters)
        {
            List<Link> links = new List<Link>();
            string url = (target.id.Equals("")) ? "/hot" : "/r/" + target.display_name + "/hot";
            url = (parameters.Equals("")) ? url : url + "?" + parameters;
            RestClient listings_request = new RestClient(url, access_token);
            string result = await listings_request.MakeRequest(parameters);

            LinkTree link_tree = JsonConvert.DeserializeObject<LinkTree>(result);

            foreach (LinkChild link in link_tree.data.children)
            {
                links.Add(link.data);
            }

            return new Tuple<List<Link>, string>(links, link_tree.data.after);
        }

        public async Task<Tuple<List<Comment>, string>> GetComments(Link link, string parameters)
        {
            List<Comment> comments = new List<Comment>();
            string url = "/comments/" + link.id;
            RestClient comments_request = new RestClient(url, access_token);
            string result = await comments_request.MakeRequest();

            List<CommentTree> comment_tree = JsonConvert.DeserializeObject<List<CommentTree>>(result);

            foreach (CommentChild comment in comment_tree[1].data.children)
            {
                comments.Add(comment.data);
            }

            return new Tuple<List<Comment>, string>(comments, comment_tree[1].data.after);
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

            while (reader.Read())
            {
                if (reader.Value != null && reader.Value.Equals("access_token"))
                {
                    reader.Read();
                    res = (string)reader.Value;
                }
            }

            return res;
        }
    }
}
