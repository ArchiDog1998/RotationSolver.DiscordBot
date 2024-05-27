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

    [SlashCommandPermissions(Permissions.Administrator)]
    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Don't At Me")]
    public async Task DontAtMe(ContextMenuContext ctx)
    {
        var main = ctx.TargetMessage.MentionedUsers.FirstOrDefault(u => u.Id == Config.ArchiDiscordID);

        await ctx.DeleteResponseAsync();

        if (main == null) return;

        await ctx.TargetMessage.DeleteAsync();
        await ctx.TargetMember.SendMessageAsync($"Do NOT {main.Mention}! You are timedout for **60** seconds. Your original message:");
        await ctx.TargetMember.SendMessageAsync(ctx.TargetMessage.Content);

        await ctx.TargetMember.TimeoutAsync(new DateTimeOffset(DateTime.UtcNow.AddSeconds(60)));
    }
}
