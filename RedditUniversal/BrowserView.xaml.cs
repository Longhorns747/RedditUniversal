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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RedditUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserView : Page
    {
        string access_token = "";

        public BrowserView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string url = ((string)e.Parameter).Split(',')[0];
            access_token = ((string)e.Parameter).Split(',')[1];

            if (url.Contains("imgur"))
                url += ".jpg";

            webViewer.Navigate(new Uri(url));
        }

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListingDisplay), access_token);
        }
    }
}
