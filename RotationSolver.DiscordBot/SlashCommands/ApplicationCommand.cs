using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace RotationSolver.DiscordBot.SlashCommands;

internal class ApplicationCommand : ApplicationCommandModule
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
        await ctx.DeferAsync();

        var message = await ctx.Channel.GetMessageAsync(ctx.TargetMessage.Id);
        await DeleteTimeoutMessage(message);

        await ctx.DeleteResponseAsync();
    }

    public static async Task DeleteTimeoutMessage(DiscordMessage message)
    {
        var member = message.Author as DiscordMember;
        if (member == null) return;

        var main = message.MentionedUsers.FirstOrDefault(u => u.Id == Config.ArchiDiscordID);
        if (main == null) return;

        await message.DeleteAsync();
        await member.SendMessageAsync($"Do NOT {main.Mention}! You are timedout for **60** seconds. Your original message:");
        await member.SendMessageAsync(message.Content);
        await member.TimeoutAsync(new DateTimeOffset(DateTime.UtcNow.AddSeconds(60)));
    }
}
