using GreyBot;
using GreyBot.Data;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddDbContextFactory<GreyBotContext>();

#if DEBUG
var configPath = $"{Environment.CurrentDirectory}\\BotConfig.Development.json";
#else
var configPath = $"{Environment.CurrentDirectory}\\BotConfig.json";
#endif

services.AddSingleton(BotConfig.GetFromFile(configPath));

var build = services.BuildServiceProvider();

build.GetService<GreyBotContext>();

var bot = build.GetService<DiscordBot>()
    ?? throw new NullReferenceException();

await bot.Run();

Console.WriteLine("Bye-bye");