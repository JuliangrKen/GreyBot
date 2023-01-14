using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GreyBot
{
    public class DiscordBot
    {
        private readonly BotConfig config;
        private readonly DiscordSocketClient socketClient;
        private readonly InteractionService interactionService;
        private readonly IServiceProvider serviceProvider;

        public DiscordBot
        (
            BotConfig config,
            DiscordSocketClient socketClient,
            InteractionService interactionService,
            IServiceProvider serviceProvider
        )
        {
            this.config = config;
            this.socketClient = socketClient;
            this.interactionService = interactionService;
            this.serviceProvider = serviceProvider;
        }

        public async Task Run()
        {
            socketClient.Log += Log;
            socketClient.Ready += HandleSlashCommandAsync;

            await socketClient.LoginAsync(TokenType.Bot, config.BotToken);
            await socketClient.StartAsync();
            await Task.Delay(-1);
        }

        private async Task HandleSlashCommandAsync()
        {
            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);

#if DEBUG
            await interactionService.RegisterCommandsToGuildAsync(config.DevGuildId
                ?? throw new NullReferenceException());
#else
            await interactionService.RegisterCommandsGloballyAsync();
#endif

            socketClient.InteractionCreated += async interaction =>
            {
                var scope = serviceProvider.CreateScope();
                var ctx = new SocketInteractionContext(socketClient, interaction);
                await interactionService.ExecuteCommandAsync(ctx, scope.ServiceProvider);
            };
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }
    }
}
