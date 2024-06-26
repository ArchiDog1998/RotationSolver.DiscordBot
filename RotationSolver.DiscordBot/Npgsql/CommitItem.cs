namespace RotationSolver.DiscordBot.Npgsql;
internal class CommitItem
{
    public string Sha { get; set; } = string.Empty;
    public long Repo { get; set; }
}
