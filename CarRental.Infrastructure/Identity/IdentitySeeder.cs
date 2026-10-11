using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CarRental.Infrastructure.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            {
                AppRoles.SuperAdmin,
                AppRoles.Admin,
                AppRoles.CarOwner,
                AppRoles.Employee,
                AppRoles.Customer,
                AppRoles.Driver
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole(role));

                    if (!result.Succeeded)
                    {
                        throw new Exception(
                            string.Join(
                                ", ",
                                result.Errors.Select(
                                    e => e.Description)));
                    }
                }
            }
        }

        public static async Task SeedSuperAdminAsync(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            var email =
                configuration["SuperAdmin:Email"];

            var password =
                configuration["SuperAdmin:Password"];

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception(
                    "SuperAdmin email is not configured.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception(
                    "SuperAdmin password is not configured.");
            }

            var existingUser =
                await userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                // Make sure the existing user has SuperAdmin role
                if (!await userManager.IsInRoleAsync(
                        existingUser,
                        AppRoles.SuperAdmin))
                {
                    await userManager.AddToRoleAsync(
                        existingUser,
                        AppRoles.SuperAdmin);
                }

                return;
            }

            var user = new ApplicationUser
            {
                FullName = "Super Admin",
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var result =
                await userManager.CreateAsync(
                    user,
                    password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(
                        ", ",
                        result.Errors.Select(
                            e => e.Description)));
            }

            var roleResult =
                await userManager.AddToRoleAsync(
                    user,
                    AppRoles.SuperAdmin);

            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    string.Join(
                        ", ",
                        roleResult.Errors.Select(
                            e => e.Description)));
            }
        }
    }
}