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
using RedditUniversal.Controllers;
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RedditUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserView : Page
    {
        string access_token = "";
        bool logged_in;
        string subreddit;

        public BrowserView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            BrowserViewParameters parameters = (BrowserViewParameters)e.Parameter;
            string url = parameters.url;
            access_token = parameters.access_token;
            logged_in = parameters.logged_in;
            subreddit = parameters.subreddit;

            HttpRequestMessage webRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
            webRequest.Headers.UserAgent.Add(new Windows.Web.Http.Headers.HttpProductInfoHeaderValue("Mozilla/5.0 (Linux; <Android Version>; <Build Tag etc.>) AppleWebKit/<WebKit Rev> (KHTML, like Gecko) Chrome/<Chrome Rev> Mobile Safari/<WebKit Rev>"));

            webViewer.IsTapEnabled = false;
            webViewer.NavigateWithHttpRequestMessage(webRequest);
        }

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListingDisplay), new ListingDisplayParameters(subreddit, logged_in, access_token));
        }
    }
}
