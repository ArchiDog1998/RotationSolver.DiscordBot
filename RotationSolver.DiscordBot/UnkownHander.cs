using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json.Linq;
using RotationSolver.DiscordBot.SlashCommands;

namespace RotationSolver.DiscordBot;
internal static class UnkownHander
{
    public static void Init()
    {
        Service.Client.UnknownEvent += Client_UnknownEvent;
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

                    case 111: //Tag Modified.
                        guildId = ulong.Parse(obj["guild_id"]!.ToString());
                        guild = await sender.GetGuildAsync(guildId);

                        tarId = ulong.Parse(obj["target_id"]!.ToString());
                        if (!guild.Threads.TryGetValue(tarId, out var thread)) break;

                        changes = obj["changes"] as JArray;
                        foreach (var change in changes!)
                        {
                            var newValues = change["new_value"] as JArray;

                            var ids = newValues!.Select(i => ulong.Parse(i!.ToString()));

                            var oldValues = change["old_value"] as JArray;

                            ids = ids.Except(oldValues!.Select(i => ulong.Parse(i!.ToString())));

                            switch (change["key"]!.ToString())
                            {
                                case "applied_tags":
                                    await OnApplyTag(guild, thread, [.. ids]);
                                    break;
                            }
                        }
                        break;
                }
                break;
        }
    }

    private static async Task OnApplyTag(DiscordGuild guild, DiscordThreadChannel thread, ulong[] ids)
    {
        if (ids.Length == 0) return;

        var channel = guild.GetChannel(Config.KnownIssueChannel);
        if (channel == null) return;

        var hasMessage = SqlHelper.GetIssueData(thread.Id, out var messageIds);
        var addMessage = ids.Contains(Config.ConfirmedTag) && !ids.Contains(Config.CompletedTag) && !ids.Contains(Config.WontFixTag);
        var deleteMessage = ids.Contains(Config.CompletedTag) || ids.Contains(Config.WontFixTag);

        if (addMessage && !hasMessage)
        {
            var message = await channel.SendMessageAsync($"## {thread.Mention}");
            SqlHelper.InsertIssueData(thread.Id, message.Id);
        }
        else if (deleteMessage && hasMessage && messageIds.Length == 1)
        {
            var message = await channel.GetMessageAsync(messageIds[0]);
            await channel.DeleteMessageAsync(message);
            SqlHelper.DeleteIssueData(thread.Id);
        }
    }

    private static async Task OnRoleChanged(DiscordGuild guild, DiscordMember member, ulong[] roles, bool isAdd)
    {
        if (roles.Any(i => i == Config.SupporterRole))
        {
            if (isAdd)
            {
                var builder = new DiscordEmbedBuilder()
                {
                    Title = "**Thanks for your support!**",
                    Color = DiscordColor.IndianRed,
                    Description = $"**Hi {member.Mention}, Thanks for your support!**",
                    Footer = new() { Text = "Thank you so much!" },
                };

                await member.SendMessageAsync(builder);
                await SqlHelper.InitName(member.Id, member.DisplayName);
            }
        }
        if (roles.Any(i => i is Config.KofiRole or Config.PatreonRole))
        {
            if (isAdd)
            {
                SqlHelper.IsvalidSupporter(member.Id, true);

                var builder = new DiscordEmbedBuilder()
                {
                    Title = "**Thanks for your support!**",
                    Color = DiscordColor.IndianRed,
                    Description = $"**Hi {member.Mention}, Thank you for the support!**\nPlease don't forget to go {Config.BotChannelLink} and use `/supporter hash` to enable your supporter-only features!",
                    Footer = new() { Text = "Thank you so much!" },
                };

                await member.SendMessageAsync(builder);

                await SupporterCommands.UpdateHashes();
                await SqlHelper.InitName(member.Id, member.DisplayName);
            }
            else
            {
                SqlHelper.IsvalidSupporter(member.Id, false);
            }
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
                    channel = await guild.CreateChannelAsync($"🌱┃{member.DisplayName} Rotations", ChannelType.GuildForum, parent, $"Talk about everything about the rotations from {member.Mention}!", overwrites: [p]);

                    SqlHelper.UpdateRotationDevChannel(member.Id, channel.Id);

                    await guild.GetChannel(Config.RotationAnnounceMentChannel).ModifyPositionAsync(0);
                }
                else
                {
                    await Service.CanViewChannel(channel, true);
                    await channel.AddOverwriteAsync(member, permissions, reason: $"{member.DisplayName} becomes a rotation developer!");
                }

                var builder = new DiscordEmbedBuilder()
                {
                    Title = "**You are Rotation Developer Now!**",
                    Color = DiscordColor.CornflowerBlue,
                    Description = $"Hello {member.Mention}, thank you for being interested about making a rotation for Rotation Solver, you can use {channel.Mention} to receive feedback from the other players that want to use your rotation if you wish to. \nYou'd better to add the `Download Link` of your libraries to the `Post Guidelines`!\nHave fun!",
                    Footer = new() { Text = "Come on! You are the best!" },
                };

                await member.SendMessageAsync(builder);
            }
            else
            {
                if (channel != null)
                {
                    await Service.CanViewChannel(channel, false);
                    await channel.DeleteOverwriteAsync(member, $"{member.DisplayName} is not a rotation developer!");
                }
            }
        }
    }
}
