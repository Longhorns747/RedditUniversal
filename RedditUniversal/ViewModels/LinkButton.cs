using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using RedditUniversal.DataModels;

namespace RedditUniversal.ViewModels
{
    /// <summary>
    /// A button for a link
    /// </summary>
    class LinkButton : Button
    {
        public Link link { get; set; }
        public const int THUMBNAIL_SIZE = 70;

        public LinkButton(Link link)
        {
            this.link = link;

            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.Width = Window.Current.Bounds.Width;

            StackPanel button_content = new StackPanel();
            button_content.Orientation = Orientation.Vertical;
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            //Set up author bar
            StackPanel author_bar = new StackPanel();
            author_bar.Orientation = Orientation.Horizontal;

            TextBlock author = new TextBlock();
            author.Text = link.author;
            author.FontSize = 12;
            author.Foreground = Colors.AuthorTextColor;
            author_bar.Children.Add(author);

            TextBlock subreddit = new TextBlock();
            subreddit.Text = link.subreddit;
            subreddit.FontSize = 12;
            subreddit.Foreground = Colors.NeutralColor;
            subreddit.Padding = new Thickness(2, 0, 0, 0);
            author_bar.Children.Add(subreddit);

            TextBlock score = new TextBlock();
            score.Text = link.score.ToString();
            score.FontSize = 12;
            score.Padding = new Thickness(2, 0, 0, 0);
            score.Foreground = Colors.NeutralColor;
            score.HorizontalAlignment = HorizontalAlignment.Right;
            author_bar.Children.Add(score);

            button_content.Children.Add(author_bar);

            //Set up caption bar
            StackPanel caption_bar = new StackPanel();
            caption_bar.Orientation = Orientation.Horizontal;
            caption_bar.HorizontalAlignment = HorizontalAlignment.Stretch;

            //Set up thumbnail
            Image thumbnail = new Image();
            thumbnail.Height = 1;
            thumbnail.Width = 1;
            thumbnail.VerticalAlignment = VerticalAlignment.Center;

            Uri thumb;
            if (Uri.TryCreate(link.thumbnail, UriKind.Absolute, out thumb))
            {
                BitmapImage myBitmapImage = new BitmapImage(thumb);
                thumbnail.Width = THUMBNAIL_SIZE;
                thumbnail.Height = THUMBNAIL_SIZE;
                thumbnail.Source = myBitmapImage;
            }

            caption_bar.Children.Add(thumbnail);

            //Set up caption
            TextBlock caption = new TextBlock();
            caption.Text = link.title;
            caption.TextWrapping = TextWrapping.WrapWholeWords;
            caption.Width = Window.Current.Bounds.Width - thumbnail.Width;
            caption.Padding = new Thickness(10);
            caption.VerticalAlignment = VerticalAlignment.Center;

            caption_bar.Children.Add(caption);

            button_content.Children.Add(caption_bar);

            this.Content = button_content;
            this.BorderBrush = Colors.BorderColor;
            this.BorderThickness = new Thickness(1);
        }

        public Image GetThumbnail()
        {
            return (Image)((StackPanel)((StackPanel)this.Content).Children.Last()).Children.First();
        }

        public TextBlock GetCaption()
        {
            return (TextBlock)((StackPanel)((StackPanel)this.Content).Children.Last()).Children.Last(); ;
        }

        public int GetThumbSize()
        {
            return THUMBNAIL_SIZE;
        }
    }
}
