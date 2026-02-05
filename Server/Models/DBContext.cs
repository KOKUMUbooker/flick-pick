using  Microsoft.EntityFrameworkCore;

namespace MovieManager.Models;

public class MovieAppDbContext : DbContext
{
    public MovieAppDbContext(DbContextOptions<MovieAppDbContext> options) : base(options) {
    }

    public DbSet<Movie> Movies {get; set;} = null!;
    public DbSet<User> Users {get; set;} = null!;
    public DbSet<MovieCast> MovieCasts {get; set;} = null!;
    public DbSet<Review> Reviews {get; set;}
    public DbSet<Role> Roles {get; set;}
    public DbSet<Cast> Cast {get; set;}
    public DbSet<Client> Clients {get; set;}
    public DbSet<RefreshToken> RefreshTokens {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("app");

        // 1. MOVIE - REVIEWS (One-to-Many)
        // A Movie can have many reviews
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Movie)
            .WithMany(m => m.Reviews)
            .HasForeignKey(r => r.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // 2. USER - REVIEWS (One-to-Many)
        // A user can have many reviews
        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 3. ROLE - USER (One-to-Many)
        // A role can have many users
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // 4. RefreshToken -> User (Many-to-One)
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

        // 5. MANY-TO-MANY: Movie - Cast via MovieCast
        modelBuilder.Entity<MovieCast>()
            .HasKey(mc => new { mc.MovieId, mc.CastId }); // Composite primary key
            
        // MovieCast -> Movie relationship
        modelBuilder.Entity<MovieCast>()
            .HasOne(mc => mc.Movie)
            .WithMany(m => m.MovieCasts)
            .HasForeignKey(mc => mc.MovieId)
            .OnDelete(DeleteBehavior.Cascade); // Delete join records when movie deleted
            
        // MovieCast -> Cast relationship  
        modelBuilder.Entity<MovieCast>()
            .HasOne(mc => mc.Cast)
            .WithMany(c => c.MovieCasts)
            .HasForeignKey(mc => mc.CastId)
            .OnDelete(DeleteBehavior.Cascade); // Delete join records when cast deleted

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
        var clientSecret = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("movie-manager-secret-key"));

        modelBuilder.Entity<Client>().HasData(
            new Client
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), // Fixed GUID
                ClientId = clientId,
                Name = "Movie Manager Web Application",
                ClientSecret = clientSecret,
                ClientURL = "https://localhost:5176",  
                IsActive = true,
                Created = now,
                LastModified = now
            }
        );
    }

    private static void ConfigureEntityConstraints(ModelBuilder modelBuilder)
    {
        // MOVIE constraints
        modelBuilder.Entity<Movie>()
            .Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        modelBuilder.Entity<Movie>()
            .Property(m => m.Description)
            .IsRequired()
            .HasMaxLength(2000);
            
        modelBuilder.Entity<Movie>()
            .Property(m => m.AgeRating)
            .IsRequired()
            .HasMaxLength(10);
            
        modelBuilder.Entity<Movie>()
            .Property(m => m.Rating)
            .HasPrecision(3, 1);
            
        // USER constraints
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
            
        modelBuilder.Entity<User>()
            .Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);
            
        // REVIEW constraints
        modelBuilder.Entity<Review>()
            .Property(r => r.Text)
            .IsRequired()
            .HasMaxLength(5000);
            
        // CAST constraints
        modelBuilder.Entity<Cast>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        modelBuilder.Entity<Cast>()
            .Property(c => c.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);
            
        // MOVIECAST constraints  
        modelBuilder.Entity<MovieCast>()
            .Property(mc => mc.CharacterName)  
            .HasMaxLength(100)
            .IsRequired(false);  
            
        modelBuilder.Entity<MovieCast>()
            .Property(mc => mc.Order) // For display order
            .HasDefaultValue(0);

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
        // Movie indexes
        modelBuilder.Entity<Movie>()
            .HasIndex(m => m.Title);
            
        modelBuilder.Entity<Movie>()
            .HasIndex(m => m.ReleaseDate);
            
        modelBuilder.Entity<Movie>()
            .HasIndex(m => m.Rating);
            
        modelBuilder.Entity<Movie>()
            .HasIndex(m => m.Verified);
            
        // User indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
            
        modelBuilder.Entity<User>()
            .HasIndex(u => u.RoleId);
            
        // Review indexes
        modelBuilder.Entity<Review>()
            .HasIndex(r => r.MovieId);
            
        modelBuilder.Entity<Review>()
            .HasIndex(r => r.UserId);
            
        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.MovieId, r.UserId })
            .IsUnique(); // One review per user per movie
            
        // Cast indexes
        modelBuilder.Entity<Cast>()
            .HasIndex(c => c.Name);
            
        // MovieCast indexes
        modelBuilder.Entity<MovieCast>()
            .HasIndex(mc => mc.MovieId);
            
        modelBuilder.Entity<MovieCast>()
            .HasIndex(mc => mc.CastId);
            
        modelBuilder.Entity<MovieCast>()
            .HasIndex(mc => mc.CharacterName);

        // RefreshTokens indexes
        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r=>r.UserId);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r=>r.ClientId);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r=>r.Expires);
        
        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r=>r.IsRevoked);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r=>new { r.UserId, r.IsRevoked, r.Expires });
    }
}