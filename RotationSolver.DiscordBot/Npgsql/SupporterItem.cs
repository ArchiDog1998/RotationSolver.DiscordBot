namespace RotationSolver.DiscordBot.Npgsql;
internal class SupporterItem
{
    public ulong DiscordID { get; set; }
    public string[] Hashes { get; set; } = [];
    public string? Name { get; set; }
    public bool IsValid { get; set; }
}
