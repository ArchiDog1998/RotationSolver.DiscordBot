using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RotationSolver.DiscordBot.Npgsql;
internal class DeveloperItem
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public ulong DiscordID { get; set; }
    public ulong? ChannelID { get; set; }
    public uint? LodestoneID { get; set; }
}
