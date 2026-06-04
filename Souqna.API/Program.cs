using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Souqna.Infrastructure;
using Souqna.Application.Constants;
using Souqna.Infrastructure.Data;

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

            builder.Services.AddSwaggerGen(c =>
            {
                // Add JWT bearer definition for Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            // Add Infrastructure services (which includes Application services and Identity registration)
            builder.Services.AddInfrastructureServices(builder.Configuration);

            // Configure authentication (JWT)
            var jwtKey = builder.Configuration["Jwt:Key"];
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = System.TimeSpan.Zero
                };
            });

            // Authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CanManageProducts", policy => policy.RequireRole(Roles.Admin, Roles.Seller));
                options.AddPolicy("CanManageCategories", policy => policy.RequireRole(Roles.Admin));
                options.AddPolicy("CanManageUsers", policy => policy.RequireRole(Roles.Admin));
                options.AddPolicy("CanManageOrders", policy => policy.RequireRole(Roles.Admin, Roles.Seller));
            });
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

            // Seed roles on startup
            try
            {
                RoleSeeder.SeedRolesAsync(app.Services).GetAwaiter().GetResult();
            }
            catch
            {
                // swallow errors during seeding to avoid startup failure
            }

            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
         app.UseStaticFiles();
            app.UseMiddleware<Middleware.ExptionsMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
