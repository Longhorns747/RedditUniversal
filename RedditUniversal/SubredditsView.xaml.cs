using RedditUniversal.DataModels;
using RedditUniversal.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SubredditsView : Page
    {
        List<Subreddit> subreddits;
        RedditRequester requester;
        State current_state;

        public SubredditsView()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            current_state = (State)(e.Parameter);
            requester = await RedditRequester.MakeRedditRequester(current_state);

            current_state = await requester.RefreshToken();

            subreddits = await GetSubreddits();
        }

        /// <summary>
        /// Returns the user's subscribed subreddits
        /// </summary>
        /// <returns></returns>
        private async Task<List<Subreddit>> GetSubreddits()
        {
            return await requester.GetSubreddits("");
        }

        private void links_but_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LinksDisplay), current_state);
        }
    }
}
