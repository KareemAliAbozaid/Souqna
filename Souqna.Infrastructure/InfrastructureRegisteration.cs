using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Souqna.Application;
using Souqna.Application.Interfaces.Repositories;
using Souqna.Application.Interfaces.Services;
using Souqna.Infrastructure.Repositories;
using Souqna.Infrastructure.Repositories.Service;

namespace Souqna.Infrastructure
{
    public static class InfrastructureRegisteration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Application services
            services.AddApplicationServices();

            // Add Repository implementations
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepositories<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add DbContext
            services.AddDbContext<Data.ApplicationDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });

            // Add Image Management Service
            services.AddSingleton<IImageManagementService, ImageManagementService>();

            // Add File Provider
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(wwwrootPath));

            return services;
        }
    }
}
