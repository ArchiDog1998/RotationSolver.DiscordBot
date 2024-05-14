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
            var message = await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbeds(embeds));
            await message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(Service.Client, Config.RotationSolverIcon));
        }
    }
}
