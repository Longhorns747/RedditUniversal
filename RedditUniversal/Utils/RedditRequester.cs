using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RedditUniversal.Utils;
using System.IO;
using RedditUniversal.DataModels;
using System.Net.Http;

namespace RedditUniversal.Utils
{
    /// <summary>
    /// Level of abstraction above REST client for making specific Reddit API calls
    /// </summary>
    class RedditRequester
    {
        public State state { get; set; }

        public async static Task<RedditRequester> MakeRedditRequester(State state)
        {
            RedditRequester res = new RedditRequester(state);
            await res.RetrieveUserAccessToken();
            return res;
        }

        private RedditRequester(State state)
        {
            this.state = state;
        }

        /// <summary>
        /// Gets the subscribed subreddits of the currently logged in user
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<List<Subreddit>> GetSubreddits(string parameters)
        {
            string url = "subreddits/mine/subscriber/";
            url = (parameters.Equals("")) ? url : url + "?" + parameters;
            RestClient listings_request = new RestClient(url, state.access_token);
            string result = await listings_request.MakeRequest(parameters);
            List<Subreddit> subreddits = new List<Subreddit>();

            SubredditTree subreddit_tree = JsonConvert.DeserializeObject<SubredditTree>(result);

            foreach (SubredditChild subreddit in subreddit_tree.data.children)
            {
                subreddits.Add(subreddit.data);
            }

            return subreddits;
        }

        /// <summary>
        /// Gets the front page links from Reddit
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<Tuple<List<Link>, string>> GetHot(string parameters)
        {
            return await GetHot(new Subreddit(), parameters);
        }

        /// <summary>
        /// Gets the links from a specific subreddit from Reddit
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        
        public async Task<Tuple<List<Link>, string>> GetHot(Subreddit target, string parameters)
        {
            List<Link> links = new List<Link>();
            string url = (target.id == null) ? "/hot" : "/r/" + target.title + "/hot";
            url = (parameters.Equals("")) ? url : url + "?" + parameters;
            RestClient listings_request = new RestClient(url, state.access_token);
            string result = await listings_request.MakeRequest(parameters);

            LinkTree link_tree = JsonConvert.DeserializeObject<LinkTree>(result);

            foreach (LinkChild link in link_tree.data.children)
            {
                links.Add(link.data);
            }

            return new Tuple<List<Link>, string>(links, link_tree.data.after);
        }

        /// <summary>
        /// Gets the comments for a particular link
        /// </summary>
        /// <param name="link"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<Tuple<List<Comment>, string>> GetComments(Link link, string parameters)
        {
            List<Comment> comments = new List<Comment>();
            string url = "/comments/" + link.id;
            RestClient comments_request = new RestClient(url, state.access_token);
            string result = await comments_request.MakeRequest();

            List<CommentTree> comment_tree = JsonConvert.DeserializeObject<List<CommentTree>>(result);

            foreach (CommentChild comment in comment_tree[1].data.children)
            {
                comments.Add(comment.data);
            }

            comments.Remove(comments.Last());

            return new Tuple<List<Comment>, string>(comments, comment_tree[1].data.after);
        }

        /// <summary>
        /// Retrieves the access token for either a user or the Application depending on the access_token set
        /// when the requester object was instantiated
        /// </summary>
        /// <returns>Returns new access_token</returns>
        public async Task RetrieveUserAccessToken()
        {
            if (state.refresh_token.Equals("") || state.access_token.Equals(""))
            {
                state.access_token = await AppLogin();
            }
        }

        /// <summary>
        /// Logs in for the application as opposed to a specific user
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Refreshes the access_token using the webapp
        /// </summary>
        /// <returns>an updated state with the refreshed token</returns>
        public async Task<State> RefreshToken()
        {
            if (NeedToRefresh() && !state.refresh_token.Equals(""))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://universalredditlogin.azurewebsites.net/home/refreshtoken?refresh_token=" + state.refresh_token);
                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                    string content = await response.Content.ReadAsStringAsync();

                    if (content.Contains("access_token"))
                    {
                        JsonTextReader reader = new JsonTextReader(new StringReader(content));

                        while (reader.Read())
                        {
                            if (reader.Value != null)
                            {
                                if (reader.Value.Equals("access_token"))
                                {
                                    reader.Read();
                                    this.state.access_token = (string)reader.Value;
                                }
                                else if (reader.Value.Equals("expires_in"))
                                {
                                    reader.Read();
                                    Int64 expires_in = (Int64)reader.Value;
                                    this.state.expire_time = DateTime.UtcNow.AddSeconds(expires_in);
                                }
                            }
                        }
                    }
                }
            }

            return state;
        }

        /// <summary>
        /// Checks if the token needs to be refreshed
        /// </summary>
        /// <returns></returns>
        private bool NeedToRefresh()
        {
            return DateTime.UtcNow.CompareTo(state.expire_time) > 0;
        }
    }
}
