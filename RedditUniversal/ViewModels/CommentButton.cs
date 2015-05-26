using RedditUniversal.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Data.Html;

namespace RedditUniversal.ViewModels
{
    /// <summary>
    /// A button for a comment
    /// </summary>
    class CommentButton : Button
    {
        public Comment comment { get; set; }
        public int depth;
        public const int CHILD_PADDING = 10;
        public Uri url;

        public CommentButton(Comment comment, int depth)
        {
            this.comment = comment;
            this.depth = depth;
            url = new Uri("http://www.reddit.com/r/" + comment.subreddit + "/comments/" + comment.link_id.Substring(3) + "?comment=" + comment.id);

            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.Width = Window.Current.Bounds.Width - CHILD_PADDING * depth;
            this.Margin = new Thickness(CHILD_PADDING * depth, 0, 0, 0);

            StackPanel button_content = new StackPanel();
            button_content.Orientation = Orientation.Vertical;
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            //Set up author bar
            StackPanel author_bar = new StackPanel();
            author_bar.Orientation = Orientation.Horizontal;

            TextBlock author = new TextBlock();
            author.Text = comment.author;
            author.FontSize = 12;
            author.Foreground = Colors.AuthorTextColor;
            author_bar.Children.Add(author);

            TextBlock score = new TextBlock();
            score.Text = (comment.score >= 0) ? "+" + comment.score : "-" + comment.score;
            score.FontSize = 12;
            score.Padding = new Thickness(2, 0, 0, 0);
            score.Foreground = (comment.score >= 0) ? Colors.UpvoteColor : Colors.DownvoteColor;
            score.HorizontalAlignment = HorizontalAlignment.Right;
            author_bar.Children.Add(score);

            button_content.Children.Add(author_bar);

            //Set up caption
            TextBlock caption = new TextBlock();
            caption.Text = comment.body;
            caption.TextWrapping = TextWrapping.WrapWholeWords;
            caption.Foreground = Colors.TextColor;
            caption.Width = Window.Current.Bounds.Width - CHILD_PADDING * (depth + 2);
            button_content.Children.Add(caption);

            this.Content = button_content;
            this.BorderBrush = Colors.BorderColor;
            this.BorderThickness = new Thickness(1);
        }

        public TextBlock GetCaption()
        {
            return ((TextBlock)((StackPanel)this.Content).Children.Last());
        }

        public int GetChildPadding()
        {
            return CHILD_PADDING;
        }
    }
}
