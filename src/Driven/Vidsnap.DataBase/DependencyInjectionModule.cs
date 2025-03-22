using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Vidsnap.DataBase.Repositories;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.DataBase
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionModule
    {
        public static IServiceCollection AddAdapterDatabaseRepositories(this IServiceCollection services)
        {
            services.AddTransient<IVideoRepository, VideoRepository>();

            return services;
        }
    }
}