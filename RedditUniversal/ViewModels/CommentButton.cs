using RedditUniversal.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace RedditUniversal.ViewModels
{
    class CommentButton : Button
    {
        public Comment comment { get; set; }
        public int depth;
        public const int CHILD_PADDING = 10;

        public CommentButton(Comment comment, int depth)
        {
            this.comment = comment;
            this.depth = depth;

            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.Width = Window.Current.Bounds.Width;
            this.Padding = new Thickness(CHILD_PADDING * depth, 0, 0, 0);

            StackPanel button_content = new StackPanel();
            button_content.Orientation = Orientation.Horizontal;
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            //Set up caption
            TextBlock caption = new TextBlock();
            caption.Text = (comment.body != null) ? comment.body : comment.body_html;
            caption.TextWrapping = TextWrapping.WrapWholeWords;
            button_content.Children.Add(caption);

            this.Content = button_content;
            this.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(100, 100, 100, 255));
            this.BorderThickness = new Thickness(1);
        }
    }
}
