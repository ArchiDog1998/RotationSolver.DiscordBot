using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace RotationSolver.DiscordBot.SlashCommands;

public class HappyBunnyChannelAttribute : SlashCheckBaseAttribute
{
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        if (ctx.Channel.Id != Config.HappyBunnyChannel) //Wrong channel.
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent($"Sorry, {ctx.Member.Mention}. This feature can only be used in {Config.HappyBunnyChannelLink}!"));

            await Task.Delay(10000);
            await ctx.DeleteResponseAsync();
            return false;
        }
        return true;
    }
}

[SlashCommandGroup("Bunny", "The commands for administrators")]
[HappyBunnyChannel]
internal class BunnyCommands : ApplicationCommandModule
{
    public enum PartyType
    {
        [ChoiceName("Normal")]
        Normal,

        [ChoiceName("Unlimited")]
        Unlimited,
    }
    
    [SlashCommand("Event", "Hold an event.")]
    public async Task HoldEvent(InteractionContext ctx,
        [Option("DutyName", "The Duty Name")] string dutyName = Resource.DefaultDuty,
        [Option("Description", "The description of the event.")] string description = "",
        [Option("Time", "The Unix Time Stamp https://www.unixtimestamp.com/")] long timeStamp = 0,
        [Option("Duration", "The duration in minutes")] long duration = 60,
        [Option("PartyType", "The Type of the party")] PartyType partyType = PartyType.Normal,
        [Option("Link", "The tutorial link for this duty")] string link = "")
    {
        await ctx.DeferAsync();

        var channel = ctx.Guild.GetChannel(Config.HappyBunnyEventChannel);

        var item = Resource.DutyAndImage.MinBy(i => LevenshteinDistance(i.Item1.ToLower(), dutyName.ToLower()));

        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timeStamp);

        var now = DateTime.UtcNow.AddMinutes(10);
        if (dateTime < now)
        {
            dateTime = now;
            timeStamp = new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }

        var name = item.Item1;

        if (!string.IsNullOrEmpty(link))
        {
            name = $"[{name}]({link})";
        }

        description = $"<t:{timeStamp}:F> <t:{timeStamp}:R>\nDuration: `{duration} minutes`\n## {name}\n{description}";

        switch (partyType)
        {
            case PartyType.Normal:
                var tank = await ctx.Guild.GetEmojiAsync(Config.TankEmoji);
                var healer = await ctx.Guild.GetEmojiAsync(Config.HealerEmoji);
                var dps = await ctx.Guild.GetEmojiAsync(Config.DpsEmoji);
                description += $"\n{tank} {item.Item3} {healer} {item.Item4} {dps} {item.Item5}";
                break;

            case PartyType.Unlimited:
                var any = await ctx.Guild.GetEmojiAsync(Config.AnyRoleEmoji);
                description += $"\n{any} {item.Item3 + item.Item4 + item.Item5}";
                break;
        }

        var embed = new DiscordEmbedBuilder()
            .WithTitle(EventHander.EventTittle)
            .WithDescription(description)
            .WithFooter("Materia - Sophia")
            .WithTimestamp(dateTime)
            .WithThumbnail("https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/Images/Logo.png")
            .WithColor(DiscordColor.Yellow)
            .WithAuthor(((DiscordMember)ctx.User).DisplayName, iconUrl: ctx.User.AvatarUrl);

        var thumbnailId = item.Item2;
        if (thumbnailId > 0)
        {
            var lead = thumbnailId / 1000;
            embed = embed.WithImageUrl($"https://xivapi.com/i/{lead:D3}000/{thumbnailId:D6}_hr1.png");
        }

        var messageStr = $"Hi, {ctx.Guild.GetRole(Config.HappyBunnyRole).Mention}!\nThe next event **{name}** will be hold <t:{timeStamp}:R>.\nIf you are interested, please add the job emoji at this message to join this event!";

        var message = await channel.SendMessageAsync(new DiscordMessageBuilder()
            .WithContent(messageStr)
            .AddEmbed(embed));

