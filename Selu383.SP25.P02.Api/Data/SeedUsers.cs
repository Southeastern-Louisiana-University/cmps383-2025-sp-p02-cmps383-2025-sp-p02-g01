using Microsoft.AspNetCore.Identity;
using Selu383.SP25.P02.Api.Features.Users;
using Selu383.SP25.P02.Api.Features.Theaters.Roles;

namespace Selu383.SP25.P02.Api.Data
{
    public static class SeedUsers
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            // Creates roles if they don't exist
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new Role { Name = "Admin" });

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new Role { Name = "User" });

            // Creates user with default password 
            string defaultPassword = "Password123!";

            // Creates user (bob)
            if (await userManager.FindByNameAsync("bob") == null)
            {
                var bob = new User { UserName = "bob" };
                var result = await userManager.CreateAsync(bob, defaultPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(bob, "User");
                }
            }

            // Creates user (sue)
            if (await userManager.FindByNameAsync("sue") == null)
            {
                var sue = new User { UserName = "sue" };
                var result = await userManager.CreateAsync(sue, defaultPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(sue, "User");
                }
            }

            // Creates admin (galkadi)
            if (await userManager.FindByNameAsync("galkadi") == null)
            {
                var galkadi = new User { UserName = "galkadi" };
                var result = await userManager.CreateAsync(galkadi, defaultPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(galkadi, "Admin");
                }
            }
        }
    }
}