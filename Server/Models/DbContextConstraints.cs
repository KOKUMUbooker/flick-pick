using Microsoft.EntityFrameworkCore;

namespace FlickPickApp.Models;

public partial class MovieAppDbContext : DbContext {
        private static void ConfigureEntityConstraints(ModelBuilder modelBuilder)
    {
        // USER constraints
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<User>()
            .Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);

        // CLIENT constraints
        modelBuilder.Entity<Client>(entity =>
        {
            entity.Property(c => c.ClientId)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(c => c.ClientId)
                .IsUnique(); // ClientId should be unique

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.ClientSecret)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(c => c.ClientURL)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(c => c.IsActive)
                .HasDefaultValue(true);
        });

        // REFRESH TOKENS constraints
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasIndex(rt => rt.Token)
                .IsUnique(); // Token should be unique

            entity.Property(rt => rt.JwtId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(rt => rt.CreatedByIp)
                .HasMaxLength(50);
        });
    }
}