using Microsoft.EntityFrameworkCore;

public class GamePlatformContext : DbContext
{
    public GamePlatformContext(DbContextOptions<GamePlatformContext> options)
        : base(options)
    {
    }
    public DbSet<PlayerData> PlayerData {get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerData>()
            .HasKey(player => player.PlayerId);

        modelBuilder.Entity<PlayerData>()
            .HasIndex(player => player.Email)
            .IsUnique();
    }

}