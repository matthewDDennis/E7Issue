using System;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Caching.Memory;
using System.Collections.Generic;

namespace EF7Issue
{
    /// <summary>
    /// Implements the IArticleQuery interface using a RavenDb database store.
    /// </summary>
    /// <remarks>
    /// Because all operations are thread safe, this can be created as a singleton and 
    /// used by all. IE services.AddSingleton&lt;IArticleQuery, ArticleQuery>
    /// </remarks>
    public class ArticleQuery : IArticleQuery
    {
        const int ArticlesPerPage = 25;

        private readonly ArticleDbContext  _contentDbContext;

        public ArticleQuery(ArticleDbContext contentDbContext)
        {
            _contentDbContext  = contentDbContext;
        }

        // This method works, and includes the Author and Member associated with the Article.
        async Task<Article> IArticleQuery.GetAsync(int articleId)
        {
            Article article = await _contentDbContext.Articles.AsNoTracking()
                                    .Include(a => a.Authors)
                                    .ThenInclude(a => a.Member)
                                    .Where(a => a.Id == articleId)
                                    .SingleOrDefaultAsync()
                                    .ConfigureAwait(false);

            return article;
        }

        // This method does not have a Select and complains about a.PublishingStatusId and a.ArticleTypeId.
        public async Task<IEnumerable<Article>> GetLatest(int? page = null, int? pageSize = null)
        {
            page                           = Math.Min(1, page ?? 1);
            pageSize                       = pageSize ?? ArticlesPerPage;

            var articles = await _contentDbContext.Articles
                                .AsNoTracking()
                                .Include(a => a.Authors)
                                .ThenInclude(a => a.Member)
                                .Where(a => a.PublishingStatusId == (byte)2 &&
                                            a.ArticleTypeId == (byte)1)
                                .OrderByDescending(a => a.Updated)
                                .Skip((page.Value-1) * pageSize.Value)
                                .Take(pageSize.Value)
                                .ToListAsync()
                                .ConfigureAwait(false);

            return articles;
        }

        // This method includes a projection using a Select and almost works, but does not return
        // the included Authors and Members.
        public async Task<IEnumerable<Article>> GetLatest2(int? page = null, int? pageSize = null)
        {
            page = Math.Min(1, page ?? 1);
            pageSize = pageSize ?? ArticlesPerPage;

            var articles = await _contentDbContext.Articles
                                .AsNoTracking()
                                .Include(a => a.Authors)
                                .ThenInclude(a => a.Member)
                                .Where(a => a.PublishingStatusId == (byte)2 &&
                                            a.ArticleTypeId == (byte)1)
                                .OrderByDescending(a => a.Updated)
                                .Select(a => new Article
                                {
                                    ArticleTypeId      = a.ArticleTypeId,
                                    Content            = a.Content,
                                    Description        = a.Description,
                                    Id                 = a.Id,
                                    Posted             = a.Posted,
                                    PublishingStatusId = a.PublishingStatusId,
                                    Title              = a.Title,
                                    Updated            = a.Updated
                                })
                                .Skip((page.Value - 1) * pageSize.Value)
                                .Take(pageSize.Value)
                                .ToListAsync()
                                .ConfigureAwait(false);

            return articles;
        }

    }
}
