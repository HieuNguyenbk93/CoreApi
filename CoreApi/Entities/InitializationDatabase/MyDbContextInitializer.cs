using Microsoft.AspNetCore.Identity;

namespace CoreApi.Entities.InitializationDatabase
{
    public class MyDbContextInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<MyDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            if (dbContext.Set<ApplicationRole>().Any()) return;
            // Create a default role
            var adminRole = new ApplicationRole();
            adminRole.Name = "admin";
            adminRole.Title = "Quản trị hệ thống";
            adminRole.IsVisible = true;
            await roleManager.CreateAsync(adminRole);
            var userRole = new ApplicationRole();
            userRole.Name = "user";
            userRole.Title = "Người dùng";
            userRole.IsVisible = true;
            await roleManager.CreateAsync(userRole);

            if (dbContext.Set<ApplicationUser>().Any()) return;

            // Create a default user

            var adminUser = new ApplicationUser();
            adminUser.FullName = "Admin";
            adminUser.UserName = "admin";
            adminUser.Email = "admin@example.com";
            adminUser.IsVisible = true;
            await userManager.CreateAsync(adminUser, "123456");
            var userUser = new ApplicationUser();
            userUser.FullName = "End user";
            userUser.UserName = "user";
            userUser.Email = "user@example.com";
            userUser.IsVisible = true;
            await userManager.CreateAsync(userUser, "123456");

            // Assign the default user to the default role
            await userManager.AddToRoleAsync(adminUser, adminRole.Name);
            await userManager.AddToRoleAsync(userUser, userRole.Name);
        }
    }
}
