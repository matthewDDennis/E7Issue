using Microsoft.Framework.DependencyInjection;

namespace EF7Issue
{
    public class ArticlesServicesBuilder 
    {
        public ArticlesServicesBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
