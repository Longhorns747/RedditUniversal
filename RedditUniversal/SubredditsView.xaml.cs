using RedditUniversal.DataModels;
using RedditUniversal.Utils;
using RedditUniversal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RedditUniversal
{
    /// <summary>
    /// A page to display a logged-in user's subreddits
    /// </summary>
    public sealed partial class SubredditsView : Page
    {
        RedditRequester requester;
        State current_state;
        int num_subreddits = 0;
        List<SubredditButton> subreddit_buttons;

        public SubredditsView()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(this.Resize_Buttons);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            current_state = (State)(e.Parameter);
            requester = await RedditRequester.MakeRedditRequester(current_state);
            subreddit_buttons = new List<SubredditButton>();

            current_state = await requester.RefreshToken();
            await GetSubreddits();

            progress_ring.Visibility = Visibility.Collapsed;
            progress_ring.IsActive = false;
        }

        /// <summary>
        /// Returns the user's subscribed subreddits
        /// </summary>
        /// <returns></returns>
        private async Task GetSubreddits()
        {
            await GetSubreddits("");
        }

        private async Task GetSubreddits(string parameters)
        {
            Tuple<List<Subreddit>, string> result = await requester.GetSubreddits("count=" + subreddit_buttons.Count() + "&" + parameters);
            List<Subreddit> subreddits = result.Item1;
            string after = result.Item2;

            foreach (Subreddit subreddit in subreddits)
            {
                SubredditButton curr_button = new SubredditButton(subreddit);
                curr_button.Click += new RoutedEventHandler(subreddit_but_Click);
                subreddit_buttons.Add(curr_button);
            }

            if (subreddit_buttons.Distinct().Count().Equals(subreddit_buttons.Count()))
            {
                await GetSubreddits("after=" + after);
            }
            else
            {
                subreddit_buttons = subreddit_buttons.Distinct().ToList();
                AddSubredditsToUI(subreddit_buttons);
            }            
        }

        /// <summary>
        /// Handler for each subreddit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subreddit_but_Click(object sender, RoutedEventArgs e)
        {
            current_state.current_subreddit = ((SubredditButton)sender).subreddit;
            this.Frame.Navigate(typeof(LinksDisplay), current_state);
        }

        private void AddSubredditsToUI(List<SubredditButton> subreddits)
        {
            foreach (SubredditButton subreddit in subreddits)
            {

                Grid.SetRow(subreddit, num_subreddits);
                num_subreddits++;                

                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;

                SubredditPanel.RowDefinitions.Add(row);
                SubredditPanel.Children.Add(subreddit);
            }
        }

        private void links_but_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LinksDisplay), current_state);
        }

        /// <summary>
        /// Resizes buttons based on window size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Resize_Buttons(object sender, WindowSizeChangedEventArgs e)
        {
            foreach (SubredditButton button in subreddit_buttons)
            {
                button.resize_button();
            }
        }
    }
}
