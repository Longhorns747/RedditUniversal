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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Core;

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
        List<LinkButton> link_buttons = new List<LinkButton>();
        List<Subreddit> subreddits;

        public ListingDisplay()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            requester = new RedditRequester((string)e.Parameter);
            if (!(await requester.RetrieveAccessToken()))
            {
                subreddits = await GetSubreddits();
            }           
            GetHot();
        }

        private async Task<List<Subreddit>> GetSubreddits()
        {
            return await requester.GetSubreddits("");
        }

        private async void GetHot()
        {
            List<Link> links = await requester.GetHot("");
            Link after = links.Last();
            links.Remove(links.Last());

            BuildUI(links);
        }

        private async void GetHot(Subreddit target)
        {
            List<Link> links = await requester.GetHot(target, "");
        }

        private void BuildUI(List<Link> links)
        {
            int i = 0;
            foreach(Link link in links)
            {
                LinkButton curr_button = new LinkButton(link);
                Grid.SetRow(curr_button, i);
                i++;
                link_buttons.Add(curr_button);

                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;

                LinkPanel.RowDefinitions.Add(row);
                LinkPanel.Children.Add(curr_button);
            }

            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(this.Resize_Buttons);
        }

        private void Resize_Buttons(object sender, WindowSizeChangedEventArgs e)
        {
            foreach(LinkButton button in link_buttons)
            {
                button.Width = Window.Current.Bounds.Width;
                button.GetCaption().Width = button.Width - button.GetThumbnail().Width; //So much jank, so little time

            }
        }
    }
}
