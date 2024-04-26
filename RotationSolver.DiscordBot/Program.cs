using RotationSolver.DiscordBot;


//var commit = await GithubHelper.GitHubClient.Repository.Commit.Get("ArchiDog1998", "RotationSolver",);

//GitHubCommit commit;

//commit.Files.Select(f => f.Additions)

await Service.Init();
using var githubListener = new Listener(1024, Service.SendGithubPublish);
await Task.Delay(-1);
