using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace RotationSolver.DiscordBot.SlashCommands;

[SlashCommandGroup("Admin", "The commands for administrators")]
[SlashCommandPermissions(DSharpPlus.Permissions.Administrator)]
internal class AdministratorCommands : ApplicationCommandModule
{
    [SlashCommand("Event", "Hold an event.")]
    public async Task HoldEvent(InteractionContext ctx,
        [Option("Description", "The description of the event.")] string description,
        [Option("Time", "The Unix Time Stamp https://www.unixtimestamp.com/")] long timeStamp,
        [Option("DutyName", "The Duty Name")] string dutyName,
        [Option("Link", "The tutorial link for this duty")] string link = "")
    {
        await ctx.DeferAsync();

        var channel = ctx.Guild.GetChannel(Config.HappyBunnyChannel);

        var item = Resource.DutyAndImage.MinBy(i => LevenshteinDistance(i.Item1.ToLower(), dutyName.ToLower()));

        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timeStamp);

        var now = DateTime.UtcNow.AddMinutes(10);
        if (dateTime < now)
        {
            dateTime = now;
            timeStamp = new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }

        description = $"<t:{timeStamp}:F> <t:{timeStamp}:R>\n## [{item.Item1}](https://garlandtools.org/db/#instance/{item.Item3})\n{description}";

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

        if(!string.IsNullOrEmpty(link))
        {
            embed = embed.WithUrl(link);
        }

        await channel.SendMessageAsync(new DiscordMessageBuilder()
            .WithContent($"Hi, {ctx.Guild.GetRole(Config.HappyBunnyRole).Mention}! Add the emoji at this message to join this event!")
            .AddEmbed(embed));

        await ctx.DeleteResponseAsync();
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
