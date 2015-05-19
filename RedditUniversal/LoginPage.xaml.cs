using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using RedditUniversal.DataModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RedditUniversal
{
    /// <summary>
    /// Page to login the user to reddit
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        string url =  @"http://universalredditlogin.azurewebsites.net/";

        public LoginPage()
        {
            this.InitializeComponent();
            web.Navigate(new Uri(url));
        }

        private async void web_LoadCompleted(object sender, NavigationEventArgs e)
        {
            string content = await ((WebView)sender).InvokeScriptAsync("eval", new string[] { "document.body.innerHTML;" });
            if (content.Contains("access_token"))
            {
                JsonTextReader reader = new JsonTextReader(new StringReader(content));
                State state = new State();

                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        if (reader.Value.Equals("access_token"))
                        {
                            reader.Read();
                            state.access_token = (string)reader.Value;
                        }
                        else if (reader.Value.Equals("refresh_token"))
                        {
                            reader.Read();
                            state.refresh_token = (string)reader.Value;
                        }
                        else if (reader.Value.Equals("expires_in"))
                        {
                            reader.Read();
                            Int64 expires_in = (Int64)reader.Value;
                            state.expire_time = DateTime.UtcNow.AddSeconds(expires_in);
                        }
                    }
                }

                state.logged_in = true;
                state.current_subreddit = new Subreddit("", "");
                state.current_link = new Link();
                this.Frame.Navigate(typeof(LinksDisplay), state);
            }            
        }
    }
}
