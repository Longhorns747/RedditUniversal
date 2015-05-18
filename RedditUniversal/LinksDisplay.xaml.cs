using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
using RedditUniversal.Utils;
using RedditUniversal.DataModels;
using Windows.UI.Xaml.Media.Imaging;
using RedditUniversal.ParameterModels;
using Windows.UI.Core;
using RedditUniversal.ViewModels;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RedditUniversal
{
    /// <summary>
    /// Page to view links from a subreddit
    /// </summary>
    public sealed partial class LinksDisplay : Page
    {
        RedditRequester requester;
        List<LinkButton> link_buttons = new List<LinkButton>();
        List<Subreddit> subreddits;
        int num_links = 0;
        bool logged_in = false;
        string current_subreddit = "";
        static int max_count = 0;

        /// <summary>
        /// Initalizes the LinkDisplay and adds some application level handlers as this is the first page to run
        /// </summary>
        public LinksDisplay()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(this.Resize_Buttons);
            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);
            Application.Current.Resuming += new EventHandler<Object>(App_Resuming);
            Windows.Storage.ApplicationData.Current.DataChanged += new TypedEventHandler<ApplicationData, object>(DataChangeHandler);
        }

        /// <summary>
        /// If this page is navigated to from elsewhere, parse the parameters to maintain state and retrieve links
        /// </summary>
        /// <param name="e">Navigation parameters of type: ListingDisplayParameters</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            LinksDisplayParameters parameters = (LinksDisplayParameters)(e.Parameter);
            requester = new RedditRequester(parameters.access_token);

            await requester.RetrieveUserAccessToken();

            if (parameters.logged_in)
                subreddits = await GetSubreddits();

            logged_in = parameters.logged_in;

            current_subreddit = (parameters.subreddit.Equals("")) ? "Front Page" : parameters.subreddit;
            current_subreddit_label.Text = current_subreddit;

            GetHot("");
        }

        /// <summary>
        /// Returns the user's subscribed subreddits
        /// </summary>
        /// <returns></returns>
        private async Task<List<Subreddit>> GetSubreddits()
        {
            return await requester.GetSubreddits("");
        }

        /// <summary>
        /// Returns links in "hot" order
        /// </summary>
        /// <param name="parameters">url parameters</param>
        private async void GetHot(string parameters)
        {
            Tuple<List<Link>, string> result = await requester.GetHot("count=" + max_count + "&" + parameters);
            List<Link> links = result.Item1;
            string after = result.Item2;

            AddLinksToUI(links, after);
        }

        /// <summary>
        /// Returns links for a specific subreddit in "hot" order
        /// </summary>
        /// <param name="target"></param>
        private async void GetHot(Subreddit target)
        {
            Tuple<List<Link>, string> result = await requester.GetHot(target, "count=" + max_count);
            List<Link> links = result.Item1;
            string after = result.Item2;

            AddLinksToUI(links, after);
        }

        /// <summary>
        /// Add a list of links to the UI
        /// </summary>
        /// <param name="links"></param>
        /// <param name="after">The value of the next set of links to retrieve</param>
        private void AddLinksToUI(List<Link> links, string after)
        {
            foreach(Link link in links)
            {
                LinkButton curr_button = new LinkButton(link);
                curr_button.Click += new RoutedEventHandler(link_but_Click);

                Grid.SetRow(curr_button, num_links);
                num_links++;
                max_count = (num_links > max_count) ? max_count + 1 : max_count;
                link_buttons.Add(curr_button);

                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;

                LinkPanel.RowDefinitions.Add(row);
                LinkPanel.Children.Add(curr_button);
            }

            if(num_links < max_count)
            {
                GetHot("after=" + after);
            }
            else
            {
                AddAfterButtonToUI(after);
            }           
        }

        /// <summary>
        /// Adds the "more" button to bottom of UI
        /// </summary>
        /// <param name="after"></param>
        private void AddAfterButtonToUI(string after)
        {
            AfterButton after_but = new AfterButton(after);
            after_but.Content = "More";
            after_but.Click += new RoutedEventHandler(after_but_Click);
            Grid.SetRow(after_but, num_links);

            RowDefinition row = new RowDefinition();
            row.Height = GridLength.Auto;

            LinkPanel.RowDefinitions.Add(row);
            LinkPanel.Children.Add(after_but);
        }

        /// <summary>
        /// Handler for the "more" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void after_but_Click(object sender, RoutedEventArgs e)
        {
            AfterButton after_but = (AfterButton)sender;
            Tuple<List<Link>, string> result = await requester.GetHot("after=" + after_but.after + "&count=" + max_count);
            List<Link> links = result.Item1;
            string after = result.Item2;

            AddLinksToUI(links, after);
        }

        /// <summary>
        /// Resizes buttons based on window size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Resize_Buttons(object sender, WindowSizeChangedEventArgs e)
        {
            foreach(LinkButton button in link_buttons)
            {
                button.Width = Window.Current.Bounds.Width;
                button.GetCaption().Width = button.Width - button.GetThumbnail().Width; //So much jank, so little time
            }
        }

        /// <summary>
        /// Handler for the login button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void login_but_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        /// <summary>
        /// Handler for each link button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void link_but_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BrowserView), new BrowserViewParameters(((LinkButton)sender).link, requester.access_token, current_subreddit, logged_in));
        }

        /// <summary>
        /// Handler for suspending the app; saves state to the cloud
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["access_token"] = requester.access_token;
            roamingSettings.Values["logged_in"] = logged_in;
        }

        /// <summary>
        /// Resums app; retrieves state from the cloud
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_Resuming(Object sender, Object e)
        {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            requester = new RedditRequester((string)roamingSettings.Values["access_token"]);
            logged_in = (bool)roamingSettings.Values["logged_in"];
        }

        /// <summary>
        /// Roaming app data change handler
        /// </summary>
        /// <param name="appData"></param>
        /// <param name="o"></param>
        void DataChangeHandler(Windows.Storage.ApplicationData appData, object o)
        {
            requester = new RedditRequester((string)appData.RoamingSettings.Values["access_token"]);
            logged_in = (bool)appData.RoamingSettings.Values["logged_in"];
        }
    }
}
