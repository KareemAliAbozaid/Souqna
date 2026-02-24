using Microsoft.Extensions.DependencyInjection;

namespace Souqna.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ApplicationServiceRegistration).Assembly);

            return services;
        }
    }
}
