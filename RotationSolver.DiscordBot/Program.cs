using RotationSolver.DiscordBot;
using RotationSolver.DiscordBot.Npgsql;

//using var connect = new PostgreContext();

using var listener = new Listener(1024);
await Service.Init();
await Task.Delay(-1);