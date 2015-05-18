using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditUniversal.DataModels
{
    public class Comment
    {
        public string domain { get; set; }
        public object banned_by { get; set; }
        public CommentMediaEmbed media_embed { get; set; }
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
        public object media { get; set; }
        public int score { get; set; }
        public object approved_by { get; set; }
        public bool over_18 { get; set; }
        public bool hidden { get; set; }
        public int num_comments { get; set; }
        public string thumbnail { get; set; }
        public string subreddit_id { get; set; }
        public object edited { get; set; }
        public object link_flair_css_class { get; set; }
        public object author_flair_css_class { get; set; }
        public int downs { get; set; }
        public CommentSecureMediaEmbed secure_media_embed { get; set; }
        public bool saved { get; set; }
        public object removal_reason { get; set; }
        public bool stickied { get; set; }
        public bool is_self { get; set; }
        public string permalink { get; set; }
        public string name { get; set; }
        public double created { get; set; }
        public string url { get; set; }
        public object author_flair_text { get; set; }
        public string title { get; set; }
        public double created_utc { get; set; }
        public object distinguished { get; set; }
        public double upvote_ratio { get; set; }
        public List<object> mod_reports { get; set; }
        public bool visited { get; set; }
        public object num_reports { get; set; }
        public int ups { get; set; }
        public string link_id { get; set; }
        public CommentTree replies { get; set; }
        public string parent_id { get; set; }
        public int? controversiality { get; set; }
        public string body { get; set; }
        public string body_html { get; set; }
        public bool? score_hidden { get; set; }
    }

    public class CommentMediaEmbed
    {
    }

    public class CommentSecureMediaEmbed
    {
    }

    public class CommentChild
    {
        public string kind { get; set; }
        public Comment data { get; set; }
    }

    public class CommentData
    {
        public string modhash { get; set; }
        public List<CommentChild> children { get; set; }
        public string after { get; set; }
        public string before { get; set; }
    }

    public class CommentTree
    {
        public string kind { get; set; }
        public CommentData data { get; set; }
    }
}
