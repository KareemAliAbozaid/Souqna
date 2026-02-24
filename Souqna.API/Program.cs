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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
             
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStatusCodePagesWithReExecute("/erroes/{0}");
            app.UseHttpsRedirection();
            app.UseMiddleware<Middleware.ExptionsMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
