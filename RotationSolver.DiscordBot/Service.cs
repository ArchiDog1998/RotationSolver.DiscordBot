﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json.Linq;
using OpenAI.Chat;
using RotationSolver.DiscordBot.SlashCommands;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        slashCommands.RegisterCommands<SupporterCommands>();
        slashCommands.RegisterCommands<BunnyCommands>();
        slashCommands.RegisterCommands<ApplicationCommand>();

        slashCommands.SlashCommandErrored += SlashCommands_SlashCommandErrored;

        Client.GuildMemberRemoved += Client_GuildMemberRemoved;
        Client.GuildMemberAdded += Client_GuildMemberAdded;
        Client.MessageCreated += Client_MessageCreated;
        Client.MessageUpdated += Client_MessageUpdated;
        Client.MessageDeleted += Client_MessageDeleted;
        Client.ComponentInteractionCreated += Client_ComponentInteractionCreated;

        UnkownHander.Init();
        EventHander.Init();

        await Client.ConnectAsync(new("RS on FFXIV"));
        DailyWork.Init();
    }

    private static async Task Client_GuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs args)
    {
        var member = await args.Guild.GetMemberAsync(args.Member.Id);
        var roles = member.Roles.Select(i => i.Id);
        if (roles.Any(i => i == Config.SupporterRole))
        {
            await SendSupportThank(member);
        }
        if (roles.Any(i => i is Config.KofiRole or Config.PatreonRole))
        {
            await SendSubscribeThank(member);
        }
    }

    internal static async Task SendSupportThank(DiscordMember member)
    {
        var builder = new DiscordEmbedBuilder()
        {
            Title = "**Thanks for your support!**",
            Color = DiscordColor.IndianRed,
            Description = $"**Hi {member.Mention}, Thanks for your support!**\nYou can go {Config.BotChannelLink} and use `/supporter name` to change your display name in the supporter list in the plugin!",
            Footer = new() { Text = "Thank you so much!" },
        };

        await member.SendMessageAsync(builder);
        await SqlHelper.InitName(member);
    }

    internal static async Task SendSubscribeThank(DiscordMember member)
    {
        await SqlHelper.IsvalidSupporter(member.Id, true);

        var builder = new DiscordEmbedBuilder()
        {
            Title = "**Thanks for your support!**",
            Color = DiscordColor.IndianRed,
            Description = $"**Hi {member.Mention}, Thank you for the support!**\nPlease don't forget to go {Config.BotChannelLink} and use `/supporter hash` to enable your supporter-only features!",
            Footer = new() { Text = "Thank you so much!" },
        };

        await member.SendMessageAsync(builder);

        await SupporterCommands.UpdateHashes();
        await SqlHelper.InitName(member);
    }

    private static async Task Client_MessageDeleted(DiscordClient sender, MessageDeleteEventArgs args)
    {
        var channel = await sender.GetChannelAsync(Config.ServerLogChannel);
        if (channel == null) return;

        var author = args.Message.Author;
        if (author == null || author.IsBot) return;
        var time = args.Message.EditedTimestamp ?? args.Message.CreationTimestamp;

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Message Deleted")
            .WithTimestamp(time)
            .WithAuthor(author.Username, iconUrl: author.AvatarUrl);

        await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed).WithContent(args.Message.Content));
    }

    private static async Task Client_MessageUpdated(DiscordClient sender, MessageUpdateEventArgs args)
    {
        var channel = await sender.GetChannelAsync(Config.ServerLogChannel);
        if (channel == null) return;

        var message = args.MessageBefore;
        if (message == null) return;

        var author = message.Author;
        if (author.IsBot) return;

        var time = message.EditedTimestamp ?? message.CreationTimestamp;

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Message Edited")
            .WithTimestamp(time)
            .WithAuthor(author.Username, message.JumpLink.AbsoluteUri, author.AvatarUrl);

        await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed).WithContent(message.Content));
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

                case "AcceptTheRefund" when SupporterCommands._askings.TryRemove(args.Message.Id, out var member):
                    await channel.SendMessageAsync($"{member.Mention}'s request for the **Refund** has been **APPROVED** by {args.Interaction.User.Mention}.");

                    await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"{args.Interaction.User.Mention} **APPROVED** {member.Mention} **Refund**"));
                    break;

                case "RejectTheRefund" when SupporterCommands._askings.TryRemove(args.Message.Id, out var member):
                    await channel.SendMessageAsync($"{member}'s request for the **Refund** has been **REJECTED** by {args.Interaction.User.Mention}.");
                    await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"{args.Interaction.User.Mention} **REJECTED** {member.Mention} **Refund**"));
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

        var builder = new DiscordMessageBuilder(args.Message);
        builder.ClearComponents();
        await args.Message.ModifyAsync(builder);
    }

    private static async Task Client_MessageCreated(DiscordClient sender, MessageCreateEventArgs args)
    {
        await AnswerQuestionAsync(args);

        if (args.Guild == null) return;
        var member = await args.Guild.GetMemberAsync(args.Author.Id);
        if (member == null) return;

        if (args.Channel.Id is Config.AnnounceMent or Config.RotationAnnounceMentChannel or Config.GithubChannel)
        {
            await AddAnimatedLogo(args.Guild, args.Message);
        }

        if (member.IsBot) return;
        if (member.Roles.Any(r => r.Id == Config.ModeratorRole)) return; //Moderators can do anything!

        await DontAtMe(args);
        await ManageLogMessages(args, member);
    }

    private static async Task AnswerQuestionAsync(MessageCreateEventArgs args)
    {
        if (args.Message.ReferencedMessage != null) return;
        if (!args.Message.MentionedUsers.Contains(Client.CurrentUser)) return;

        var messagebuilder = new DiscordMessageBuilder().WithReply(args.Message.Id);
        if (args.Channel.Id is Config.QuestionChannel)
        {
            var question = args.Message.Content.Replace(Client.CurrentUser.Mention, "");
            var content = "> " + question.Replace("\n", "> ") + "\n\n";
            messagebuilder = messagebuilder.WithContent(content);
            var message = await args.Channel.SendMessageAsync(messagebuilder);

            DateTime time = DateTime.Now;
            await foreach (var update in GptHelper.GptTalk(question))
            {
                foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
                {
                    content += updatePart.Text;
                }

                if (DateTime.Now - time > TimeSpan.FromSeconds(3))
                {
                    time = DateTime.Now;
                    try
                    {
                        await message.ModifyAsync(content);
                    }
                    catch
                    {

                    }
                }
            }
            await message.ModifyAsync(content);
        }
        else
        {
            messagebuilder = messagebuilder.WithContent($"I am very proud, I will only answer you in {args.Guild.GetChannel(Config.QuestionChannel).Mention}!");
            await args.Channel.SendMessageAsync(messagebuilder);
        }
    }

    internal static async Task AddAnimatedLogo(DiscordGuild guild, DiscordMessage message)
    {
        var emojis = await guild.GetEmojisAsync();
        var emoji = emojis.FirstOrDefault(e => e.Name.Contains("RSLogoAnimated"));
        if (emoji != null)
        {
            await message.CreateReactionAsync(emoji);
        }
    }

    private static async Task ManageLogMessages(MessageCreateEventArgs args, DiscordMember member)
    {
        if (args.Channel.Id != Config.LogsChannel) return;

        if (args.Message.Attachments.Any(i => i.Width.HasValue)) return; //Has a pic.

        await args.Message.DeleteAsync();
        await member.SendMessageAsync($"You have to post at least one pic on the {args.Channel.Mention}!");
    }

    private static async Task DontAtMe(MessageCreateEventArgs args)
    {
        var message = await args.Channel.GetMessageAsync(args.Message.Id);
        if (message.ReferencedMessage != null) return;
        await ApplicationCommand.DeleteTimeoutMessage(message);
    }

    internal static async Task CanViewChannel(DiscordChannel channel, bool canView)
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
        try
        {
            await args.Context.DeferAsync();
        }
        catch
        {

        }

        var exception = args.Exception;
        if (exception is SlashExecutionChecksFailedException)
        {
            //await args.Context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Wait for 1 min to use this command again!"));
            return;
        }

        var dev = await SendException(exception, args.Context.Member.Mention);

        try
        {
            await args.Context.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Errored! Already sent the error to {dev?.Mention ?? "unknown"}, Please wait for their check!"));
        }
        catch
        {

        }
    }

    private static async Task Client_GuildMemberRemoved(DiscordClient sender, GuildMemberRemoveEventArgs args)
    {
        //Invalid Supporter.
        await SqlHelper.IsvalidSupporter(args.Member.Id, false);
        await SupporterCommands.UpdateNames();
        await SupporterCommands.UpdateHashes();

        //Invalid Rotation Dev.
        if (SqlHelper.GetChannelId(args.Member.Id, out var data))
        {
            var channel = args.Guild.GetChannel(data);

            await CanViewChannel(channel, false);
            await channel.DeleteOverwriteAsync(args.Member, $"{args.Member.DisplayName} left this guild!");
        }
    }

    internal static async void SendGithubPublish(string s)
    {
        try
        {
            var obj = JObject.Parse(s);
            if (obj["action"]?.ToString() != "published") return;

            var repository = obj["repository"];
            if (repository == null) return;

            var url = repository["html_url"]?.ToString();
            var name = repository["name"]?.ToString();
            var body = obj["release"]?["body"]?.ToString();

            if (body != null)
            {
                var lines = body.Split('\n');
                body = string.Join('\n', lines.Take(30));
            }

            var owner = repository["owner"];
            var icon = owner?["avatar_url"]?.ToString();
            var login = owner?["login"]?.ToString();
            var authorUrl = owner?["html_url"]?.ToString();

            var isRS = name?.Equals("RotationSolver", StringComparison.OrdinalIgnoreCase) ?? false;
            var channel = await Client.GetChannelAsync(isRS ? Config.AnnounceMent : Config.RotationAnnounceMentChannel);

            var embedBuilder = new DiscordEmbedBuilder()
                .WithAuthor(login, authorUrl, icon)
                .WithTitle($"**{name} Released!**")
                .WithUrl(url)
                .WithDescription(body)
                .WithColor(DiscordColor.Black);

            var role = channel.Guild.GetRole(Config.AnnouncementSubRole);

            var content = role.Mention;
            if (isRS)
            {
                var threadIds = SqlHelper.GetFixedIssue();

                var ideaChannel = await Client.GetChannelAsync(Config.KnownIdeasChannel);
                var bugChannel = await Client.GetChannelAsync(Config.KnownIssueChannel);

                var result = await ideaChannel.Guild.ListActiveThreadsAsync();


                foreach (var threadId in threadIds)
                {
                    var thread = result.Threads.FirstOrDefault(t => t.Id == threadId);

                    if (thread == null) continue;
                    //TODO: close thread.

                    try
                    {
                        if (!SqlHelper.GetIssueData(thread.Id, out var messageId)) continue;
                        content += "\n- " + thread.Mention;

                        var deleteMessage = await ideaChannel.GetMessageAsync(messageId)
                            ?? await bugChannel.GetMessageAsync(messageId);
                        await deleteMessage.DeleteAsync();
                    }
                    finally
                    {
                        await SqlHelper.DeleteIssueData(thread.Id);
                    }
                }

                foreach (var thread in channel.Guild.Threads.Values)
                {
                    if (thread.ParentId != Config.PreReleaseChannel) continue;
                    //TODO: close thread.
                }
            }

            await channel.SendMessageAsync(new DiscordMessageBuilder().WithContent(content).AddEmbed(embedBuilder));
        }
        catch(Exception ex)
        {
            await SendException(ex);
        }
    }

    internal static async void SendKofi(string s)
    {
        try
        {
            s = HttpUtility.UrlDecode(s[5..]);
            var obj = JObject.Parse(s);
            var name = obj["from_name"]?.ToString();
            var amount = obj["amount"]?.ToString();
            var currency = obj["currency"]?.ToString();

            var channel = await Client.GetChannelAsync(Config.KofiChannel);

            await channel.SendMessageAsync($"Thank you **{name}** for donating {currency} {amount}! :sparkling_heart:");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + '\n' + (ex.StackTrace ?? string.Empty));
            return;
        }
    }

    internal static async void SendPatreon(string s)
    {
        try
        {
            var obj = JObject.Parse(s);
            var data = obj["data"];
            if (data is null) return;

            if (data["type"]?.ToString() is not "member") return;

            var attr = data["attributes"];
            if (attr is null) return;

            var name = attr["full_name"]?.ToString();
            var amount = double.Parse(attr["pledge_amount_cents"]?.ToString() ?? "0");

            if (amount == 0) return;

            amount /= 100;
            var currency = attr["campaign_currency"]?.ToString();

            var channel = await Client.GetChannelAsync(Config.KofiChannel);

            await channel.SendMessageAsync($"Thank you **{name}** for donating {currency} {amount:f2}! :sparkling_heart:");
        }
        catch (Exception? ex)
        {
            await SendException(ex);
        }
    }

    internal static async Task<DiscordChannel?> SendException(Exception? ex, string mention = "")
    {
        var dev = await Client.GetChannelAsync(Config.ModeratorChannel);
        if (dev == null)
        {
            return null;
        }

        var str = string.Empty;
        while (ex != null)
        {
            str += ex.Message + "\n" + (ex.StackTrace ?? string.Empty) + "\n \n";
            ex = ex.InnerException;
        }
        if(!string.IsNullOrEmpty(str))
        {
            str += "\nFrom " + mention;
        }
       
        await dev.SendMessageAsync(str[..Math.Min(str.Length - 1, 2000)]);

        return dev;
    }
}
