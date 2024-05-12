using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    foreach (var guild in Service.Client.Guilds.Values)
                    {
                        await UpdateContributorRoles(guild);
                    }
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


}
