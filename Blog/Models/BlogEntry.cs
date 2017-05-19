using System;

namespace Blog.Models
{
    public class BlogEntry
    {
        public int Id { get; set; }

        public string Body { get; set; }

        public string Author { get; set; }

        public string Title { get; set; }

        public DateTime Published { get; set; }
    }
}