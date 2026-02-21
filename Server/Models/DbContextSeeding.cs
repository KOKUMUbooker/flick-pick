using Microsoft.EntityFrameworkCore;

namespace FlickPickApp.Models;

public partial class MovieAppDbContext : DbContext {
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

    private static void SeedDefaultClient(ModelBuilder modelBuilder)
    {
        var now = DateTimeOffset.UtcNow;

        // Generate a client secret (in production, use a secure random generator)
        var clientId = "movie-manager-web";

        var clientSecret = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("MySuperSecureAndRandomKeyThatLooksJustAwesomeAndNeedsToBeVeryVeryLong!!!111oneeleven"));

        modelBuilder.Entity<Client>().HasData(
            new Client
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), // Fixed GUID
                ClientId = clientId,
                Name = "Movie Manager Web Application",
                ClientSecret = clientSecret,
                ClientURL = "https://localhost:5173",
                IsActive = true,
                Created = now,
                LastModified = now
            }
        );
    }


}