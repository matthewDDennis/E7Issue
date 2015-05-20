using System.Collections.Generic;

namespace EF7Issue
{
    public class Member
    {
        public int Id { get; set; }
        public int? UserProfileId { get; set; }
        public string Name { get; set; }
        public virtual IList<ArticleMember> Articles { get; } = new List<ArticleMember>();
    }
}