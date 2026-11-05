
using CampusPlacementManagement.Constants;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var services = scope.ServiceProvider;

            var roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();

            var configuration =
                services.GetRequiredService<IConfiguration>();

            var loggerFactory =
                services.GetRequiredService<ILoggerFactory>();

            var logger = loggerFactory.CreateLogger("DbSeeder");

            // 1. Create all application roles

            foreach (var role in UserRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult =
                        await roleManager.CreateAsync(
                            new IdentityRole(role));

                    CheckIdentityResult(roleResult);
                }
            }

            // 2. Read SuperAdmin details from configuration

            var adminEmail =
                configuration["SuperAdmin:Email"];

            var adminPassword =
                configuration["SuperAdmin:Password"];

            var adminFullName =
                configuration["SuperAdmin:FullName"]
                ?? "Platform Super Admin";

            var adminPhoneNumber =
                configuration["SuperAdmin:PhoneNumber"];

            // Skip admin creation if credentials are not configured

            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword))
            {
                logger.LogWarning(
                    "SuperAdmin credentials are not configured. " +
                    "Application roles were seeded, but SuperAdmin was not created.");

                return;
            }

            // 3. Check whether SuperAdmin already exists

            var adminUser =
                await userManager.FindByEmailAsync(adminEmail);

            // 4. Create SuperAdmin if it does not exist

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = adminFullName,
                    PhoneNumber = adminPhoneNumber,
                    AccountStatus = AccountStatus.Approved,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var userResult =
                    await userManager.CreateAsync(
                        adminUser,
                        adminPassword);

                CheckIdentityResult(userResult);
            }

            // 5. Assign SuperAdmin role

            if (!await userManager.IsInRoleAsync(
                adminUser,
                UserRoles.SuperAdmin))
            {
                var roleAssignmentResult =
                    await userManager.AddToRoleAsync(
                        adminUser,
                        UserRoles.SuperAdmin);

                CheckIdentityResult(roleAssignmentResult);
            }

            logger.LogInformation(
                "SuperAdmin seeding completed successfully.");
        }

        private static void CheckIdentityResult(
            IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Identity seeding failed: {errors}");
            }
        }
    }
}
