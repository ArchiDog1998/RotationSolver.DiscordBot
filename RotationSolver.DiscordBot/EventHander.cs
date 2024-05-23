using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace RotationSolver.DiscordBot;

internal enum JobCate : byte
{
    None,

    Tank,
    Healer,
    Melee,
    Range,
    Caster,
    Limited,
}

internal static class EventHander
{
    internal const string EventTittle = "🤪 Happy Bunny Event 🤪";

    private static readonly Dictionary<string, JobCate> JobCategory = new ()
    {
        {"Warrior", JobCate.Tank},
        { "Paladin", JobCate.Tank },
        {"Gunbreaker", JobCate.Tank },
        {"DarkKnight", JobCate.Tank },

        {"Scholar", JobCate.Healer},
        {"WhiteMage", JobCate.Healer},
        {"Sage", JobCate.Healer },
        {"Astrologian", JobCate.Healer },

        {"Monk", JobCate.Melee},
        {"Samurai", JobCate.Melee},
        {"Reaper", JobCate.Melee },
        {"Ninja", JobCate.Melee },
        {"Dragoon", JobCate.Melee },

        { "Dancer", JobCate.Range },
        {"Machinist", JobCate.Range },
        {"Bard", JobCate.Range },

        {"Summoner", JobCate.Caster },
        {"RedMage", JobCate.Caster },
        {"BlackMage", JobCate.Caster},

        {"BlueMage", JobCate.Limited },
    };

    public static void Init()
    {
        Service.Client.MessageReactionAdded += Client_MessageReactionAdded;
        Service.Client.MessageReactionRemoved += Client_MessageReactionRemoved;
    }

    private static async Task Client_MessageReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs args)
    {
        var message = await args.Channel.GetMessageAsync(args.Message.Id);

        if (!IsMessageValid(message)) return;
        var cate = GetCateFromEmoji(args.Emoji);
        if (cate == JobCate.None) return;

        var rawEmbed = message.Embeds[0];
        var field = GetField(rawEmbed, cate, out var index);
        if (field == null) return;
        var embed = new DiscordEmbedBuilder(rawEmbed);

        var name = CreateOneField(args.Emoji, args.User);

        field.Value = string.Join("\n", field.Value.Split("\n").Where(s => s != name));

        if (string.IsNullOrEmpty(field.Value))
        {
            embed.RemoveFieldAt(index);
        }

        await args.Message.ModifyAsync(embed.Build());
    }

    private static async Task Client_MessageReactionAdded(DiscordClient sender, MessageReactionAddEventArgs args)
    {
        var message = await args.Channel.GetMessageAsync(args.Message.Id);
        if (!IsMessageValid(message)) return;
        var cate = GetCateFromEmoji(args.Emoji);
        if (cate == JobCate.None) return;

        var rawEmbed = message.Embeds[0];

        if (rawEmbed.Fields != null)
        {
            foreach (var item in rawEmbed.Fields)
            {
                if (item.Value.Contains(args.User.Mention))
                {
                    await message.DeleteReactionAsync(args.Emoji, args.User);
                    return;
                }
            }
        }

        var name = CreateOneField(args.Emoji, args.User);
        var field = GetField(rawEmbed, cate, out _);

        var embed = new DiscordEmbedBuilder(rawEmbed);

        if (field == null)
        {
            (string, string)[] fields = rawEmbed.Fields == null 
                ? [(cate.ToString(), name)]
                : [..rawEmbed.Fields.Select(f => (f.Name, f.Value)).Append((cate.ToString(), name))];

            embed.ClearFields();

            foreach (var f in fields.OrderBy(f => Enum.Parse<JobCate>(f.Item1)))
            {
                embed.AddField(f.Item1, f.Item2, true);
            }
        }
        else
        {
            field.Value += "\n" + name;
        }

        await args.Message.ModifyAsync(embed.Build());
    }

    private static DiscordEmbedField? GetField(DiscordEmbed embed, JobCate cate, out int index)
    {
        index = 0;
        if (embed.Fields == null) return null;

        foreach (var field in embed.Fields)
        {
            if (field.Name == cate.ToString())
            {
                return field;
            }
            index++;
        }
        return null;
    }

    private static string CreateOneField(DiscordEmoji emoji, DiscordUser user)
    {
        return $"{emoji} {user.Mention}";
    }

    private static JobCate GetCateFromEmoji(DiscordEmoji emoji)
    {
        if(!emoji.Name.StartsWith("Job")) return JobCate.None;
        var name = emoji.Name[3..];
        if (!JobCategory.TryGetValue(name, out var cate)) return JobCate.None;

        return cate;
    }

    private static bool IsMessageValid(DiscordMessage message)
    {
        if (message.Author?.Id != Service.Client.CurrentUser.Id) return false;
        if (message.Channel.Id != Config.HappyBunnyChannel) return false;
        if (message.Embeds.Count == 0) return false;
        var embed = message.Embeds[0];

        if (embed.Title != EventTittle) return false;
        return true;
    }
}
