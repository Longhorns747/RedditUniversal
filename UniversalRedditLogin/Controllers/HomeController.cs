using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UniversalRedditLogin.Utils;

namespace UniversalRedditLogin.Controllers
{
    public class HomeController : Controller
    {
        public void Index()
        {
            string base_url = @"https://www.reddit.com/api/v1/authorize?client_id=" + RedditInfo.APP_ID + 
                @"&response_type=code&state=lol&redirect_uri=http://universalredditlogin.azurewebsites.net/home/loggedin" + "&duration=permanent&scope=read,mysubreddits,identity";

            Response.Redirect(base_url);
        }

        public async Task<string> RefreshToken()
        {
            string refresh_token = Request.QueryString["refresh_token"];
            string content = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.reddit.com/api/v1/access_token");
                var byteArray = Encoding.UTF8.GetBytes(RedditInfo.APP_ID + ":" + RedditInfo.APP_SECRET);
                var header = new AuthenticationHeaderValue(
                           "Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Authorization = header;

                var formContent = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("grant_type", "refresh_token"),
                        new KeyValuePair<string, string>("refresh_token", refresh_token)
                    });

                HttpResponseMessage response = await client.PostAsync(new Uri("https://www.reddit.com/api/v1/access_token"), formContent);
                content = await response.Content.ReadAsStringAsync();
            }

            return content;
        }

        public async Task<string> LoggedIn()
        {
            string code = Request.QueryString["code"];
            string content = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.reddit.com/api/v1/access_token");
                var byteArray = Encoding.UTF8.GetBytes(RedditInfo.APP_ID + ":" + RedditInfo.APP_SECRET);
                var header = new AuthenticationHeaderValue(
                           "Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Authorization = header;

                var formContent = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("grant_type", "authorization_code"),
                        new KeyValuePair<string, string>("code", code),
                        new KeyValuePair<string, string>("redirect_uri", "http://universalredditlogin.azurewebsites.net/home/loggedin")
                    });

                HttpResponseMessage response = await client.PostAsync(new Uri("https://www.reddit.com/api/v1/access_token"), formContent);
                content = await response.Content.ReadAsStringAsync();
            }

            return content;
        }
    }
}