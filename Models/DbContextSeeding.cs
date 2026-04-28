using Microsoft.AspNetCore.Mvc;
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

    private static void SeedAdminUser(ModelBuilder modelBuilder, IConfiguration configuration)
    {
        var now = DateTimeOffset.UtcNow;

        var adminEmail = configuration.GetValue<string>("ADMIN_EMAIL") ?? throw new Exception("ADMIN_EMAIL not configured");
        var adminFullName = configuration.GetValue<string>("ADMIN_FULLNAME") ?? throw new Exception("ADMIN_FULLNAME not configured");
        var adminPasswd = configuration.GetValue<string>("ADMIN_PASS") ?? throw new Exception("ADMIN_PASS not configured");
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Email = adminEmail,
                FullName = adminFullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPasswd),
                RoleId = RoleConstants.AdminRoleId,
                Created = now,
                LastModified = now,
                EmailVerified = true
            }
        );
    }

    private static void SeedDemoUsers(ModelBuilder modelBuilder,IConfiguration configuration)
    {
        var now = DateTimeOffset.UtcNow;

        var user1Email = configuration.GetValue<string>("USER1_EMAIL");
        var user1FullName = configuration.GetValue<string>("USER1_FULLNAME");
        var user1Passwd = configuration.GetValue<string>("USER1_PASS");
        if(!string.IsNullOrEmpty(user1Email) && !string.IsNullOrEmpty(user1FullName) && !string.IsNullOrEmpty(user1Passwd))
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Email = user1Email,
                    FullName = user1FullName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(user1Passwd),
                    RoleId = RoleConstants.UserRoleId,
                    Created = now,
                    LastModified = now,
                    EmailVerified = true
                }
            );
        }

        var user2Email = configuration.GetValue<string>("USER2_EMAIL");
        var user2FullName = configuration.GetValue<string>("USER2_FULLNAME");
        var user2Passwd = configuration.GetValue<string>("USER2_PASS");
        if(!string.IsNullOrEmpty(user2Email) && !string.IsNullOrEmpty(user2FullName) && !string.IsNullOrEmpty(user2Passwd))
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                    Email = user2Email,
                    FullName = user2FullName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(user2Passwd),
                    RoleId = RoleConstants.UserRoleId,
                    Created = now,
                    LastModified = now,
                    EmailVerified = true
                }
            );
        }

        var user3Email = configuration.GetValue<string>("USER3_EMAIL");
        var user3FullName = configuration.GetValue<string>("USER3_FULLNAME");
        var user3Passwd = configuration.GetValue<string>("USER3_PASS");
        if(!string.IsNullOrEmpty(user3Email) && !string.IsNullOrEmpty(user3FullName) && !string.IsNullOrEmpty(user3Passwd))
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    Email = user3Email,
                    FullName = user3FullName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(user3Passwd),
                    RoleId = RoleConstants.UserRoleId,
                    Created = now,
                    LastModified = now,
                    EmailVerified = true
                }
            );
        }
    }
}
