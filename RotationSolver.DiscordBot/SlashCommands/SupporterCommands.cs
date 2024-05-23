using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace RotationSolver.DiscordBot.SlashCommands;

public class SupporterCheckAttribute(params ulong[] roleIds) : BotChannelAttribute
{
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        if (! await base.ExecuteChecksAsync(ctx)) return false;
        else if (!ctx.Member.Roles.Any(r => roleIds.Contains(r.Id))) //Worng role.
        {
            var roles = roleIds.Select(id => ctx.Guild.GetRole(id).Name);

            var builder = new DiscordEmbedBuilder()
            {
                Title = "Be a **Supporter**!",
                Url = "https://www.patreon.com/ArchiDog1998",
                ImageUrl = "https://c7.patreon.com/https%3A%2F%2Fwww.patreon.com%2F%2Fcreator-teaser-image%2F7803473/selector/%23creator-teaser%2C.png",
                Color = DiscordColor.IndianRed,
                Description = $"Hi, {ctx.Member.Mention}! You dont have any of the following roles: {string.Join(", ", roles)}!\n \n"
                    + "If you have supported, please provide your reciept and DM to ArchiTed!",
                Footer = new() { Text = "It's just $2!" },
            };
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(builder));
            return false;
        }
        else
        {
            return true;
        }
    }
}

[SlashCommandGroup("Supporter", "The commands for supporters")]
public class SupporterCommands : ApplicationCommandModule
{
    [SlashCooldown(5, 600, SlashCooldownBucketType.User)]
    [SupporterCheck(Config.SupporterRole, Config.KofiRole, Config.PatreonRole)]
    [SlashCommand("Name", "Adds your name to the ingame plugin supporter list if you are one.")]
    public async Task SupporterName(InteractionContext ctx,
    [Option("DisplayName", "Display your name in the game plugin")] string name)
    {
        await ctx.DeferAsync();

        SqlHelper.UpdateSupporterData(ctx.Member.Id, string.Empty, name);
        await UpdateNames();

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Changed your ingame display name."));
    }

    internal static async Task UpdateNames()
    {
        var supporters = SqlHelper.GetNames();

        await GithubHelper.ModifyFile<HashSet<string>>("RotationSolver", "Resources/Supporters.json", ModifySupporters);

        bool ModifySupporters(ref HashSet<string>? value, out string commit)
        {
            commit = GithubHelper.ModifySupporterName;
            value ??= [];

            var delete = value.Except(supporters);
            var added = supporters.Except(value);

            if (!delete.Any() && !added.Any()) return false;

            foreach (var item in delete)
            {
                value.Remove(item);
            }

            foreach (var item in added)
            {
                value.Add(item);
            }
            return true;
        }
    }

    [SlashCooldown(5, 600, SlashCooldownBucketType.User)]
    [SupporterCheck(Config.KofiRole, Config.PatreonRole)]
    [SlashCommand("Hash", "Adds your hash to the supporter list to access the plugins supporter-only features.")]
    public async Task SupporterHash(InteractionContext ctx,
    [Option("Hash", "That is shown in the Debug panel in the addon's ingame menu.")] string hash)
    {
        await ctx.DeferAsync();

        if (hash.Length != 24 || !hash.EndsWith("=="))
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Invalid hash! Please get it from the `Debug` panel!"));
            return;
        }

        SqlHelper.UpdateSupporterData(ctx.Member.Id, hash, string.Empty);
        await UpdateHashes();
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Changed your hash. Please reload the RS plugin in about 10 mins."));
    }

    internal static async Task UpdateHashes()
    {
        var supportersHash = SqlHelper.GetHashes();

        await GithubHelper.ModifyFile<HashSet<string>>("RotationSolver", "Resources/SupportersHash.json", ModifySupportersHash);


        bool ModifySupportersHash(ref HashSet<string>? value, out string commit)
        {
            commit = GithubHelper.ModifySupporterHash;
            value ??= [];

            var delete = value.Except(supportersHash);
            var added = supportersHash.Except(value);

            if (!delete.Any() && !added.Any()) return false;

            foreach (var item in delete)
            {
                value.Remove(item);
            }

            foreach (var item in added)
            {
                value.Add(item);
            }
            return true;
        }
    }

    [SupporterCheck(Config.SupporterRole, Config.KofiRole, Config.PatreonRole)]
    [SlashCommand("Info", "Get your information privately in a DM.")]
    public static async Task SupporterInfo(InteractionContext ctx)
    {
        await ctx.DeferAsync();

        var id = ctx.Member.Id;
        var hasData = SqlHelper.GetName(id, out var value);

        if (!hasData)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Didn't find your data! Please add your information first to the database!"));
            return;
        }

        var embedItem = new DiscordEmbedBuilder()
            .WithTitle("Your Information in the database.")
            .WithColor(DiscordColor.Blue);

        if (value.Length != 0)
        {
            var v = value[0];

            if (!string.IsNullOrEmpty(v))
            {
                embedItem = embedItem.AddField("Display Name", v);
            }
        }

        if (SqlHelper.GetHash(id, out value) && value.Length != 0)
        {
            embedItem = embedItem.AddField("Hashes", string.Join("\n", value));
        }

        await ctx.Member.SendMessageAsync(embed: embedItem);
        await ctx.DeleteResponseAsync();
    }
}
