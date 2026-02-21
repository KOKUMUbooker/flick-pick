using Microsoft.EntityFrameworkCore;

namespace WatchHive.Models;

public partial class WatchHiveDbContext : DbContext { 
    private static void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // User indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.RoleId);

        // RefreshTokens indexes
        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r => r.UserId);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r => r.ClientId);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r => r.Expires);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r => r.IsRevoked);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r => new { r.UserId, r.IsRevoked, r.Expires });

        modelBuilder.Entity<Vote>()
            .HasIndex(v => new { v.UserId, v.MovieSuggestionId })
            .IsUnique(); // Ensure one vote on a MovieSuggestion per user

        modelBuilder.Entity<MovieNightRating>()
            .HasIndex(r => new { r.UserId, r.MovieNightEventId })
            .IsUnique();

        modelBuilder.Entity<UserGroup>()
            .HasIndex(ug => new { ug.UserId, ug.GroupId })
            .IsUnique();
    }
}