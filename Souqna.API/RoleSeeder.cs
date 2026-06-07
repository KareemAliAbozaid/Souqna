using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Souqna.Application.Constants;
using System;
using System.Threading.Tasks;

namespace Souqna.API
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { Roles.Admin, Roles.Seller, Roles.Customer };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
