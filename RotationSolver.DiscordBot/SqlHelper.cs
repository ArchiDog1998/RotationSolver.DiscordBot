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

    public static async Task UpdateSupporterData(ulong id, string? hash, string? name)
    {
        using var connect = new PostgreContext();

        var item = connect.Supporter.Find(id);

        if (item == null)
        {
            await connect.Supporter.AddAsync(new SupporterItem
            {
                DiscordID = id,
                Hashes = hash == null ? [] : [hash],
                Name = name,
            });
        }
        else
        {
            if (hash != null)
            {
                if (item.Hashes.Contains(hash))
                {
                    var list = item.Hashes.Where(s => !string.IsNullOrEmpty(s)).ToList();
                    list.Remove(hash);
                    list.Insert(0, hash);
                    item.Hashes = [..list];
                }
                else
                {
                    item.Hashes = item.Hashes.Append(hash).Where(s => !string.IsNullOrEmpty(s)).TakeLast(8).ToArray();
                }
                
            }
            item.Name = name ?? item.Name;
            connect.Update(item);
        }

        await connect.SaveChangesAsync();
    }

    public static async Task FixedIssueData(ulong threadId)
    {
        using var connect = new PostgreContext();
        var item = connect.Issues.Find(threadId);
        if (item == null) return;
        item.Fixed = true;
        connect.Update(item);
        await connect.SaveChangesAsync();
    }

    public static async Task InsertIssueData(ulong threadId, ulong messageId)
    {
        using var connect = new PostgreContext();
        await connect.Issues.AddAsync(new() { ThreadID = threadId, MessageID = messageId });
        await connect.SaveChangesAsync();
    }

    public static ulong[] GetFixedIssue()
    {
        using var connect = new PostgreContext();
        return [.. connect.Issues.Where(i => i.Fixed).Select(i => i.ThreadID)];
    }

    public static ulong[] GetNotFixedIssue()
    {
        using var connect = new PostgreContext();
        return [..connect.Issues.Where(i => !i.Fixed).Select(i => i.ThreadID)];
    }

    public static bool GetIssueData(ulong id, out ulong data)
    {
        using var connect = new PostgreContext();
        var item = connect.Issues.Find(id);
        data = item?.MessageID ?? 0;
        return item != null;
    }

    public static async Task DeleteIssueData(ulong id)
    {
        using var connect = new PostgreContext();
        var item = connect.Issues.Find(id);
        if (item == null) return;
        connect.Issues.Remove(item);
        await connect.SaveChangesAsync();
    }

    public static async Task UpdateRotationDevChannel(ulong id, ulong channelId)
    {
        using var connect = new PostgreContext();
        var item = connect.RotationDev.Find(id);

        if (item == null)
        {
            await connect.RotationDev.AddAsync(new DeveloperItem
            {
                DiscordID = id,
                ChannelID = channelId,
            });
        }
        else
        {
            item.ChannelID = channelId;
            connect.Update(item);
        }
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
            await UpdateSupporterData(id, string.Empty, name);
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
}
