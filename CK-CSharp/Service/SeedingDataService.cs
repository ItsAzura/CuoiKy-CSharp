using Microsoft.AspNetCore.Identity;

namespace CK_CSharp.Service
{
    public static class SeedingDataService
    {
        public static async Task<IApplicationBuilder> SeedDataAsync(this WebApplication app)
        {
            //using var scope = app.Services.CreateScope();
            //var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //var adminRole = new IdentityRole("Admin");
            //var userRole = new IdentityRole("User");

            //await roleManager.CreateAsync(adminRole);
            //await roleManager.CreateAsync(userRole);

            return app;
        }
    }
}
