using Microsoft.EntityFrameworkCore;

namespace RotationSolver.DiscordBot.Npgsql;
internal class PostgreContext : DbContext
{
    public DbSet<CommitItem> GithubCommit { get; set; }
    public DbSet<DeveloperItem> RotationDev { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(Config.PostgreSQL)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CommitItem>().HasKey(i => i.Sha);
        modelBuilder.Entity<DeveloperItem>().HasKey(i => i.DiscordID);
    }
}
