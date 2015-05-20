using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Framework.DependencyInjection;

namespace EF7Issue
{
    public static class ArticlesServicesBuilderExtensions
    {
        public static ArticlesServicesBuilder AddSql(this ArticlesServicesBuilder builder, 
                                                     string connectionString)
        {
            var services = builder.Services;
            var entityFrameworkServicesBuilder = new EntityFrameworkServicesBuilder(services);
            entityFrameworkServicesBuilder
                // .AddSqlServer()  // 2 calls to this causes problems
                .AddDbContext<ArticleDbContext>(options => options.UseSqlServer(connectionString));

            AddServices(services);

            return builder;
        }

        private static void AddServices(IServiceCollection services)
        {
            // This is where we would add implementation dependent services.
            services.TryAdd(new ServiceCollection()
                            .AddScoped<IArticleQuery, ArticleQuery>()
            );

        }
    }
}
