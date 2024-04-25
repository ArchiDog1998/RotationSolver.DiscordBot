using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System.Collections.Concurrent;

namespace RotationSolver.DiscordBot.SlashCommands;

public class BotChannelAttribute : SlashCheckBaseAttribute
{
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        if (ctx.Channel.Id != Config.BotChannel) //Wrong channel.
        {
            await ctx.DeferAsync();
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Sorry, {ctx.Member.Mention}. This feature can only be used in {Config.BotChannelLink}!"));

            await Task.Delay(10000);
            await ctx.DeleteResponseAsync();
            return false;
        }
        return true;
    }
}

public class GeneralCommands : ApplicationCommandModule
{
    [SlashCooldown(5, 60, SlashCooldownBucketType.User)]
    [SlashCommand("pet", "Do you wanna be petted?")]
    public async Task JustAPet(InteractionContext ctx)
    {
        await ctx.DeferAsync();

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Pet the dear {ctx.Member.Mention}!"));
    }

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Pet Me")]
    public async Task PetMenu(ContextMenuContext ctx)
    {
        await ctx.DeferAsync();

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.Member.Mention} pets the dear {ctx.TargetMember.Mention}!"));
    }

    [SlashCommand("sharefflogs", "Upload your ff logs if you want. (Without your dc name by default)")]
    public async Task ShareFFlogs(InteractionContext ctx,
        [Option("logsPic", "The pics about your log")] DiscordAttachment pics,
        [Option("author", "The author of this rotation")] DiscordUser? author = null,
        [Option("rotationJobs", "The jobs of the rotation. It is better to use emoji.")] string? rotationjobs = null,
        [Option("text", "The things you want to say to the author or the users")] string? text = null,
        [Option("showYou", "Add your name to the message")] bool showYourName = false)
    {
        await ctx.DeferAsync();

        if (!pics.Width.HasValue)
        {
            await ctx.Member.SendMessageAsync($"You have to post a fflog picture!");
            await ctx.DeleteResponseAsync();
            return;
        }

        var authorMember = author as DiscordMember;
        if (authorMember != null && !authorMember.Roles.Any(r => r.Id == Config.RotationDevRole))
        {
            await ctx.Member.SendMessageAsync($"The author you chosen has to be the `Rotation Dev`!");
            await ctx.DeleteResponseAsync();
            return;
        }

        var channel = ctx.Guild.GetChannel(Config.LogsChannel);

        var builder = new DiscordEmbedBuilder()
        {
            Title = "FF Logs using RS!",
            ImageUrl = pics.Url,
            Color = DiscordColor.HotPink,
            Description = text ?? string.Empty,
        };

        if (!string.IsNullOrEmpty(rotationjobs))
        {
            builder = builder.AddField("Rotation Jobs", rotationjobs, true);
        }
        if (author != null)
        {
            builder = builder.AddField("Rotation Author", author.Mention, true);
        }
        if (showYourName)
        {
            builder = builder.AddField("FF Logs Owner", ctx.Member.Mention, true);
        }

        await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(builder));
        await ctx.DeleteResponseAsync();
    }

    internal static readonly ConcurrentDictionary<ulong, (DiscordMember, DiscordRole)> _askings = [];

    [BotChannel]
    [SlashCommand("askforRole", "ask for the role you want")]
    public async Task AskForRole(InteractionContext ctx,
        [Option("Role", "The role you want")] DiscordRole role,
        [Option("Description", "The description, github or ko-fi")] string desc)
    {
        var dev = ctx.Guild.GetChannel(Config.ModeratorChannel);
        if (dev == null) return;

        await ctx.DeferAsync();
        var accept = new DiscordButtonComponent(ButtonStyle.Primary, "AcceptTheRole", "Accept", false);
        var reject = new DiscordButtonComponent(ButtonStyle.Danger, "RejectTheRole", "Reject", false);

        var message = new DiscordMessageBuilder()
            .AddComponents(accept, reject)
            .AddEmbed(new DiscordEmbedBuilder()
            .WithDescription(desc)
            .AddField("Name: ", ctx.Member.Mention)
            .AddField("Role: ", role.Mention));

        var mess = await dev.SendMessageAsync(message);

        _askings[mess.Id] = (ctx.Member, role);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Your request was sent to {dev.Mention}, so please wait."));
    }
}
