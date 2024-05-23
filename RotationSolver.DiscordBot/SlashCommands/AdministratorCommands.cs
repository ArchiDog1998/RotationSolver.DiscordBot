using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace RotationSolver.DiscordBot.SlashCommands;

[SlashCommandGroup("Admin", "The commands for administrators")]
[SlashCommandPermissions(DSharpPlus.Permissions.Administrator)]
internal class AdministratorCommands : ApplicationCommandModule
{
    [SlashCommand("Event", "Hold an event.")]
    public async Task HoldEvent(InteractionContext ctx,
        [Option("Description", "The description of the event.", true)] string description,
        [Option("Time", "The Time Stamp", true)] long timeStamp,
        [Option("Image", "The Image ID", true)] long thumbnailId = 0)
    {
        await ctx.DeferAsync();

        var channel = ctx.Guild.GetChannel(Config.HappyBunnyChannel);

        description = $"<t:{timeStamp}:F>\n\n" + description;

        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timeStamp);

        var embed = new DiscordEmbedBuilder()
            .WithTitle(EventHander.EventTittle)
            .WithDescription(description)
            .WithFooter("Materia - Sophia")
            .WithTimestamp(dateTime)
            .WithColor(DiscordColor.Green);

        if (thumbnailId > 0)
        {
            var lead = thumbnailId / 1000;
            embed = embed.WithImageUrl($"https://xivapi.com/i/{lead:D3}000/{thumbnailId:D6}_hr1.png");
        }

        await channel.SendMessageAsync(embed);

        await ctx.DeleteResponseAsync();
    }
}
