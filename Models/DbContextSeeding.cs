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
                Id = RoleConstants.AdminRoleId,
                Email = "admin@system.com",
                FullName = "System Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                RoleId = RoleConstants.AdminRoleId,
                Created = now,
                LastModified = now
            }
        );
    }
}
