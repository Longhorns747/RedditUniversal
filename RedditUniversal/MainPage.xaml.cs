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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RedditUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string client_id = "tJvmFPf0SKBJGg";
        const string redirect_uri = "http://www.reddituniversal.com";
        const string scope = "read,mysubreddits,identity";
        static string url =
            @"https://www.reddit.com/api/v1/authorize?client_id=" + client_id +
                "&response_type=token&state=huehue&redirect_uri=" + redirect_uri + "&scope=" + scope;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            web.Navigate(new Uri(url));
        }

        private void web_NavStart(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            string target_url = "";
            try { target_url = args.Uri.ToString(); }
            finally
            {
                if(target_url.Contains("access_token"))
                {
                    string[] parts = target_url.Split(new char[] { '#', '&' });
                    string access_token = parts[1].Substring(13);

                    this.Frame.Navigate(typeof(ListingDisplay), access_token);
                }

            }
        }
    }
}
