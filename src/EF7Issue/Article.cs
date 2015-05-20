using System;
using System.Collections.Generic;

namespace EF7Issue
{ 
    public class Article
    {
        public int Id { get; set; }
        public byte ArticleTypeId { get; set; }
        public byte PublishingStatusId { get; set; }
        public DateTime? Posted { get; set; }
        public DateTime? Updated { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public virtual IList<ArticleMember> Authors { get; }  = new List<ArticleMember>();
    }
}