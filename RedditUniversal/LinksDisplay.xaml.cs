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
        State current_state;
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
            ApplicationData.Current.DataChanged += new TypedEventHandler<ApplicationData, object>(DataChangeHandler);
        }

        /// <summary>
        /// If this page is navigated to from elsewhere, parse the parameters to maintain state and retrieve links
        /// </summary>
        /// <param name="e">Navigation parameters of type: ListingDisplayParameters</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            current_state = (State)(e.Parameter);
            requester = await RedditRequester.MakeRedditRequester(current_state);

            current_state = await requester.RefreshToken();

            if (current_state.logged_in)
            {
                subreddits = await GetSubreddits();
                login_but.Visibility = Visibility.Collapsed;
                logout_but.Visibility = Visibility.Visible;
            }
            else
            {
                login_but.Visibility = Visibility.Visible;
                logout_but.Visibility = Visibility.Collapsed;
            }

            current_subreddit_label.Text = (current_state.current_subreddit.display_name.Equals("")) ? "Front Page" : current_state.current_subreddit.display_name;

            await GetHot("");
            progress_ring.Visibility = Visibility.Collapsed;
            progress_ring.IsActive = false;
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
        private async Task GetHot(string parameters)
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
        private async Task GetHot(Subreddit target)
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
        private async void AddLinksToUI(List<Link> links, string after)
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
                await GetHot("after=" + after);
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

            bool res = LinkPanelScrollViewer.ChangeView(null, current_state.vertical_scroll_offset, null, false);

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
                button.resize_button();
            }
        }

        /// <summary>
        /// Handler for the login button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void login_but_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));
        }

        /// <summary>
        /// Handler for each link button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void link_but_Click(object sender, RoutedEventArgs e)
        {
            current_state.current_link = ((LinkButton)sender).link;
            current_state.vertical_scroll_offset = LinkPanelScrollViewer.VerticalOffset;
            this.Frame.Navigate(typeof(BrowserView), current_state);
        }

        /// <summary>
        /// Handler for suspending the app; saves state to the cloud
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["access_token"] = current_state.access_token;
            roamingSettings.Values["refresh_token"] = current_state.refresh_token;
            roamingSettings.Values["expire_time"] = current_state.expire_time.ToString();
            roamingSettings.Values["logged_in"] = current_state.logged_in;
            roamingSettings.Values["current_subreddit_id"] = current_state.current_subreddit.id;
            roamingSettings.Values["current_subreddit_display_name"] = current_state.current_subreddit.display_name;
        }

        /// <summary>
        /// Resums app; retrieves state from the cloud
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_Resuming(Object sender, Object e)
        {
            ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            current_state.access_token = (string)roamingSettings.Values["access_token"];
            current_state.refresh_token = (string)roamingSettings.Values["refresh_token"];
            current_state.expire_time = DateTime.Parse((string)roamingSettings.Values["expire_time"]);
            current_state.logged_in = (bool)roamingSettings.Values["logged_in"];
            current_state.current_subreddit = new Subreddit((string)roamingSettings.Values["current_subreddit_id"], (string)roamingSettings.Values["current_subreddit_display_name"]);
        }

        /// <summary>
        /// Roaming app data change handler
        /// </summary>
        /// <param name="appData"></param>
        /// <param name="o"></param>
        void DataChangeHandler(Windows.Storage.ApplicationData appData, object o)
        {
            ApplicationDataContainer roamingSettings = appData.RoamingSettings;
            current_state.access_token = (string)roamingSettings.Values["access_token"];
            current_state.refresh_token = (string)roamingSettings.Values["refresh_token"];
            current_state.expire_time = DateTime.Parse((string)roamingSettings.Values["expire_time"]);
            current_state.logged_in = (bool)roamingSettings.Values["logged_in"];
            current_state.current_subreddit = new Subreddit((string)roamingSettings.Values["current_subreddit_id"], (string)roamingSettings.Values["current_subreddit_display_name"]);
        }


        /// <summary>
        /// Logs the current user out by refreshing the current state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void logout_but_Click(object sender, RoutedEventArgs e)
        {
            State blank_state = new State();
            blank_state.access_token = "";
            blank_state.current_link = new Link();
            blank_state.current_subreddit = new Subreddit("", "");
            blank_state.refresh_token = "";
            blank_state.expire_time = DateTime.Now;
            blank_state.logged_in = false;
            requester = await RedditRequester.MakeRedditRequester(blank_state);

            this.Frame.Navigate(typeof(LinksDisplay), blank_state);
        }
    }
}
