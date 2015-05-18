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
using RedditUniversal.ParameterModels;
using Windows.UI.Core;
using RedditUniversal.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RedditUniversal
{
    /// <summary>
    /// Page to view links from a subreddit
    /// </summary>
    public sealed partial class ListingDisplay : Page
    {
        RedditRequester requester;
        List<LinkButton> link_buttons = new List<LinkButton>();
        List<Subreddit> subreddits;
        int num_links = 0;
        bool logged_in = false;
        string current_subreddit = "";
        static int max_count = 0;

        public ListingDisplay()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(this.Resize_Buttons);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ListingDisplayParameters parameters = (ListingDisplayParameters)(e.Parameter);
            requester = new RedditRequester(parameters.access_token);

            await requester.RetrieveUserAccessToken();

            if (parameters.logged_in)
                subreddits = await GetSubreddits();

            logged_in = parameters.logged_in;

            current_subreddit = (parameters.subreddit.Equals("")) ? "Front Page" : parameters.subreddit;
            current_subreddit_label.Text = current_subreddit;

            GetHot("");
        }

        private async Task<List<Subreddit>> GetSubreddits()
        {
            return await requester.GetSubreddits("");
        }

        private async void GetHot(string parameters)
        {
            Tuple<List<Link>, string> result = await requester.GetHot("count=" + max_count + "&" + parameters);
            List<Link> links = result.Item1;
            string after = result.Item2;

            AddLinksToUI(links, after);
        }

        private async void GetHot(Subreddit target)
        {
            Tuple<List<Link>, string> result = await requester.GetHot(target, "count=" + max_count);
            List<Link> links = result.Item1;
            string after = result.Item2;

            AddLinksToUI(links, after);
        }

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

        private async void after_but_Click(object sender, RoutedEventArgs e)
        {
            AfterButton after_but = (AfterButton)sender;
            Tuple<List<Link>, string> result = await requester.GetHot("after=" + after_but.after + "&count=" + max_count);
            List<Link> links = result.Item1;
            string after = result.Item2;

            AddLinksToUI(links, after);
        }

        private void Resize_Buttons(object sender, WindowSizeChangedEventArgs e)
        {
            foreach(LinkButton button in link_buttons)
            {
                button.Width = Window.Current.Bounds.Width;
                button.GetCaption().Width = button.Width - button.GetThumbnail().Width; //So much jank, so little time
            }
        }

        private void login_but_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void link_but_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BrowserView), new BrowserViewParameters(((LinkButton)sender).link, requester.access_token, current_subreddit, logged_in));
        }
    }
}
