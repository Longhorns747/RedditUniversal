using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace RedditUniversal.ViewModels
{
    class Colors
    {
        public static SolidColorBrush AuthorTextColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 51, 102, 153));
        public static SolidColorBrush BorderColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 100, 100, 100));
        public static SolidColorBrush TextColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
        public static SolidColorBrush UpvoteColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 139, 96));
        public static SolidColorBrush DownvoteColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 148, 148, 255));
        public static SolidColorBrush NeutralColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 198, 198, 198));

    }
}
