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
using Newtonsoft.Json;
using RedditUniversal.Utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RedditUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListingDisplay : Page
    {
        string access_token = "";

        public ListingDisplay()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            access_token = (string)e.Parameter;
        }

        private async void getListings_Click(object sender, RoutedEventArgs e)
        { 
            RestClient listings_request = new RestClient("subreddits/mine/subscriber/", access_token);
            string result = await listings_request.MakeRequest("limit=1");
            JsonTextReader reader = new JsonTextReader(new StringReader(result));
            Dictionary<string, string> subreddits = new Dictionary<string, string>();
            while(reader.Read())
            {
                if(reader.Value != null && reader.Value.Equals("id"))
                {
                    reader.Read();
                    string id = (string)reader.Value;

                    while (!reader.Value.Equals("display_name"))
                    {
                        reader.Read();
                    }

                    reader.Read();
                    string display_name = (string)reader.Value;
                    subreddits.Add(display_name, id);
                }
            }
        }
    }
}
