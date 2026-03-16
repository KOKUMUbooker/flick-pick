namespace WatchHive.Models;

public static class DbInitializer
{
    public static async Task SeedAsync(WatchHiveDbContext context)
    {
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role
                {
                    Id = RoleConstants.AdminRoleId,
                    RoleValue = RoleEnum.Admin,
                    Created = DateTimeOffset.UtcNow,
                    LastModified = DateTimeOffset.UtcNow
                },
                new Role
                {
                    Id = RoleConstants.UserRoleId,
                    RoleValue = RoleEnum.User,
                    Created = DateTimeOffset.UtcNow,
                    LastModified = DateTimeOffset.UtcNow
                }
            );

            await context.SaveChangesAsync();
        }

        if (!context.Users.Any(u => u.Email == "admin@sys.com"))
        {
            var admin = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Email = "admin@sys.com",
                FullName = "System Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                RoleId = RoleConstants.AdminRoleId,
                Created = DateTimeOffset.UtcNow,
                LastModified = DateTimeOffset.UtcNow,
                EmailVerified = true
            };

            context.Users.Add(admin);

            await context.SaveChangesAsync();
        }

        if (!context.Users.Any(u => u.Email == "john@app.com"))
        {
            var user1 = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Email = "john@app.com",
                FullName = "John Doe",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass@123"),
                RoleId = RoleConstants.UserRoleId,
                Created = DateTimeOffset.UtcNow,
                LastModified = DateTimeOffset.UtcNow,
                EmailVerified = true
            };

            context.Users.Add(user1);

            await context.SaveChangesAsync();
        }
        if (!context.Users.Any(u => u.Email == "jane@app.com"))
        {
            var user2 = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Email = "jane@app.com",
                FullName = "Jane Doe",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass@123"),
                RoleId = RoleConstants.UserRoleId,
                Created = DateTimeOffset.UtcNow,
                LastModified = DateTimeOffset.UtcNow,
                EmailVerified = true
            };

            context.Users.Add(user2);

            await context.SaveChangesAsync();
        }

        if (!context.Users.Any(u => u.Email == "po@app.com"))
        {
            var user3 = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Email = "po@app.com",
                FullName = "Dragon Warrior",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass@123"),
                RoleId = RoleConstants.UserRoleId,
                Created = DateTimeOffset.UtcNow,
                LastModified = DateTimeOffset.UtcNow,
                EmailVerified = true
            };

            context.Users.Add(user3);

            await context.SaveChangesAsync();
        }
    }
}