        await ctx.DeleteResponseAsync();

        await Task.Delay(dateTime - DateTime.UtcNow.AddMinutes(5));

        message = await channel.GetMessageAsync(message.Id);

        if (message == null) return;

        if (IsPartyFull(message, item.Item3, item.Item4, item.Item5, partyType, out var chocen, out var drop))
        {
            messageStr = $"Let's rock for the **{name}**!\n{chocen}";

            if (!string.IsNullOrEmpty(drop)) 
            {
                messageStr += "\n\nSorry, this event is full. We look forward to your next participation!\n" + drop;
            }

            var newMessage = await message.RespondAsync(messageStr);

            await Task.Delay(dateTime.AddMinutes(duration) - DateTime.UtcNow);
            await channel.SendMessageAsync($"Thank you for participating in the event **{name}**! Hope you had a great time!\n{chocen}");
            await channel.DeleteMessageAsync(newMessage);
        }
        else if(!string.IsNullOrEmpty(chocen))
        {
            await channel.SendMessageAsync($"Because there are not enough people, the event **{name}** is canceled. Thank you for your participation.\n{chocen}");
        }

        await channel.DeleteMessageAsync(message);
    }

    private static bool IsPartyFull(DiscordMessage message, byte tankCount, byte healerCount, byte dpsCount, PartyType type, out string chocen, out string dropped)
    {
        dropped = string.Empty;
        chocen = string.Empty;
        if(message.Embeds.Count == 0) return false;
        var fields = message.Embeds[0].Fields;
        if (fields == null || !fields.Any()) return false;
        chocen = string.Join(", ", fields.SelectMany(i => i.Value.Split("\n")));

        switch (type)
        {
            case PartyType.Normal:
                
                var tanks = fields.Where(f => f.Name == JobCate.Tank.ToString()).SelectMany(i => i.Value.Split("\n")).ToList();
                if (tanks.Count < tankCount) return false;

                var healers = fields.Where(f => f.Name == JobCate.Healer.ToString()).SelectMany(i => i.Value.Split("\n")).ToList();
                if (healers.Count < healerCount) return false;

                string[] dpsName = [JobCate.Melee.ToString(), JobCate.Range.ToString(), JobCate.Caster.ToString()];
                var dps = fields.Where(f => dpsName.Contains(f.Name)).SelectMany(i => i.Value.Split("\n")).ToList();
                if (dps.Count < dpsCount) return false;

                Shuffle(dps);

                chocen = string.Join(", ", tanks.Take(tankCount).Union(healers.Take(healerCount)).Union(dps.Take(dpsCount)));
                dropped = string.Join(", ", tanks.Skip(tankCount).Union(healers.Skip(healerCount)).Union(dps.Skip(dpsCount)));
                return true;

            case PartyType.Unlimited:
                var member = chocen.Split(", ").ToList();
                if (member.Count < tankCount + healerCount + dpsCount) return false;

                Shuffle(member);

                chocen = string.Join(", ", member.Take(tankCount + healerCount + dpsCount));
                dropped = string.Join(", ", member.Skip(tankCount + healerCount + dpsCount));
                return true;

            default:
                return false;
        }
    }

    private static readonly Random rng = new ();

    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public static int LevenshteinDistance(string s, string t)
    {
        if (string.IsNullOrEmpty(s))
        {
            if (string.IsNullOrEmpty(t))
                return 0;
            return t.Length;
        }

        if (string.IsNullOrEmpty(t))
        {
            return s.Length;
        }

        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        // initialize the top and right of the table to 0, 1, 2, ...
        for (int i = 0; i <= n; d[i, 0] = i++) ;
        for (int j = 1; j <= m; d[0, j] = j++) ;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                int min1 = d[i - 1, j] + 1;
                int min2 = d[i, j - 1] + 1;
                int min3 = d[i - 1, j - 1] + cost;
                d[i, j] = Math.Min(Math.Min(min1, min2), min3);
            }
        }
        return d[n, m];
    }
}
