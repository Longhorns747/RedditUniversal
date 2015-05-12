using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RedditUniversal.Utils
{
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class RestClient
    {
        const string BASE_URL = "https://oauth.reddit.com";
        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public string access_token { get; set; }

        public RestClient()
        {
            EndPoint = "";
            Method = HttpVerb.GET;
            ContentType = "application/json";
            PostData = "";
        }
        public RestClient(string endpoint, string access_token)
        {
            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "application/json";
            PostData = "";
            this.access_token = access_token;
        }
        public RestClient(string endpoint, HttpVerb method, string access_token)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json";
            PostData = "";
            this.access_token = access_token;
        }

        public RestClient(string endpoint, HttpVerb method, string postData, string access_token)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json";
            PostData = postData;
            this.access_token = access_token;
        }


        public async Task<string> MakeRequest()
        {
            return await MakeRequest("");
        }

        public async Task<string> MakeRequest(string parameters)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", access_token);
                client.DefaultRequestHeaders.Add("User-Agent", "WindowsUniversal");

                string paramList = (parameters.Equals("")) ? "" : "?" + parameters;
                HttpResponseMessage response = await client.GetAsync(EndPoint + paramList);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return content;
                }
            }
        }

    }

}
