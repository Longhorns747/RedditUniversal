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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            access_token = (string)e.Parameter;
            requester = new RedditRequester(access_token);
            //List<Subreddit> subreddits = await GetSubreddits();
            GetHot();
        }

        private async Task<List<Subreddit>> GetSubreddits()
        {
            return await requester.GetSubreddits("");
        }

        private async void GetHot()
        {
            List<Link> links = await requester.GetHot("limit=10");
            Link after = links.Last();
            links.Remove(links.Last());

            BuildUI(links);
        }

        private async void GetHot(Subreddit target)
        {
            List<Link> links = await requester.GetHot(target, "limit=10");
        }

        private void BuildUI(List<Link> links)
        {
            int i = 0;
            foreach(Link link in links)
            {
                HyperlinkButton curr_button = new HyperlinkButton();
                curr_button.HorizontalAlignment = HorizontalAlignment.Stretch;

                StackPanel button_content = new StackPanel();
                button_content.Orientation = Orientation.Horizontal;
                button_content.HorizontalAlignment = HorizontalAlignment.Stretch;

                Uri thumb;
                if(Uri.TryCreate(link.url, UriKind.Absolute, out thumb))
                {
                    BitmapImage myBitmapImage = new BitmapImage(thumb);
                    Image thumbnail = new Image();
                    thumbnail.Width = 70;
                    thumbnail.Height = 70;
                    thumbnail.Source = myBitmapImage;
                    button_content.Children.Add(thumbnail);
                }

                TextBlock caption = new TextBlock();
                caption.Text = link.title;
                caption.TextWrapping = TextWrapping.WrapWholeWords;
                caption.Width = Window.Current.Bounds.Width - 70;

                button_content.Children.Add(caption);

                curr_button.NavigateUri = new Uri(link.url);
                curr_button.Content = button_content;
                curr_button.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(192, 192, 192, 255));
                curr_button.BorderThickness = new Thickness(1);
                Grid.SetRow(curr_button, i);
                i++;

                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;

                LinkPanel.RowDefinitions.Add(row);
                LinkPanel.Children.Add(curr_button);
            }
        }
    }
}
