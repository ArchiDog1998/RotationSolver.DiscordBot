using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octokit;

namespace RotationSolver.DiscordBot;
internal static class GithubHelper
{
    private const string UserName = "ArchiDog1998";

    internal static GitHubClient GitHubClient = new(new ProductHeaderValue("ophion"))
    {
        Credentials = new(Config.GitHubToken) // For the token.
    };

    internal static async Task<IReadOnlyList<RepositoryContributor>> GetContributors()
    {
        return await GitHubClient.Repository.GetAllContributors(UserName, "RotationSolver.Basic");
    }

    internal delegate bool ModifyValueDelegate<T>(ref T value, out string commit);

    internal static async Task ModifyFile<T>(string repoName, string path, ModifyValueDelegate<T?> modifyFile)
    {
        bool shouldCreate = true;
        string sha = string.Empty;
        T? value = default;
        try
        {
            IReadOnlyList<RepositoryContent> content;

            content = await GitHubClient.Repository.Content.GetAllContents(UserName, repoName, path);

            shouldCreate = content.Count == 0;

            if (!shouldCreate)
            {
                sha = content[0].Sha;
                try
                {
                    value = JsonConvert.DeserializeObject<T>(content[0].Content);
                }
                catch
                {
                    value = default;
                }
            }
        }
        catch (NotFoundException)
        {
        }
        catch
        {
        }

        if (!modifyFile(ref value, out var commit))
        {
            return;
        }

        try
        {
            if (shouldCreate)
            {
                await GitHubClient.Repository.Content.CreateFile(UserName, repoName, path, new(commit, JsonConvert.SerializeObject(value, Formatting.Indented)));
            }
            else
            {
                await GitHubClient.Repository.Content.UpdateFile(UserName, repoName, path, new(commit, JsonConvert.SerializeObject(value, Formatting.Indented), sha));
            }
        }
        catch
        {
        }
    }

    internal static string ModifySupporterName = "Modified the supporter's name.",
        ModifySupporterHash = "Modified the supporter's hash.";

    internal static void SendGithubPush(string s)
    {
        var obj = JObject.Parse(s);
        var token = obj["repository"];
        if (token == null) return;

        var id = long.Parse(token["id"]!.ToString());
        foreach (var commit in (JArray)obj["commits"]!)
        {
            var sha = commit["id"]!.ToString();
            SqlHelper.InsertGithubCommit(sha, id);
        }
    }

    internal static async Task<DiscordEmbed[]> GetCommitMessage()
    {
        var list = new List<DiscordEmbed>();
        SqlHelper.GetAndClearCommits(out var data);

        if (data == null || data.Length == 0) return [];

        foreach (var grp in data.GroupBy(i => i.Repo))
        {
            var id = grp.Key;
            var project = await GitHubClient.Repository.Get(id);

            List<Author> authorList = [];
            SortedDictionary<string, List<string>> labels = [];
            int addition = 0, deletions = 0;

            foreach (var sha in grp.Select(i => i.Sha))
            {
                var commit = await GitHubClient.Repository.Commit.Get(id, sha);

                if (commit.Commit.Message == ModifySupporterName
                    || commit.Commit.Message == ModifySupporterHash) continue;

                if(!authorList.Any(a => a.Id == commit.Author.Id))
                {
                    authorList.Add(commit.Author);
                }

                var message = commit.Commit.Message.Split("\n").FirstOrDefault();

                if (message == null) continue;
                if (message.StartsWith("New translations")) continue;
                if (message.StartsWith("Merge branch")) continue;

                var items = message.Split(':');
                message = items.LastOrDefault() ?? message;

                var key = items.Length == 2 ? RemapCommitType(items[0]) : "Others";

                var label = $"- {message} [Link]({commit.HtmlUrl})";

                if (!labels.TryGetValue(key, out var lt)) labels[key] = lt = [];
                lt.Add(label);

                addition += commit.Files.Sum(f => f.Additions);
                deletions += commit.Files.Sum(f => f.Deletions);
            }

            string body = string.Empty;

            foreach (var pair in labels)
            {
                body += $"### **{pair.Key}**\n" + string.Join("\n", pair.Value) + "\n";
            }

            list.Add(new DiscordEmbedBuilder()
                .WithTitle(project.Name)
                .WithUrl(project.HtmlUrl)
                .WithDescription(body)
                .WithAuthor(project.Owner.Login, project.Owner.HtmlUrl, project.Owner.AvatarUrl)
                .AddField("Additions", $"**+{addition}**", true)
                .AddField("Deletions", $"**-{deletions}**", true)
                .AddField("Contributers", string.Join(", ", authorList.Select(a => $"[{a.Login}]({a.HtmlUrl})"))));
        }
        return [.. list];
    }

    private static string RemapCommitType(string key)
    {
        return key.ToLower() switch
        {
            "feat" or "feat!" => "Features",
            "fix" or "fix!" => "Bug Fixes",
            "refactor" or "refactor!" => "Refactors",
            "docs" => "Documents",
            _ => key,
        };
    }
}
