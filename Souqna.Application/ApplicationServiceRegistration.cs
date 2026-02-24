using Microsoft.Extensions.DependencyInjection;

namespace Souqna.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register AutoMapper - scans for all mapping profiles in this assembly
            services.AddAutoMapper(typeof(ApplicationServiceRegistration).Assembly);

            return services;
        }
    }
}
