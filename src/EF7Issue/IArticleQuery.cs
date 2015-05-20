using System.Collections.Generic;
using System.Threading.Tasks;

namespace EF7Issue
{
    public interface IArticleQuery
    {
        /// <summary>
        /// Gets the latest active and available articles.
        /// </summary>
        /// <param name="page">The page of articles. null => 1</param>
        /// <param name="pageSize">The number of articles per page. null => ArticlesPerPage.</param>
        /// <returns>A list of articles with paging information.</returns>
        Task<IEnumerable<Article>> GetLatest(int? page = null, int? pageSize = null);

        /// <summary>
        /// Gets the latest active and available articles.
        /// </summary>
        /// <param name="page">The page of articles. null => 1</param>
        /// <param name="pageSize">The number of articles per page. null => ArticlesPerPage.</param>
        /// <returns>A list of articles with paging information.</returns>
        Task<IEnumerable<Article>> GetLatest2(int? page = null, int? pageSize = null);

        /// <summary>
        /// Queries for a specific Article to be displayed.
        /// </summary>
        /// <param name="articleId">The Id of the article to be returned.</param>
        /// <returns>The currently Available version of the Article, or null if not available or does not exist.</returns>
        Task<Article> GetAsync(int articleId);
    }
}
