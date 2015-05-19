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
using Windows.Web.Http;
using RedditUniversal.ViewModels;
using RedditUniversal.Utils;
using RedditUniversal.DataModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RedditUniversal
{
    /// <summary>
    /// A page that shows the content of a Reddit link
    /// </summary>
    public sealed partial class BrowserView : Page
    {
        State current_state;

        public BrowserView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Parses the parameters passed on when this page is navigated to
        /// </summary>
        /// <param name="e">Must be of type BrowserViewParameters</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            current_state = (State)e.Parameter;

            HttpRequestMessage webRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(current_state.current_link.url));
            webRequest.Headers.UserAgent.Add(new Windows.Web.Http.Headers.HttpProductInfoHeaderValue("Mozilla/5.0 (Linux; <Android Version>; <Build Tag etc.>) AppleWebKit/<WebKit Rev> (KHTML, like Gecko) Chrome/<Chrome Rev> Mobile Safari/<WebKit Rev>"));

            webViewer.IsTapEnabled = false;
            webViewer.NavigateWithHttpRequestMessage(webRequest);
        }

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LinksDisplay), current_state);
        }

        private void comments_button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CommentsView), current_state);
        }
    }
}
