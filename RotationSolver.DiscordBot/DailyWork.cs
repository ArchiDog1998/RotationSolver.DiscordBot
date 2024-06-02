using DSharpPlus.Entities;

namespace RotationSolver.DiscordBot;

internal class DailyWork
{
    public static void Init()
    {
        _ = Task.Run(async () =>
        {
            var now = DateTime.UtcNow + TimeSpan.FromHours(10);
            var span = TimeSpan.FromDays(1) - (now - now.Date);

            await Task.Delay(span);

            var timer = new PeriodicTimer(TimeSpan.FromDays(1));

            do
            {
                try
                {
                    await SendGithubMessage();
                    await UpdateSupporterName();
                }
                catch
                {

                }
            }
            while (await timer.WaitForNextTickAsync());
        });
    }

    private static async Task SendGithubMessage()
    {
        var channel = await Service.Client.GetChannelAsync(Config.GithubChannel);
        var embeds = await GithubHelper.GetCommitMessage();
        if (embeds.Length != 0)
        {
            var emoji = DiscordEmoji.FromGuildEmote(Service.Client, Config.RotationSolverIcon);
            foreach (var embed in embeds) 
            {
                var message = await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed));
                await message.CreateReactionAsync(emoji);
            }
        }
    }

    private static async Task UpdateSupporterName()
    {
        var guild = await Service.Client.GetGuildAsync(Config.ServerId);

        if (guild == null) return;

        foreach (var id in SqlHelper.GetDiscordIds())
        {
            var member = await guild.GetMemberAsync(id);
            if (member == null) continue;
            await SqlHelper.InitName(member);
        }
    }
}
