using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Souqna.Domin.Interfaces;
using Souqna.Domin.Services;
using Souqna.Infrastructure.Repositories;
using Souqna.Infrastructure.Repositories.Service;

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
            // 
            services.AddSingleton<IImagemanagmentService, ImagemanagmentService>();
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(wwwrootPath)
            );

            // Register AutoMapper
            services.AddAutoMapper(cfg => { /* configuration */ }, AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}
