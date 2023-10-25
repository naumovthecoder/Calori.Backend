using Calori.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Calori.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection
            services, IConfiguration configuration)
        {
            var connectionString = configuration["DbConnection"];
            services.AddDbContext<CaloriDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            services.AddScoped<ICaloriDbContext>(provider => provider.GetService<CaloriDbContext>());
            return services;
        }
    }
}
