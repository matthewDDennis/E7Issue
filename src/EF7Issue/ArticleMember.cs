using System;

namespace EF7Issue
{
    public class ArticleMember
    {
        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
        public int MemberId { get; set; }
        public virtual Member Member { get; set; }
        public bool Verified { get; set; }
    }
}