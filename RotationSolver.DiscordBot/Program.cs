using RotationSolver.DiscordBot;


using var githubPushListener = new Listener(1026, GithubHelper.SendGithubPush);
using var githubReleaseListener = new Listener(1024, Service.SendGithubPublish);
using var kofiListener = new Listener(1025, Service.SendKofi);

await Service.Init();
await Task.Delay(-1);
