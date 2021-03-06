﻿using System;
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
        int num_links = 0;
        State current_state;
        public static int max_count = 0;
        bool need_to_scroll = true; //TODO: Find better way to do this

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
                login_but.Visibility = Visibility.Collapsed;
                logout_but.Visibility = Visibility.Visible;
                subreddit_but.Visibility = Visibility.Visible;
            }
            else
            {
                login_but.Visibility = Visibility.Visible;
                logout_but.Visibility = Visibility.Collapsed;
                subreddit_but.Visibility = Visibility.Collapsed;
            }

            current_subreddit_label.Text = (current_state.current_subreddit.title == null || current_state.current_subreddit.title.Equals("")) ? "Front Page" : current_state.current_subreddit.title;

            need_to_scroll = true;
            if(current_state.current_subreddit.title == null || current_state.current_subreddit.title.Equals(""))
            {
                await GetHot("");
                front_page_but.Visibility = Visibility.Collapsed;
            }
            else
            {
                await GetHot(current_state.current_subreddit);
            }
            
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

            await AddLinksToUI(links, after);
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

            await AddLinksToUI(links, after);
        }

        /// <summary>
        /// Returns links for a specific subreddit in "hot" order
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async Task GetHot(Subreddit target, string parameters)
        {
            Tuple<List<Link>, string> result = await requester.GetHot(target, "count=" + max_count + "&" + parameters);
            List<Link> links = result.Item1;
            string after = result.Item2;

            await AddLinksToUI(links, after);
        }

        /// <summary>
        /// Add a list of links to the UI
        /// </summary>
        /// <param name="links"></param>
        /// <param name="after">The value of the next set of links to retrieve</param>
        private async Task AddLinksToUI(List<Link> links, string after)
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

            if (num_links < max_count)
            {
                if (current_state.current_subreddit.id != null || !current_state.current_subreddit.id.Equals(""))
                    await GetHot("after=" + after);
                else
                    await GetHot(current_state.current_subreddit, "after=" + after);
            }
            else
            {
                AddAfterButtonToUI(after);
                if (need_to_scroll)
                {
                    progress_ring.Visibility = Visibility.Collapsed;
                    progress_ring.IsActive = false;
                    menu.Visibility = Visibility.Visible;
                    bool res = LinkPanelScrollViewer.ChangeView(null, current_state.vertical_scroll_offset, null, false);
                    need_to_scroll = false;
                }
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
            after_but.HorizontalAlignment = HorizontalAlignment.Center;

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
            LinkPanel.Children.Remove(after_but);
            LinkPanel.RowDefinitions.Remove(LinkPanel.RowDefinitions.Last());
            Tuple<List<Link>, string> result;

            if (current_state.current_subreddit.title == null || current_state.current_subreddit.title.Equals(""))
            {
                result = await requester.GetHot("after=" + after_but.after + "&count=" + max_count);
            }
            else
            {
                result = await requester.GetHot(current_state.current_subreddit, "after=" + after_but.after + "&count=" + max_count);
            }
                
            List<Link> links = result.Item1;
            string after = result.Item2;

            await AddLinksToUI(links, after);
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
            blank_state.current_subreddit = new Subreddit();
            blank_state.refresh_token = "";
            blank_state.expire_time = DateTime.Now;
            blank_state.logged_in = false;
            max_count = 0;
            requester = await RedditRequester.MakeRedditRequester(blank_state);

            this.Frame.Navigate(typeof(LinksDisplay), blank_state);
        }

        private void subreddit_but_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SubredditsView), current_state);
        }

        private void front_page_but_Click(object sender, RoutedEventArgs e)
        {
            Subreddit subreddit = new Subreddit();
            current_state.current_subreddit = subreddit;
            max_count = 0;
            current_state.vertical_scroll_offset = 0;
            this.Frame.Navigate(typeof(LinksDisplay), current_state);
        }
    }
}
