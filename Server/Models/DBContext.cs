using Microsoft.EntityFrameworkCore;

namespace FlickPickApp.Models;

public class MovieAppDbContext : DbContext
{
    public MovieAppDbContext(DbContextOptions<MovieAppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<UserGroup> UserGroups { get; set; } = null!;
    public DbSet<MovieNightEvent> MovieNightEvents { get; set; } = null!;
    public DbSet<MovieSuggestion> MovieSuggestions { get; set; } = null!;
    public DbSet<Vote> Votes { get; set; } = null!;
    public DbSet<MovieNightRating> MovieNightRatings { get; set; } = null!;
    public DbSet<MoviePreference> MoviePreferences { get; set; } = null!;
    public DbSet<ChatMessage> ChatMessages { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("app");

        //  1. ROLE - USER (One-to-Many)
        // A role can have many users
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // 2. RefreshToken -> User (Many-to-One)
        // One User can have many RefreshTokens
        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 5. RefreshToken -> Client (Many-to-One)
        // One Client can have many RefreshTokens
        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.Client)
            .WithMany(c => c.RefreshTokens)
            .HasForeignKey(rt => rt.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // 6. USER -> USERGROUP (Many-to-Many)
        modelBuilder.Entity<UserGroup>()
            .HasKey(ug => new { ug.UserId, ug.GroupId });

        modelBuilder.Entity<UserGroup>()
            .HasOne(ug => ug.User)
            .WithMany()
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserGroup>()
            .HasOne(ug => ug.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(ug => ug.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // 7. GROUP → MOVIENIGHTEVENT (1–Many)
        modelBuilder.Entity<MovieNightEvent>()
            .HasOne(e => e.Group)
            .WithMany(g => g.MovieNightEvents)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // 8. EVENT → MOVIESUGGESTION (1–Many)
        modelBuilder.Entity<MovieSuggestion>()
            .HasOne(s => s.MovieNightEvent)
            .WithMany(e => e.Suggestions)
            .HasForeignKey(s => s.MovieNightEventId)
            .OnDelete(DeleteBehavior.Cascade);

        // 9. SUGGESTION → VOTE (1–Many)
        modelBuilder.Entity<Vote>()
            .HasOne(v => v.MovieSuggestion)
            .WithMany(s => s.Votes)
            .HasForeignKey(v => v.MovieSuggestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // 10. USER → VOTE
        modelBuilder.Entity<Vote>()
            .HasOne(v => v.User)
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 11. EVENT → RATING
        modelBuilder.Entity<MovieNightRating>()
            .HasOne(r => r.MovieNightEvent)
            .WithMany(e => e.Ratings)
            .HasForeignKey(r => r.MovieNightEventId)
            .OnDelete(DeleteBehavior.Cascade);

        // 12. USER → RATING
        modelBuilder.Entity<MovieNightRating>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 13. EVENT → CHATMESSAGE
        modelBuilder.Entity<ChatMessage>()
            .HasOne(c => c.MovieNightEvent)
            .WithMany(e => e.ChatMessages)
            .HasForeignKey(c => c.MovieNightEventId)
            .OnDelete(DeleteBehavior.Cascade);

        // 14. USER → CHATMESSAGE
        modelBuilder.Entity<ChatMessage>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 15. USER → MOVIEPREFERENCE
        modelBuilder.Entity<MoviePreference>()
            .HasOne(mp => mp.User)
            .WithMany()
            .HasForeignKey(mp => mp.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        base.OnModelCreating(modelBuilder);

        SeedRoles(modelBuilder);
        SeedAdminUser(modelBuilder);
        SeedDefaultClient(modelBuilder);

        // Configure required properties and constraints
        ConfigureEntityConstraints(modelBuilder);

        // Configure indexes for performance
        ConfigureIndexes(modelBuilder);
    }

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
            .IsUnique();

        modelBuilder.Entity<MovieNightRating>()
            .HasIndex(r => new { r.UserId, r.MovieNightEventId })
            .IsUnique();

        modelBuilder.Entity<UserGroup>()
            .HasIndex(ug => new { ug.UserId, ug.GroupId })
            .IsUnique();
    }
}
