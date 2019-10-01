using System.Collections.Generic;

namespace App4.sources
{
    public class Replys
    {
        public string id { get; set; }
        public string img { get; set; }
        public string ext { get; set; }
        public string now { get; set; }
        public string userid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string sage { get; set; }
        public string admin { get; set; }
        public string status { get; set; }
    }
    public class PostContent
    {
        public string id { get; set; }
        public string fid { get; set; }
        public string img { get; set; }
        public string ext { get; set; }
        public string now { get; set; }
        public string userid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string sage { get; set; }
        public string admin { get; set; }
        public List<Replys> replys { get; set; }
        public string replyCount { get; set; }
        public string imgUri { get { return "https://nmbimg.fastmirror.org/image/"+img.Replace("\\", "") + ext; } }
        public string idFormat { get { return "No." + id; } }
        public string info { get { return now + "  ID:" + userid; } }
        public string threadContent { get{ return string.Format("<html><body>{0}</body></html>", content); } }

    }
    public class ReplysItem
    {
        public int id { get; set; }
        public string userid { get; set; }
        public int admin { get; set; }
        public string title { get; set; }
        public string email { get; set; }
        public string now { get; set; }
        public string content { get; set; }
        public string img { get; set; }
        public string ext { get; set; }
        public string name { get; set; }
        public string imgUri { get { return "https://nmbimg.fastmirror.org/image/" + img.Replace("\\", "") + ext; } }
        public string idFormat { get { return "No." + id; } }
        public string info { get { return now + "  ID:" + userid; } }
        public string threadContent { get { return string.Format("<html><body>{0}</body></html>", content); } }

    }

    public class ThreadItem
    {
        public string id { get; set; }
        public string fid { get; set; }
        public string img { get; set; }
        public string ext { get; set; }
        public string now { get; set; }
        public string userid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string sage { get; set; }
        public string admin { get; set; }
        public List<ReplysItem> replys { get; set; }
        public string replyCount { get; set; }
        public string idFormat { get { return "No." + id; } }
        public string imgUri { get { return "https://nmbimg.fastmirror.org/image/" + img.Replace("\\", "") + ext; } }
        public string threadContent { get { return string.Format("<html><body>{0}</body></html>", content); } }
        public string info { get { return now + "  ID:" + userid; } }

    }
}
