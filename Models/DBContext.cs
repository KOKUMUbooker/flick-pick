using Microsoft.EntityFrameworkCore;

namespace WatchHive.Models;

public partial class WatchHiveDbContext : DbContext
{
    public WatchHiveDbContext(DbContextOptions<WatchHiveDbContext> options) : base(options)
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

        // One to many
        // User - RefreshToken
        modelBuilder.Entity<User>()
            .HasMany(u => u.RefreshTokens)
            .WithOne(rf => rf.User)
            .HasForeignKey(rf => rf.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // User - MovieSuggestion
        modelBuilder.Entity<User>()
            .HasMany(u => u.MovieSuggestions)
            .WithOne(ms => ms.SuggestedBy)
            .HasForeignKey(ms => ms.SuggestedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction); 

        // User - Vote
        modelBuilder.Entity<User>()
            .HasMany(u => u.Votes)
            .WithOne(v => v.User)
            .HasForeignKey(v => v.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        // User - MoviePreference
        modelBuilder.Entity<User>()
            .HasMany(u => u.MoviePreferences)
            .WithOne(mp => mp.User)
            .HasForeignKey(mp => mp.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        // User - Groups
        modelBuilder.Entity<User>()
            .HasMany(u => u.CreatedGroups)
            .WithOne(g => g.CreatedBy)
            .HasForeignKey(g => g.CreatedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        // User - MovieNightRating
        modelBuilder.Entity<User>()
            .HasMany(u => u.MovieNightRatings)
            .WithOne(mr => mr.User)
            .HasForeignKey(mr => mr.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        // User - ChatMessage
        modelBuilder.Entity<User>()
            .HasMany(u => u.ChatMessages)
            .WithOne(cm => cm.User)
            .HasForeignKey(cm => cm.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        // User - MovieNightEvent (Only Group admins)
        modelBuilder.Entity<User>()
            .HasMany(u => u.MovieNightEvents)
            .WithOne(mn => mn.CreatedBy)
            .HasForeignKey(mn => mn.CreatedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        // Role - User
        modelBuilder.Entity<Role>()
            .HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        // Group - MovieNightEvent
        modelBuilder.Entity<Group>()
            .HasMany(g => g.MovieNightEvents)
            .WithOne(mn => mn.Group)
            .HasForeignKey(mn => mn.GroupId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // MovieNightEvent - ChatMessage
        modelBuilder.Entity<MovieNightEvent>()
            .HasMany(mn => mn.ChatMessages)
            .WithOne(cm => cm.MovieNightEvent)
            .HasForeignKey(cm => cm.MovieNightEventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        // MovieNightEvent - MovieNightRating
        modelBuilder.Entity<MovieNightEvent>()
            .HasMany(mn => mn.MovieNightRatings)
            .WithOne(mr => mr.MovieNightEvent)
            .HasForeignKey(mr => mr.MovieNightEventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        // MovieNightEvent - MovieSuggestion
        modelBuilder.Entity<MovieNightEvent>()
            .HasMany(mn => mn.MovieSuggestions)
            .WithOne(ms => ms.MovieNightEvent)
            .HasForeignKey(ms => ms.MovieNightEventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        // Client - RefreshToken
        modelBuilder.Entity<Client>()
            .HasMany(c => c.RefreshTokens)
            .WithOne(rf => rf.Client)
            .HasForeignKey(rf => rf.ClientId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        // Many to Many
        // User - Groups -> UserGroups Joint table used to address it
        modelBuilder.Entity<UserGroup>()
            .HasKey(ug => new { ug.UserId, ug.GroupId });

        modelBuilder.Entity<User>()
            .HasMany(u => u.UserGroups)
            .WithOne(ug => ug.User)
            .HasForeignKey(ug => ug.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<Group>()
            .HasMany(g => g.Members)
            .WithOne(ug => ug.Group)
            .HasForeignKey(ug => ug.GroupId)
            .IsRequired()
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
}
