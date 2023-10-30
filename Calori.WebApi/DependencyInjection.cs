using Calori.Application.Interfaces;
using Calori.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Calori.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection
            services, IConfiguration configuration)
        {
            //var connectionString = configuration["DbConnection"];
            var connectionStringSqlLite = configuration["DbConnectionSqlLite"];
            services.AddDbContext<CaloriDbContext>(options =>
            {
                //options.UseSqlServer(connectionString);
                options.UseSqlite(connectionStringSqlLite);
            });
            return services;
        }
    }
}
