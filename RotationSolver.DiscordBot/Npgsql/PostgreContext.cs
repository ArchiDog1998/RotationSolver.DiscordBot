using Microsoft.EntityFrameworkCore;

namespace RotationSolver.DiscordBot.Npgsql;
internal class PostgreContext : DbContext
{
    public DbSet<CommitItem> GithubCommit { get; set; }
    public DbSet<DeveloperItem> RotationDev { get; set; }
    public DbSet<SupporterItem> Supporter { get; set; }
    public DbSet<IssueItem> Issues { get; set; }
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
        modelBuilder.Entity<SupporterItem>().HasKey(i => i.DiscordID);
        modelBuilder.Entity<IssueItem>().HasKey(i => i.ThreadID);
    }
}
