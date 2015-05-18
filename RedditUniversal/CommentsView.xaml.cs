using RedditUniversal.ParameterModels;
using RedditUniversal.DataModels;
using RedditUniversal.Utils;
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
using RedditUniversal.ViewModels;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RedditUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CommentsView : Page
    {
        string access_token;
        bool logged_in;
        Link current_link;
        string subreddit;
        int num_comments = 0;
        List<CommentButton> comment_buttons = new List<CommentButton>();

        public CommentsView()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(this.Resize_Buttons);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            CommentViewParameters parameters = (CommentViewParameters)e.Parameter;
            this.access_token = parameters.access_token;
            this.logged_in = parameters.logged_in;
            this.current_link = parameters.current_link;
            this.subreddit = parameters.subreddit;

            RedditRequester requester = new RedditRequester(access_token);
            Tuple<List<Comment>, string> result = await requester.GetComments(current_link, "");
            List<Comment> comments = result.Item1;
            string after = result.Item2;

            AddCommentsToUI(comments, after);
        }

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BrowserView), new BrowserViewParameters(current_link, access_token, subreddit, logged_in));
        }

        private void AddCommentsToUI(List<Comment> comments, string after)
        {
            foreach(Comment comment in comments)
            {
                comment_tree_traversal(comment, 1);
            }
        }

        private void comment_tree_traversal(Comment comment, int depth)
        {
            if (comment == null)
            {
                return;
            }
            else
            {
                CommentButton curr_button = new CommentButton(comment, depth);

                Grid.SetRow(curr_button, num_comments);
                num_comments++;
                comment_buttons.Add(curr_button);

                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;

                comment_grid.RowDefinitions.Add(row);
                comment_grid.Children.Add(curr_button);

                if (comment.replies != null)
                {
                    comment.replies.data.children.Remove(comment.replies.data.children.Last());
                    foreach (CommentChild child_comment in comment.replies.data.children)
                    {
                        int next_depth = depth + 1;
                        comment_tree_traversal(child_comment.data, next_depth);
                    }
                }                
            }
        }

        private void Resize_Buttons(object sender, WindowSizeChangedEventArgs e)
        {
            foreach (CommentButton button in comment_buttons)
            {
                button.Width = Window.Current.Bounds.Width;
                button.GetCaption().Width = Window.Current.Bounds.Width;
            }
        }
    }
}
