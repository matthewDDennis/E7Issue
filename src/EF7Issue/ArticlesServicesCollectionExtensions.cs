using Microsoft.Framework.DependencyInjection;

namespace EF7Issue
{
    public static class ArticlesServiceCollectionsExtensions
    {
        public static ArticlesServicesBuilder AddArticles(this IServiceCollection services)
        {
            // This is where we would add implementation independent services.

            return new ArticlesServicesBuilder(services);
        }
    }
}
