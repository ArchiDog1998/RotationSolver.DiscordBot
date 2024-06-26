using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json.Linq;

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

                var guildId = ulong.Parse(obj["guild_id"]!.ToString());
                var guild = await sender.GetGuildAsync(guildId);

                switch (int.Parse(obj["action_type"]!.ToString()))
                {
                    case 25://Role change
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
                        tarId = ulong.Parse(obj["target_id"]!.ToString());
                        if (!guild.Threads.TryGetValue(tarId, out var thread)) break;

                        changes = obj["changes"] as JArray;
                        foreach (var change in changes!)
                        {
                            switch (change["key"]!.ToString())
                            {
                                case "applied_tags":
                                    var newValues = change["new_value"] as JArray;

                                    var ids = newValues!.Select(i => ulong.Parse(i!.ToString()));

                                    var oldValues = change["old_value"] as JArray;

                                    var changedIds = ids.Except(oldValues!.Select(i => ulong.Parse(i!.ToString())));

                                    await OnApplyTag(guild, thread, [..ids], [.. changedIds]);
                                    break;
                            }
                        }
                        break;

                    case 110: //Create the post
                        tarId = ulong.Parse(obj["target_id"]!.ToString());

                        var threads = await guild.ListActiveThreadsAsync();

                        thread = threads.Threads.FirstOrDefault(thread => thread.Id == tarId);

                        if (thread == null) break;
                        var messages = await thread.GetMessagesAsync(1);

                        if (messages.Count == 0) break;
                        var message = messages[0];
                        if (message == null) break;

                        await Service.AddAnimatedLogo(guild, message);

                        break;
                }
                break;
        }
    }

    private static async Task OnApplyTag(DiscordGuild guild, DiscordThreadChannel thread, ulong[] ids, ulong[] changedIds)
    {
        if (changedIds.Length == 0) return;

        ulong channelId;
        if (ids.Contains(Config.EnhancementTag)
            || ids.Contains(Config.FeaturesTag))
        {
            channelId = Config.KnownIdeasChannel;
        }
        else if(ids.Contains(Config.BugsTag))
        {
            channelId = Config.KnownIssueChannel;
        }
        else
        {
            return;
        }

        var channel = guild.GetChannel(channelId);
        if (channel == null) return;

        var hasMessage = SqlHelper.GetIssueData(thread.Id, out var messageIds);
        var addMessage = changedIds.Contains(Config.ConfirmedTag) && !changedIds.Contains(Config.CompletedTag) && !changedIds.Contains(Config.WontFixTag);
        var deleteMessage = changedIds.Contains(Config.WontFixTag);

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
                await Service.SendSupportThank(member);
            }
        }
        if (roles.Any(i => i is Config.KofiRole or Config.PatreonRole))
        {
            if (isAdd)
            {
                await Service.SendSubscribeThank(member);
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
                    Description = $"Hello {member.Mention}, thank you for being interested about making a rotation for Rotation Solver, you can use {channel.Mention} to receive feedback from the other players that want to use your rotation if you wish to. \n\nYou'd better to add the `Download Link` of your libraries to the `Post Guidelines`!\nYou can also add the [webhooks](https://docs.github.com/en/webhooks/using-webhooks/creating-webhooks) to your repo.\n{Config.PublishLink} for release.\n {Config.PushLink} for push.\nHave fun!",
                    Footer = new() { Text = "Come on! You are the best!" },
                }
                .WithThumbnail("https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/Images/Logo.png");


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
