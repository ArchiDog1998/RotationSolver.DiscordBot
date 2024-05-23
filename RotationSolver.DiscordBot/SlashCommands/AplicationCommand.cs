using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace RotationSolver.DiscordBot.SlashCommands;

internal class AplicationCommand : ApplicationCommandModule
{
    [ContextMenu(ApplicationCommandType.UserContextMenu, "Pet Me")]
    public async Task PetMenu(ContextMenuContext ctx)
    {
        await ctx.DeferAsync();

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.Member.Mention} pets the dear {ctx.TargetMember.Mention}!"));
    }
}
