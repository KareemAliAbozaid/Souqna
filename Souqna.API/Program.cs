using Microsoft.Extensions.DependencyInjection;
using Souqna.Infrastructure;

namespace Souqna.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddMemoryCache();
            
            builder.Services.AddSwaggerGen();

            // Add Infrastructure services (which includes Application services)
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();

                    if (builder.Environment.IsDevelopment())
                    {
                        // Any localhost port (ng serve, VS, IIS Express, etc.)
                        policy.SetIsOriginAllowed(origin =>
                        {
                            if (string.IsNullOrEmpty(origin))
                                return false;

                            var uri = new Uri(origin);
                            return uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                                || uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase);
                        });
                    }
                    else
                    {
                        policy.WithOrigins(
                            "http://localhost:4200",
                            "http://localhost:54044",
                            "https://localhost:4200"
                        );
                    }
                });
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
         app.UseStaticFiles();
            app.UseMiddleware<Middleware.ExptionsMiddleware>();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
