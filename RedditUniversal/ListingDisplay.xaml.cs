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
using RedditUniversal.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RedditUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListingDisplay : Page
    {
        string access_token = "";
        RedditRequester requester;

        public ListingDisplay()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            access_token = (string)e.Parameter;
            requester = new RedditRequester(access_token);
            List<Subreddit> subreddits = await GetSubreddits();
            GetHot();
        }

        private async Task<List<Subreddit>> GetSubreddits()
        {
            return await requester.GetSubreddits("");
        }

        private async void GetHot()
        {
            List<Link> links = await requester.GetHot("");
            BuildUI(links);
        }

        private async void GetHot(Subreddit target)
        {
            List<Link> links = await requester.GetHot(target, "");
        }

        private void BuildUI(List<Link> links)
        {

        }
    }
}
