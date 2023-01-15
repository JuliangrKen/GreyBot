using Discord;
using Discord.Interactions;
using Discord.WebSocket;
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

var discordSocketCfg = new DiscordSocketConfig()
{
    LogGatewayIntentWarnings = false,
    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
    AlwaysDownloadUsers = true,
};

services.AddSingleton(discordSocketCfg);
services.AddSingleton<DiscordSocketClient>();
services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
services.AddSingleton<DiscordBot>();

var build = services.BuildServiceProvider();

build.GetService<GreyBotContext>();

var bot = build.GetService<DiscordBot>()
    ?? throw new NullReferenceException();

await bot.Run();