using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Souqna.Domin.Interfaces;
using Souqna.Infrastructure.Repositories;

namespace Souqna.Infrastructure
{
    public static class InfrastructureRegisteration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add your infrastructure services here
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepositories<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //add conections string from appsettings.json
            services.AddDbContext<Data.ApplicationDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });
            return services;
        }
    }
}
