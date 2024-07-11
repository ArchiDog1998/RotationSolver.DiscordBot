using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RotationSolver.DiscordBot.Npgsql;
using RotationSolver.DiscordBot.SlashCommands;
using System.Diagnostics.CodeAnalysis;

namespace RotationSolver.DiscordBot;

internal static class SqlHelper
{
    private static NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(Config.PostgreSQL);
    }

    public static async Task<CommitItem[]> GetAndClearCommits()
    {
        using var connect = new PostgreContext();

        CommitItem[] data = [.. connect.GithubCommit];

        await connect.Database.ExecuteSqlRawAsync("TRUNCATE public.\"GithubCommit\"");

        await connect.SaveChangesAsync();

        return data;
    }

    public static async Task InsertGithubCommit(string sha, long repoId)
    {
        using var connect = new PostgreContext();

        connect.GithubCommit.Add(new() { Sha = sha, Repo = repoId, });

        await connect.SaveChangesAsync();
    }

    public static void UpdateSupporterData(ulong id, string hash, string name)
    {
        SetValues($"CALL public.upsert_supporter({id}, {hash.GetValue()}, {name.GetValue()})");
        if (!string.IsNullOrEmpty(hash))
        {
            IsvalidSupporter(id, true);
        }
    }

    public static void FixedIssueData(ulong threadId)
    {
        SetValues($"UPDATE public.\"Issues\" SET \"Fixed\"=true WHERE \"ThreadID\" = {threadId};");
    }

    public static void InsertIssueData(ulong threadId, ulong messageId)
    {
        SetValues($"INSERT INTO public.\"Issues\"(\"ThreadID\", \"MessageID\")VALUES ({threadId}, {messageId});");
    }

    public static bool GetFixedIssue(out ulong[] threadIds)
    {
        return GetObjects("SELECT \"ThreadID\" FROM public.\"Issues\" WHERE \"Fixed\";", out threadIds);
    }

    public static bool GetNotFixedIssue(out ulong[] threadIds)
    {
        return GetObjects("SELECT \"ThreadID\" FROM public.\"Issues\" WHERE NOT \"Fixed\";", out threadIds);
    }

    public static bool GetIssueData(ulong id, out ulong[] data)
    {
        return GetObjects($"SELECT \"MessageID\" FROM public.\"Issues\" WHERE \"ThreadID\" = {id};", out data);
    }

    public static void DeleteIssueData(ulong id)
    {
        GetObjects<ulong>($"DELETE FROM public.\"Issues\" WHERE \"ThreadID\" = {id};", out _);
    }

    private static string GetValue(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return "null";
        }
        else
        {
            return $"'{str}'";
        }
    }

    public static async Task UpdateRotationDevChannel(ulong id, ulong channelId)
    {
        using var connect = new PostgreContext();

        await connect.RotationDev
            .Upsert(new DeveloperItem()
            {
                DiscordID = id,
                ChannelID = channelId,
            })
            .On(i => new { i.DiscordID })
            .WhenMatched(i => new DeveloperItem()
            {
                ChannelID = channelId,
            })
            .RunAsync();

        await connect.SaveChangesAsync();
    }

    public static async Task<bool> UpdateRotationDevLodestone(ulong id, uint lodestone)
    {
        using var connect = new PostgreContext();

        var item = connect.RotationDev.Find(id);

        if (item == null) return false;

        item.LodestoneID = lodestone;
        connect.Update(item);

        await connect.SaveChangesAsync();

        return true;
    }

    public static bool GetChannelId(ulong id, out ulong data)
    {
        using var connect = new PostgreContext();

        var item = connect.RotationDev.Find(id);

        data = item?.ChannelID ?? 0;
        return item?.ChannelID != null;
    }

    public static bool GetLodestoneId(ulong id, out uint data)
    {
        using var connect = new PostgreContext();

        var item = connect.RotationDev.Find(id);

        data = item?.LodestoneID ?? 0;
        return item?.LodestoneID != null;
    }

    public static async Task IsvalidSupporter(ulong id, bool valid)
    {
        using var connect = new PostgreContext();
        var item = connect.Supporter.Find(id);
        if (item == null) return;

        item.IsValid = valid;
        connect.Update(item);
        await connect.SaveChangesAsync();
    }

    public static bool GetHash(ulong id, out string[] data)
    {
        using var connect = new PostgreContext();
        var item = connect.Supporter.Find(id);
        if (item == null)
        {
            data = [];
            return false;
        }

        data = item.Hashes;
        return true;
    }

    public static bool GetName(ulong id, [MaybeNullWhen(false)] out string name)
    {
        using var connect = new PostgreContext();
        var item = connect.Supporter.Find(id);
        name = item?.Name;
        return name != null;
    }

    public static async Task InitName(DiscordMember member)
    {
        var id = member.Id;
        var name = member.DisplayName;
        if (!GetName(id, out _))
        {
            UpdateSupporterData(id, string.Empty, name);
            await SupporterCommands.UpdateNames();
        }
    }

    public static string[] GetNames()
    {
        using var connect = new PostgreContext();
        return [.. connect.Supporter.Select(x => x.Name).Where(s => !string.IsNullOrEmpty(s))];
    }

    public static ulong[] GetDiscordIds()
    {
        using var connect = new PostgreContext();
        return [.. connect.Supporter.Select(x => x.DiscordID)];
    }

    public static string[] GetHashes()
    {
        using var connect = new PostgreContext();

        return [.. connect.Supporter.Where(i => i.IsValid).ToArray().SelectMany(i => i.Hashes).Where(s => !string.IsNullOrEmpty(s))];
    }

    private static void SetValues(string cmd)
    {
        using var conenction = CreateConnection();

        conenction.Open();
        using var command = new NpgsqlCommand(cmd, conenction);

        command.ExecuteNonQuery();
    }

    [Obsolete]
    private static bool GetObjects<T>(string cmd, out T[] array, Func<NpgsqlDataReader, T>? convert = null)
    {
        convert ??= r => (T)Convert.ChangeType(r[0], typeof(T));

        using var conenction = CreateConnection();
        conenction.Open();

        using var command = new NpgsqlCommand(cmd, conenction);
        using var reader = command.ExecuteReader();

        var find = false;
        List<T> result = [];
        try
        {
            while (reader.Read())
            {
                find = true;
                try
                {
                    result.Add(convert(reader));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "\n" + ex.StackTrace ?? string.Empty);
                }
            }
        }
        catch
        {

        }

        array = [.. result];
        return find;
    }
}
