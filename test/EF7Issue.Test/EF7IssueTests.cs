using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Data.Entity;
using EF7Issue;
using Xunit;

namespace EF7Issue.Test
{
    public class EF7IssueTests
    {
        const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=MD_EF7Issue;Trusted_Connection=True;MultipleActiveResultSets=true";
        protected IServiceProvider _serviceProvider;
        protected ArticleDbContext _dbContext;

        public EF7IssueTests()
        {
            var services      = new ServiceCollection();
            services.AddEntityFramework().AddSqlServer();
            services.AddArticles().AddSql(ConnectionString);

            _serviceProvider = services.BuildServiceProvider();
            _dbContext       = _serviceProvider.GetRequiredService<ArticleDbContext>();
        }

        [Fact]
        public async void CanInitializeDatabase()
        {
            await InitializeDatabase();
            var articleCount = await _dbContext.Articles.AsNoTracking().CountAsync();
            Assert.Equal(3, articleCount);
        }

        // This test passes
        [Fact]
        public async void GetOneArticleIncludesAuthors()
        {
            await InitializeDatabase();
            IArticleQuery articleQuery = _serviceProvider.GetRequiredService<IArticleQuery>();

            var article = await articleQuery.GetAsync(1);

            Assert.NotNull(article);
            Assert.Equal("Article 1 Title", article.Title);
            Assert.NotNull(article.Authors);
            Assert.Equal(1, article.Authors.Count);
        }

        // This test fails
        [Fact]
        public async void GetLatestArticlesIncludesAuthors()
        {
            await InitializeDatabase();
            IArticleQuery articleQuery = _serviceProvider.GetRequiredService<IArticleQuery>();

            var articles = await articleQuery.GetLatest(null);

            Assert.NotNull(articles);
            Assert.Equal(3, articles.Count());

            foreach (var article in articles)
            {
                Assert.NotNull(article.Authors);
                Assert.Equal(1, article.Authors.Count);
                Assert.NotNull(article.Authors.First().Member);
            }
        }

        // This test fails
        [Fact]
        public async void GetLatest2ArticlesIncludesAuthors()
        {
            await InitializeDatabase();
            IArticleQuery articleQuery = _serviceProvider.GetRequiredService<IArticleQuery>();

            var articles = await articleQuery.GetLatest2(null);

            Assert.NotNull(articles);
            Assert.Equal(3, articles.Count());

            foreach (var article in articles)
            {
                Assert.NotNull(article.Authors);
                Assert.Equal(1, article.Authors.Count);
                Assert.NotNull(article.Authors.First().Member);
            }
        }

        // This test fails
        [Fact]
        public async void OrderOfForSqlDoesNotMatter()
        {
            await InitializeDatabase();
            for (int i = 1; i <= 3; i++)
            {
                _dbContext.BadIdenties.Add(new BadIdentity { Name = $"BadIdentity{i}" });
            }
            _dbContext.SaveChanges();

            for (int i = 1; i <= 3; i++)
            {
                var badIdentity = _dbContext.BadIdenties.AsNoTracking().SingleOrDefaultAsync(bi => bi.Id == i);
                Assert.NotNull(badIdentity);
            }

            var entityType     = _dbContext.Model.GetEntityType(typeof(BadIdentity));
            var idProperty     = entityType.GetProperty("Id");
            var relationalName = idProperty.Relational().Column;
            var sqlName        = idProperty.SqlServer().Column;

            Assert.Equal("BadIdentityId", relationalName);
            Assert.Equal("BadIdentityId", sqlName);
            var relationalColumType = idProperty.Relational().ColumnType;
            var sqlColumType        = idProperty.SqlServer().ColumnType;

        }

        private async Task InitializeDatabase()
        {
            await _dbContext.Database.EnsureDeletedAsync();
            if (await _dbContext.Database.EnsureCreatedAsync())
            {
                // Create Articles
                for (int i = 1; i <= 3; i++)
                {
                    var article = new Article
                    {
                        ArticleTypeId      = 1,
                        Title              = $"Article {i} Title",
                        Description        = $"I am Article {i}",
                        Content            = $"This is the Content for Article {i}",
                        Posted             = DateTime.Now.AddDays(i - 3),
                        Updated            = DateTime.Now,
                        PublishingStatusId = (byte)2
                    };
                    _dbContext.Articles.Add(article);
                }

                // Create Members
                for (int i = 1; i <= 3; i++)
                {
                    var member = new Member
                    {
                        Name = $"Member{i}",
                        UserProfileId = i
                    };
                    _dbContext.Members.Add(member);
                }
                await _dbContext.SaveChangesAsync();

                // Associate Authors with articles
                var articles = await _dbContext.Articles.ToArrayAsync();
                var members  = await _dbContext.Members.ToArrayAsync();

                for (int i = 0; i < 3; i++)
                {
                    var author = new ArticleMember
                    {
                        Article = articles[i],
                        Member = members[i]
                    };
                    _dbContext.ArticleMembers.Add(author);
                }
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
