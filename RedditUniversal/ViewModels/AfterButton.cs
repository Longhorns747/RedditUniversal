using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace RedditUniversal.ViewModels
{
    /// <summary>
    /// Class for the "more" button found at the bottom of listings
    /// </summary>
    class AfterButton : Button
    {
        public string after { get; set; }

        public AfterButton(string after)
        {
            this.after = after;
        }
    }
}
