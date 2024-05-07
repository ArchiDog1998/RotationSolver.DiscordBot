using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace RotationSolver.DiscordBot.SlashCommands;


public class SupporterAttribute : BotChannelAttribute
{
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        if (!await base.ExecuteChecksAsync(ctx)) //Wrong channel.
        {
            return false;
        }
        else if (!ctx.Member.Roles.Any(r => r.Id == Config.SupporterRole)) //Worng role.
        {
            await ctx.DeferAsync();

            var builder = new DiscordEmbedBuilder()
            {
                Title = "Be a **Supporter**!",
                Url = "https://ko-fi.com/B0B0IN5DX",
                ImageUrl = "https://storage.ko-fi.com/cdn/brandasset/kofi_bg_tag_dark.png",
                Color = DiscordColor.IndianRed,
                Description = "You dont have the role Supporter!\n \n"
                    + "If you have supported, please provide your reciept and DM to ArchiTed!",
                Footer = new() { Text = "It just costs $2!" },
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(builder));
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
    [Supporter]
    [SlashCooldown(1, 60, SlashCooldownBucketType.User)]
    [SlashCommand("Name", "Adds your name to the ingame plugin supporter list if you are one.")]
    public async Task SupporterName(InteractionContext ctx,
    [Option("DisplayName", "To display your name at the game plugin", true)] string name)
    {
        await ctx.DeferAsync();

        SqlHelper.UpdateSupporterData(ctx.Member.Id, 0, string.Empty, name);
        await UpdateNames();

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Changed your name."));
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

    [Supporter]
    [SlashCooldown(1, 60, SlashCooldownBucketType.User)]
    [SlashCommand("Hash", "Adds your hash to the supporter list to access the plugins supporter-only features.")]
    public async Task SupporterHash(InteractionContext ctx,
    [Option("Hash", "That is shown in the Debug panel in the game.", true)] string hash)
    {
        await ctx.DeferAsync();

        if (hash.Length != 24 || !hash.EndsWith("=="))
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Invalid hash! Please get it in the `Debug` panel!"));
            return;
        }

        SqlHelper.UpdateSupporterData(ctx.Member.Id, 0, hash, string.Empty);
        await UpdateHashes();
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Changed your hash. Please reload the RS plugin after about 10 mins."));
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

    [Supporter]
    [SlashCommand("Info", "Get Your Information privately.")]
    public static async Task SupporterInfo(InteractionContext ctx)
    {
        await ctx.DeferAsync();

        var id = ctx.Member.Id;
        var hasData = SqlHelper.GetName(id, out var value);

        if (!hasData)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Didn't find your data! Please add your infomation!"));
            return;
        }

        var embedItem = new DiscordEmbedBuilder()
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
