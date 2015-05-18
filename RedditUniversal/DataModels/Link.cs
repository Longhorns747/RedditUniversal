using RedditUniversal.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditUniversal.DataModels
{
    /// <summary>
    /// Data model for a Link, generated from a JSON response to the Reddit API
    /// </summary>
    public class Link
    {
        public string domain { get; set; }
        public object banned_by { get; set; }
        public LinkMediaEmbed media_embed { get; set; }
        public string subreddit { get; set; }
        public object selftext_html { get; set; }
        public string selftext { get; set; }
        public object likes { get; set; }
        public object suggested_sort { get; set; }
        public List<object> user_reports { get; set; }
        public object secure_media { get; set; }
        public object link_flair_text { get; set; }
        public string id { get; set; }
        public int gilded { get; set; }
        public bool archived { get; set; }
        public bool clicked { get; set; }
        public object report_reasons { get; set; }
        public string author { get; set; }
        public LinkMedia media { get; set; }
        public int score { get; set; }
        public object approved_by { get; set; }
        public bool over_18 { get; set; }
        public bool hidden { get; set; }
        public string thumbnail { get; set; }
        public string subreddit_id { get; set; }
        public bool edited { get; set; }
        public object link_flair_css_class { get; set; }
        public object author_flair_css_class { get; set; }
        public int downs { get; set; }
        public List<object> mod_reports { get; set; }
        public LinkSecureMediaEmbed secure_media_embed { get; set; }
        public bool saved { get; set; }
        public object removal_reason { get; set; }
        public bool is_self { get; set; }
        public string name { get; set; }
        public string permalink { get; set; }
        public bool stickied { get; set; }
        public double created { get; set; }
        public string url { get; set; }
        public object author_flair_text { get; set; }
        public string title { get; set; }
        public double created_utc { get; set; }
        public int ups { get; set; }
        public int num_comments { get; set; }
        public bool visited { get; set; }
        public object num_reports { get; set; }
        public object distinguished { get; set; }
    }

    public class LinkMediaEmbed
    {
        public string content { get; set; }
        public int? width { get; set; }
        public bool? scrolling { get; set; }
        public int? height { get; set; }
    }

    public class Oembed
    {
        public string provider_url { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public int thumbnail_width { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string html { get; set; }
        public string version { get; set; }
        public string provider_name { get; set; }
        public string thumbnail_url { get; set; }
        public string type { get; set; }
        public int thumbnail_height { get; set; }
    }

    public class LinkMedia
    {
        public string type { get; set; }
        public Oembed oembed { get; set; }
    }

    public class LinkSecureMediaEmbed
    {
    }

    public class LinkChild
    {
        public string kind { get; set; }
        public Link data { get; set; }
    }

    public class LinkData
    {
        public string modhash { get; set; }
        public List<LinkChild> children { get; set; }
        public string after { get; set; }
        public string before { get; set; }
    }

    public class LinkTree
    {
        public string kind { get; set; }
        public LinkData data { get; set; }
    }
}
