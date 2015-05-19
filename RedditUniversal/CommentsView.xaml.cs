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
    /// A page to view the comments of a reddit link
    /// </summary>
    public sealed partial class CommentsView : Page
    {
        State current_state;
        int num_comments = 0;
        List<CommentButton> comment_buttons = new List<CommentButton>();

        /// <summary>
        /// Adds the Window resize handler to resize all buttons when window is resized
        /// </summary>
        public CommentsView()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(this.Resize_Buttons);
        }

        /// <summary>
        /// Unpackages parameters when this page is navigated to
        /// </summary>
        /// <param name="e">Must be of type CommentViewParameters</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            current_state = (State)e.Parameter;

            RedditRequester requester = await RedditRequester.MakeRedditRequester(current_state);
            current_state = await requester.RefreshToken();

            Tuple<List<Comment>, string> result = await requester.GetComments(current_state.current_link, "");
            List<Comment> comments = result.Item1;
            string after = result.Item2;

            AddCommentsToUI(comments, after);
            progress_ring.Visibility = Visibility.Collapsed;
            progress_ring.IsActive = false;
        }

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BrowserView), current_state);
        }

        /// <summary>
        /// Adds comments to UI by traversing through the comments tree
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="after">The ID of the next comment to get for the "more" button</param>
        private void AddCommentsToUI(List<Comment> comments, string after)
        {
            foreach(Comment comment in comments)
            {
                comment_tree_traversal(comment, 0);
            }
        }

        /// <summary>
        /// Traverses through the comments tree in "pre-order" fashion. Builds the buttons and adds to the UI along the way.
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="depth">Maintain the depth of the current comment in the tree</param>
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

        /// <summary>
        /// Resizes all buttons when window is resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Resize_Buttons(object sender, WindowSizeChangedEventArgs e)
        {
            foreach (CommentButton button in comment_buttons)
            {
                button.Width = Window.Current.Bounds.Width;
                button.GetCaption().Width = Window.Current.Bounds.Width - button.depth * button.GetChildPadding();
            }
        }
    }
}
