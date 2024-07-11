using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System.Collections.Concurrent;
using NetStone;
using Octokit;
using System.Reflection;
using NetStone.Model.Parseables.Character.ClassJob;

namespace RotationSolver.DiscordBot.SlashCommands;

public class BotChannelAttribute : SlashCheckBaseAttribute
{
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        if (ctx.Channel.Id != Config.BotChannel) //Wrong channel.
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent($"Sorry, {ctx.Member.Mention}. This feature can only be used in {Config.BotChannelLink}!"));

            await Task.Delay(10000);
            await ctx.DeleteResponseAsync();
            return false;
        }
        return true;
    }
}

public class RotationDevRoleAttribute : SlashCheckBaseAttribute
{
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        if (!ctx.Member.Roles.Any(r => r.Id == Config.RotationDevRole)) //Wrong role.
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent($"Sorry, {ctx.Member.Mention}. This feature can only be used by **Rotation Dev**!"));

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

    [SlashCommand("sharefflogs", "Upload your ff logs if you want. (Without your dc name by default)")]
    public async Task ShareFFlogs(InteractionContext ctx,
        [Option("logsPic", "A picture of the log you want to show")] DiscordAttachment pics,
        [Option("author", "The author of the rotation")] DiscordUser? author = null,
        [Option("rotationJobs", "The jobs used in the picture that you're showing. It is preferable that you use emoji.")] string? rotationjobs = null,
        [Option("text", "Any message that you want to say to the author or to other users")] string? text = null,
        [Option("showYou", "Add your ingame name to the message if you want")] bool showYourName = false)
    {
        await ctx.DeferAsync();

        if (!pics.Width.HasValue)
        {
            await ctx.Member.SendMessageAsync($"You have to post a FFlogs picture!");
            await ctx.DeleteResponseAsync();
            return;
        }

        var authorMember = author as DiscordMember;
        if (authorMember != null && !authorMember.Roles.Any(r => r.Id == Config.RotationDevRole))
        {
            await ctx.Member.SendMessageAsync($"The author you chose has to be a `Rotation Dev`!");
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
    [SlashCommand("askforRole", "Ask for the role you want")]
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

    [BotChannel]
    [RotationDevRole]
    [SlashCommand("lodestonedev", "Send my lodestone id for sending my job levels")]
    public async Task MyLodestone(InteractionContext ctx, 
        [Option("lodestone", "Lodestone ID")] long lodestoneid)
    {
        await ctx.DeferAsync();

        var id = ctx.Member.Id;

        uint lodestone = 0;
        try
        {
            lodestone = Convert.ToUInt32(lodestoneid);
        }
        catch
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Failed to convert your id to `uint`. Please check your value!"));
        }
        var result = await SqlHelper.UpdateRotationDevLodestone(id, lodestone);

        await ctx.DeleteResponseAsync();

        if (result)
        {
            await ctx.Channel.SendMessageAsync($"Dear {ctx.Member.Mention}, your lodestone id was changed!");
        }
        else
        {
            await ctx.Channel.SendMessageAsync($"Dear {ctx.Member.Mention}, there is no Rotation Dev information in database! Failed to change your lodeston id.");
        }
    }

    [RotationDevRole]
    [SlashCommand("pollforjobs", "Poll for jobs for helping you choose the job to develop.")]
    public async Task PollForJobs(InteractionContext ctx,
        [Option("pollType", "The type of poll.")] PollType type,
        [Option("combatType", "The type of combat.")] CombatType combatType,
        [Option("days", "The days for polling.")]double days = 0)
    {
        await ctx.DeferAsync();
        var channel = await Service.Client.GetChannelAsync(Config.RotationAnnounceMentChannel);

        var builder = new DiscordEmbedBuilder()
        {
            Title = $"Poll For Jobs!",
            Color = DiscordColor.IndianRed,
            Description = $"Which **{combatType}** jobs should {ctx.Member.Mention} {type.ToString().ToLower()}? Please click the emoji you want to select from the ones below.",
        };

        var emojis = ctx.Guild.Emojis.Values.Where(e => e.Name.StartsWith("Job")).OrderBy(v => v.Id);

        if (SqlHelper.GetLodestoneId(ctx.Member.Id, out var lodestonId))
        {
            var lodestoneClient = await LodestoneClient.GetClientAsync();
            var lodestoneCharacter = await lodestoneClient.GetCharacter(lodestonId.ToString());
            if (lodestoneCharacter != null)
            {
                var jobs = await lodestoneCharacter.GetClassJobInfo();

                if (jobs != null)
                {
                    Dictionary<JobCate, List<(ClassJobEntry entity, string name, DiscordEmoji emoji)>> entities = [];
                    foreach (var emoji in emojis)
                    {
                        var name = emoji.Name[3..];
                        var entity = (ClassJobEntry)jobs.GetType().GetProperty(name)!.GetValue(jobs)!;
                        var cate = EventHander.JobCategory[name];

                        if (!entities.TryGetValue(cate, out var list)) entities[cate] = list = [];
                        list.Add((entity, name, emoji));
                    }

                    foreach ((var cate, var classJobs) in entities)
                    {
                        builder.AddField(cate.ToString(), string.Join("\n", classJobs.Select(j => $"{j.emoji} {j.name} Lvl.{j.entity.Level}")), true);
                    }
                }
            }
        }

        if (days > 0)
        {
            builder.Timestamp = DateTime.UtcNow.AddDays(days);
        }
        var messages = new List<DiscordMessage>() { await channel.SendMessageAsync(builder) };

        foreach (var emoji in emojis)
        {
            var message = messages.Last();

            try
            {
                var aniEmoji = (await ctx.Guild.GetEmojisAsync()).FirstOrDefault(e => e.Name.Contains("RSLogoAnimated"));
                await message.DeleteReactionsEmojiAsync(aniEmoji);
            }
            catch
            {

            }

            try
            {
                await message.CreateReactionAsync(emoji);
            }
            catch
            {
                message = await channel.SendMessageAsync("Emoji Holder");
                messages.Add(message);
                await message.CreateReactionAsync(emoji);
            }
        }
        await ctx.DeleteResponseAsync();

        if (days == 0) return;
        await Task.Delay(TimeSpan.FromDays(days));

        Dictionary<DiscordEmoji, int> Counts = [];
        foreach (var msg in messages)
        {
            try
            {
                var message = await channel.GetMessageAsync(msg.Id);

                foreach (var reaction in message.Reactions.Where(reaction => reaction.Emoji.Name.StartsWith("Job") && reaction.IsMe))
                {
                    if (!Counts.TryGetValue(reaction.Emoji, out var count)) count = 0;
                    Counts[reaction.Emoji] = reaction.Count - 1 + count;
                }
            }
            catch 
            { 
            }
        }

        if (Counts.Count == 0) return;

        var max = Counts.Select(r => r.Value).Max();
        var pairs = Counts.Where(reactions => reactions.Value == max);
        var emojies = string.Join(" ", pairs.Select(i => i.Key.ToString()));
        await messages.First().RespondAsync($"Hi, {ctx.Member.Mention}! The highest chosen {(pairs.Count() > 1 ? "jobs are" : "job is")}:\n {emojies}, with {max} vote{(max == 1 ? "s" : "")}!");
    }

    public enum PollType
    {
        Fix,
        Create,
    }
    public enum CombatType
    {
        PvP,
        PvE,
    }

    [SlashCommandPermissions(Permissions.Administrator)]
    [BotChannel]
    [SlashCommand("prerelease", "pre-release the version.")]
    public async Task CreatePrerelease(InteractionContext ctx,
        [Option("tittle", "The tittle of the post.")] string tittle,
        [Option("RS", "The rs file")] DiscordAttachment rsFile,
        [Option("rotation", "The rotation file")] DiscordAttachment? rotationFile = null)
    {
        await ctx.DeferAsync();

        var channel = ctx.Guild.GetChannel(Config.PreReleaseChannel) as DiscordForumChannel;
        if (channel == null)
        {
            await ctx.DeleteResponseAsync();
            return;
        }

        using var client = new HttpClient();

        var content = "## Issues:";
        var threadIds = SqlHelper.GetNotFixedIssue();

        var result = await ctx.Guild.ListActiveThreadsAsync();

        foreach (var threadId in threadIds)
        {
            var thread = result.Threads.FirstOrDefault(t => t.Id == threadId);

            if (thread == null) continue;

            if (!thread.AppliedTags.Any(i => i.Id == Config.CompletedTag))
            {
                continue;
            }

            await SqlHelper.FixedIssueData(threadId);
            content += "\n" + thread.Mention;
            //TODO: close thread.
        }

        var message = new DiscordMessageBuilder()
            .WithContent(content)
            .AddFile(rsFile.FileName, await client.GetStreamAsync(rsFile.Url));

        if (rotationFile != null)
        {
            message = message.AddFile(rotationFile.FileName, await client.GetStreamAsync(rotationFile.Url));
        }

        var forum = await channel.CreateForumPostAsync(new ForumPostBuilder()
            .WithMessage(message)
            .WithName(tittle));

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Created a new post " + forum.Channel.Mention));
    }
}
