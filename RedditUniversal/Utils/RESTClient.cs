using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using RedditUniversal.Models;
using Newtonsoft.Json;
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

                if(!access_token.Equals(""))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", access_token);

                client.DefaultRequestHeaders.Add("User-Agent", @"WindowsUniversal");
                HttpResponseMessage response = new HttpResponseMessage();

                //Assume (for the moment) we are posting because we are logging in as application
                if (Method.Equals(HttpVerb.POST))
                {
                    client.BaseAddress = new Uri(EndPoint);
                    var byteArray = Encoding.UTF8.GetBytes("tJvmFPf0SKBJGg:lol");
                    var header = new AuthenticationHeaderValue(
                               "Basic", Convert.ToBase64String(byteArray));
                    client.DefaultRequestHeaders.Authorization = header;

                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", @"https://oauth.reddit.com/grants/installed_client"),
                        new KeyValuePair<string, string>("device_id", "blahblahblahblahblah")
                    });

                    response = await client.PostAsync(EndPoint, formContent);
                }
                else if(Method.Equals(HttpVerb.GET))
                {
                    string paramList = (parameters.Equals("")) ? "" : "?" + parameters;
                    response = await client.GetAsync(EndPoint + paramList);
                }

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
