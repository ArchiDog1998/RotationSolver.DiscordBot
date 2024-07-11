namespace RotationSolver.DiscordBot.Npgsql;

internal class IssueItem
{
    public ulong ThreadID { get; set; }
    public ulong MessageID { get; set; }
    public bool Fixed { get; set; }
}
