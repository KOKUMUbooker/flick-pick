using Microsoft.EntityFrameworkCore;

namespace WatchHive.Models;

public partial class WatchHiveDbContext : DbContext {
    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        var now = DateTimeOffset.UtcNow;

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = RoleConstants.AdminRoleId,
                RoleValue = RoleEnum.Admin,
                Created = now,
                LastModified = now
            },
            new Role
            {
                Id = RoleConstants.UserRoleId,
                RoleValue = RoleEnum.User,
                Created = now,
                LastModified = now
            }
        );
    }

    private static void SeedAdminUser(ModelBuilder modelBuilder)
    {
        var now = DateTimeOffset.UtcNow;

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Email = "admin@sys.com",
                FullName = "System Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                RoleId = RoleConstants.AdminRoleId,
                Created = now,
                LastModified = now,
                EmailVerified = true
            }
        );
    }

    private static void SeedDemoUsers(ModelBuilder modelBuilder)
    {
        var now = DateTimeOffset.UtcNow;

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Email = "john@app.com",
                FullName = "John Doe",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass@123"),
                RoleId = RoleConstants.UserRoleId,
                Created = now,
                LastModified = now,
                EmailVerified = true
            }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Email = "jane@app.com",
                FullName = "Jane Doe",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass@123"),
                RoleId = RoleConstants.UserRoleId,
                Created = now,
                LastModified = now,
                EmailVerified = true
            }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Email = "po@app.com",
                FullName = "Dragon Warrior",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass@123"),
                RoleId = RoleConstants.UserRoleId,
                Created = now,
                LastModified = now,
                EmailVerified = true
            }
        );
    }
}
