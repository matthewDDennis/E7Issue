using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;

namespace EF7Issue
{
    public class ArticleDbContext : DbContext
    {
        public DbSet<Article>          Articles        { get; set; }
        public DbSet<Member>           Members         { get; set; }
        public DbSet<ArticleMember>    ArticleMembers  { get; set; }

        public ArticleDbContext(DbContextOptions<ArticleDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(b =>
            {
                b.Table("Article");
                b.Key(a => a.Id);
                b.Property(a => a.Id).Column("ArticleId").ForSqlServer().UseIdentity();
            });

            modelBuilder.Entity<Member>(b =>
            {
                b.Table("Member");
                b.Key(m => m.Id);
                b.Property(m => m.Id).Column("MemberId").ForSqlServer().UseIdentity();
                b.Property(m => m.UserProfileId).Column("NetworkUserProfileId");
                b.Property(m => m.Name).Column("Name");
            });

            modelBuilder.Entity<ArticleMember>(b =>
            {
                b.Table("Article_Member");
                b.Key(am => new { am.ArticleId, am.MemberId });
                b.Property(am => am.ArticleId).Column("ArticleId").ForSqlServer().UseNoValueGeneration();
                b.Property(am => am.MemberId).Column("MemberId").ForSqlServer().UseNoValueGeneration();
                b.Reference(am => am.Article).InverseCollection(a => a.Authors)
                                              .ForeignKey(am => am.ArticleId)
                                              .PrincipalKey(a => a.Id);
                b.Reference(am => am.Member).InverseCollection(m => m.Articles)
                                            .ForeignKey(am => am.MemberId)
                                            .PrincipalKey(m => m.Id);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}