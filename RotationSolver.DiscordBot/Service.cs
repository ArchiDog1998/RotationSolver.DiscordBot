using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json.Linq;
using RotationSolver.DiscordBot.SlashCommands;

namespace RotationSolver.DiscordBot;

public static partial class Service
{
    internal static DiscordClient Client { get; private set; } = null!;

    public static async Task Init()
    {
        Client = new DiscordClient(new()
        {
            Intents = DiscordIntents.All,
            Token = Config.BotToken,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
        });

        var slashCommands = Client.UseSlashCommands();
        slashCommands.RegisterCommands<GeneralCommands>();
        slashCommands.RegisterCommands<SupportersCommands>();
        slashCommands.SlashCommandErrored += SlashCommands_SlashCommandErrored;

        Client.GuildMemberRemoved += Client_GuildMemberRemoved;
        Client.UnknownEvent += Client_UnknownEvent;
        Client.MessageCreated += Client_MessageCreated;
        Client.ComponentInteractionCreated += Client_ComponentInteractionCreated;

        await Client.ConnectAsync(new("RS on FFXIV."));

        _ = Task.Run(async () =>
        {
            using var timer = new PeriodicTimer(TimeSpan.FromDays(1));

            while (await timer.WaitForNextTickAsync())
            {
                foreach (var guild in Client.Guilds.Values)
                {
                    await UpdateContributorRoles(guild);
                }
            }
        });
    }

