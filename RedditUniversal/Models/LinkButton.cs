using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace RedditUniversal.Models
{
    class LinkButton : HyperlinkButton
    {
        Link link { get; set; }

        public LinkButton(Link link)
        {
            this.link = link;

            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.Width = Window.Current.Bounds.Width;

            StackPanel button_content = new StackPanel();
            button_content.Orientation = Orientation.Horizontal;
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            Image thumbnail = new Image();
            thumbnail.Width = 70;
            thumbnail.Height = 70;

            Uri thumb;
            if (Uri.TryCreate(link.thumbnail, UriKind.Absolute, out thumb))
            {
                BitmapImage myBitmapImage = new BitmapImage(thumb);
                thumbnail.Source = myBitmapImage;
            }

            button_content.Children.Add(thumbnail);

            TextBlock caption = new TextBlock();
            caption.Text = link.title;
            caption.TextWrapping = TextWrapping.WrapWholeWords;
            caption.Width = Window.Current.Bounds.Width - 70;

            button_content.Children.Add(caption);

            this.NavigateUri = new Uri(link.url);
            this.Content = button_content;
            this.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(192, 192, 192, 255));
            this.BorderThickness = new Thickness(1);
        }
    }
}
