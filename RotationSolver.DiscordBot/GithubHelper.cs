using Newtonsoft.Json;
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
}