    private static async Task Client_ComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs args)
    {
        await args.Interaction.DeferAsync();

        var channel = args.Guild.GetChannel(Config.BotChannel);

        if (channel != null)
        {
            switch (args.Interaction.Data.CustomId)
            {
                case "AcceptTheRole" when GeneralCommands._askings.TryRemove(args.Message.Id, out var pair):
                    await pair.Item1.GrantRoleAsync(pair.Item2);
                    await channel.SendMessageAsync($"{pair.Item1.Mention}'s request for the role **{pair.Item2.Name}** has been **APPROVED** by {args.Interaction.User.Mention}.");

                    await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"{args.Interaction.User.Mention} **APPROVED** {pair.Item1.Mention} **{pair.Item2.Name}**"));
                    break;
                case "RejectTheRole" when GeneralCommands._askings.TryRemove(args.Message.Id, out var pair):
                    await channel.SendMessageAsync($"{pair.Item1.Mention}'s request for the role **{pair.Item2.Name}** has been **REJECTED** by {args.Interaction.User.Mention}.");
                    await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"{args.Interaction.User.Mention} **REJECTED** {pair.Item1.Mention} **{pair.Item2.Name}**"));

                    break;

                default:
                    await args.Interaction.DeleteOriginalResponseAsync();
                    break;
            }
        }
        else
        {
            await args.Interaction.DeleteOriginalResponseAsync();
        }

        await args.Message.DeleteAsync();
    }

    private static async Task UpdateContributorRoles(DiscordGuild guild)
    {
        var contributors = await GithubHelper.GetContributors();
        foreach (var contributor in contributors)
        {
            if (!SqlHelper.GetIDFromGithub(contributor.Id, out var data)) continue;
            if (data == null || data.Length == 0) continue;

            var member = await guild.GetMemberAsync(data[0]);
            if (member == null) continue;

            if (member.Roles.Any(i => i.Id == Config.ContributerRole)) continue; //Contributer

            await member.GrantRoleAsync(guild.GetRole(Config.ContributerRole));
        }
    }

    private static async Task Client_MessageCreated(DiscordClient sender, MessageCreateEventArgs args)
    {
        var member = await args.Guild.GetMemberAsync(args.Author.Id);
        if (member == null) return;

        if (member.IsBot) return;
        if (member.Roles.Any(r => r.Id == Config.ModeratorRole)) return; //Moderators can do anything!

        await DontAtMe(args, member);
        await ManageLogMessages(args, member);
    }

    private static async Task ManageLogMessages(MessageCreateEventArgs args, DiscordMember member)
    {
        if (args.Channel.Id != Config.LogsChannel) return;

        if (args.Message.Attachments.Any(i => i.Width.HasValue)) return; //Has a pic.

        await args.Message.DeleteAsync();
        await member.SendMessageAsync($"You have to post at least one pic on the {args.Channel.Mention}!");
    }

    private static async Task DontAtMe(MessageCreateEventArgs args, DiscordMember member)
    {
        if (args.Message.ReferencedMessage != null) return;
        var main = args.MentionedUsers.FirstOrDefault(u => u.Id == Config.ArchiDiscordID);
        if (main == null) return;

        await args.Message.DeleteAsync();
        await member.SendMessageAsync($"Do NOT {main.Mention}! You are about to be timeout for **60** seconds. Your original message:");
        await member.SendMessageAsync(args.Message.Content);

        await member.TimeoutAsync(new DateTimeOffset(DateTime.UtcNow.AddSeconds(60)));
    }

    private static async Task Client_UnknownEvent(DiscordClient sender, UnknownEventArgs args)
    {
        switch (args.EventName.ToLowerInvariant())
        {
            case "guild_audit_log_entry_create":
                var obj = JObject.Parse(args.Json);

                switch (int.Parse(obj["action_type"]!.ToString()))
                {
                    case 25://Role change
                        var guildId = ulong.Parse(obj["guild_id"]!.ToString());
                        var guild = await sender.GetGuildAsync(guildId);

                        var tarId = ulong.Parse(obj["target_id"]!.ToString());
                        var member = await guild.GetMemberAsync(tarId);

                        var changes = obj["changes"] as JArray;
                        foreach (var change in changes!)
                        {
                            var newValues = change["new_value"] as JArray;

                            var ids = newValues!.Select(i => ulong.Parse(i["id"]!.ToString())).ToArray();

                            switch (change["key"]!.ToString())
                            {
                                case "$add":
                                    await OnRoleChanged(guild, member, ids, true);
                                    break;

                                case "$remove":
                                    await OnRoleChanged(guild, member, ids, false);
                                    break;
                            }
                        }
                        break;
                }
                break;
        }
    }

    private static async Task OnRoleChanged(DiscordGuild guild, DiscordMember member, ulong[] roles, bool isAdd)
    {
        if (roles.Any(i => i == Config.SupporterRole))
        {
            if (isAdd) // Send a kind message
            {
                SqlHelper.IsvalidSupporter(member.Id, true);

                var builder = new DiscordEmbedBuilder()
                {
                    Title = "**Thanks for your support!**",
                    Color = DiscordColor.IndianRed,
                    Description = $"**Hi {member.Mention}, Thanks for your support!**\nPlease don't forget to go {Config.BotChannelLink} and use `/supporterhash` to enable your supporter-only features!",
                    Footer = new() { Text = "Thank you so much!" },
                };

                await member.SendMessageAsync(builder);
            }
            else
            {
                SqlHelper.IsvalidSupporter(member.Id, false);
            }
            await SupportersCommands.UpdateNames();
            await SupportersCommands.UpdateHashes();
        }
        if (roles.Any(i => i == Config.RotationDevRole)) //Rotation Dev.
        {
            DiscordChannel? channel = null;
            if (SqlHelper.GetChannelId(member.Id, out var data) && data.Length > 0)
            {
                try
                {
                    channel = guild.GetChannel(data[0]);
                }
                catch
                {

                }
            }
            const Permissions permissions = (Permissions)635517720854526;

            if (isAdd)
            {
                if (channel == null) //Create a channel.
                {
                    var parent = guild.GetChannel(Config.RotationsCategory);

                    var p = new DiscordOverwriteBuilder(member).Allow(permissions);
                    channel = await guild.CreateChannelAsync($"{member.DisplayName} Rotations", ChannelType.GuildForum, parent, $"Talk about everything about the rotations from {member.Mention}!", overwrites: [p]);

                    SqlHelper.UpdateRotationDevChannel(member.Id, channel.Id);
                }
                else
                {
                    await CanViewChannel(channel, true);
                    await channel.AddOverwriteAsync(member, permissions, reason: $"{member.DisplayName} becomes a rotation developer!");
                }

                var builder = new DiscordEmbedBuilder()
                {
                    Title = "**You are Rotation Developer Now!**",
                    Color = DiscordColor.CornflowerBlue,
                    Description = $"Hello {member.Mention}, thank you for being interested to make a rotation for Rotation Solver, you can use {channel.Mention} to recieve feedback from the other players that want to use your rotation if you wish to. Have fun!",
                    Footer = new() { Text = "Come on! You are the best!" },
                };

                await member.SendMessageAsync(builder);
            }
            else
            {
                if (channel != null)
                {
                    await CanViewChannel(channel, false);
                    await channel.DeleteOverwriteAsync(member, $"{member.DisplayName} is not a rotation developer!");
                }
            }
        }
    }

    private static async Task CanViewChannel(DiscordChannel channel, bool canView)
    {
        var overwrite = channel.PermissionOverwrites?.FirstOrDefault(p => p.Type == OverwriteType.Role && p.GetRoleAsync().Result.Name == "@everyone");
        if (overwrite == null)
        {
            var role = channel.Guild.Roles.Values.FirstOrDefault(r => r.Name == "@everyone");
            if (role == null) return;
            await channel.AddOverwriteAsync(role, canView ? Permissions.AccessChannels : Permissions.None, canView ? Permissions.None : Permissions.AccessChannels);
            return;
        }

        if (canView)
        {
            await overwrite.UpdateAsync(overwrite.Allowed | Permissions.AccessChannels, overwrite.Denied & ~Permissions.AccessChannels);
        }
        else
        {
            await overwrite.UpdateAsync(overwrite.Allowed & ~Permissions.AccessChannels, overwrite.Denied | Permissions.AccessChannels);
        }
    }

    private static async Task SlashCommands_SlashCommandErrored(SlashCommandsExtension sender, DSharpPlus.SlashCommands.EventArgs.SlashCommandErrorEventArgs args)
    {
        await args.Context.DeferAsync();

        var exception = args.Exception;
        if (exception is SlashExecutionChecksFailedException)
        {
            await args.Context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Wait for less than 1min to use this command again!"));
            return;
        }

        var dev = args.Context.Guild.GetChannel(Config.ModeratorChannel);
        if (dev == null)
        {
            await args.Context.DeleteResponseAsync();
            return;
        }

        var str = string.Empty;
        while (exception != null)
        {
            str += exception.Message + "\n" + (exception.StackTrace ?? string.Empty) + "\n \n";
            exception = exception.InnerException;
        }
        await dev.SendMessageAsync(str);

        await args.Context.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Errored! Already sent the error to {dev.Mention}, Please wait for their check!"));
    }

    private static async Task Client_GuildMemberRemoved(DiscordClient sender, GuildMemberRemoveEventArgs args)
    {
        //Invalid Supporter.
        SqlHelper.IsvalidSupporter(args.Member.Id, false);
        await SupportersCommands.UpdateNames();
        await SupportersCommands.UpdateHashes();

        //Invalid Rotation Dev.
        if (SqlHelper.GetChannelId(args.Member.Id, out var data) && data.Length > 0)
        {
            var channel = args.Guild.GetChannel(data[0]);

            await CanViewChannel(channel, false);
            await channel.DeleteOverwriteAsync(args.Member, $"{args.Member.DisplayName} left this guild!");
        }
    }

    internal static async void SendGithubPublish(string s)
    {
        var obj = JObject.Parse(s);
        if (obj["action"]?.ToString() != "published") return;
        var token = obj["release"];
        if (token == null) return;

        var url = token["html_url"]?.ToString();
        var name = token["name"]?.ToString();
        var body = token["body"]?.ToString();

        token = token["author"];
        var icon = token?["avatar_url"]?.ToString();
        var login = token?["login"]?.ToString();
        var authorUrl = token?["html_url"]?.ToString();

        var channel = await Client.GetChannelAsync(Config.AnnounceMent);

        var embedBuilder = new DiscordEmbedBuilder()
            .WithAuthor(login, authorUrl, icon)
            .WithTitle($"**{name} Released!**")
            .WithUrl(url)
            .WithDescription(body)
            .WithColor(DiscordColor.Black);

        var message = await channel.SendMessageAsync(embedBuilder);
        await message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(Client, Config.RotationSolverIcon));
    }
}
