using RotationSolver.DiscordBot;

await Service.Init();
using var githubListener = new Listener(1024, Service.SendGithubPublish);
await Task.Delay(-1);
