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

    internal static async Task<int> OnGithubChanged(string github, string newGithub)
    {
        if (github == newGithub) return 0;

        if (!string.IsNullOrEmpty(github))
        {
            await GitHubClient.Repository.Collaborator.Delete(UserName, "RotationSolver.Basic", github);
        }

        if (!string.IsNullOrEmpty(newGithub))
        {
            try
            {
                var invite = await GitHubClient.Repository.Collaborator.Add(UserName, "RotationSolver.Basic", newGithub, new CollaboratorRequest("pull"));
                return invite.Invitee.Id;
            }
            catch
            {

            }
        }
        return 0;
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
        foreach (var grp in data.GroupBy(i => i.Repo))
        {
            var id = grp.Key;
            var project = await GitHubClient.Repository.Get(id);

            List<Author> authorList = [];
            List<string> labels = [];
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

                var message = commit.Commit.Message.Split("\n");
                var label = $"- {message.FirstOrDefault()} [Link]({commit.HtmlUrl})";

                if (message.Length > 1)
                {
                    var lt = message.ToList();
                    lt.RemoveAt(0);
                    label += "\n" + string.Join("\n", lt);
                }

                labels.Add(label);

                addition += commit.Files.Sum(f => f.Additions);
                deletions += commit.Files.Sum(f => f.Deletions);
            }

            labels.Sort();

            string body = string.Join("\n", labels);

            list.Add(new DiscordEmbedBuilder()
                .WithTitle(project.Name)
                .WithUrl(project.HtmlUrl)
                .WithDescription(body)
                .AddField("Additions", $"**+{addition}**", true)
                .AddField("Deletions", $"**-{deletions}**", true)
                .AddField("Contributers", string.Join(", ", authorList.Select(a => $"[{a.Login}]({a.HtmlUrl})"))));
        }
        return [.. list];
    }
}
