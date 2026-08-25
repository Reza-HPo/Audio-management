using Microsoft.AspNetCore.Identity;

namespace Web.Data.Seed;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager =
            services.GetRequiredService<UserManager<IdentityUser>>();

        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole>>();

        const string email = "admin@maktabahwaz.ir";
        const string password = "ChangeMe123!";

        const string role = "Admin";

        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }

        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(
                        Environment.NewLine,
                        result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}