using RotationSolver.DiscordBot;

using var listener = new Listener(1024);
await Service.Init();
await Task.Delay(-1);