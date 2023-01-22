using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GreyBot.Extensions;
using GreyBot.Modules.Bases;

namespace GreyBot.Modules
{
    [Group("talk", "Говорение он лица бота")]
    public class TalkModule : ModuleBase
    {
        private const string TalkEmbedId = "talk_embed";
        private const string TalkEmbedTitleId = "talk_embed_title";
        private const string TalkEmbedTextId = "talk_embed_text";

        private static TalkModule? Singleton;

        public TalkModule(DiscordSocketClient socketClient)
        {
            if (Singleton != null) return;
            Singleton = this;

            socketClient.ModalSubmitted += HandleBotWriteEmbed;
        }

        [SlashCommand("embed", "Написать embed")]
        public async Task BotWriteEmbed()
        {
            var modalBuilder = new ModalBuilder()
                .WithTitle("Embed")
                .WithCustomId(TalkEmbedId)
                .AddTextInput("Заголовок", TalkEmbedTitleId, TextInputStyle.Short, "Чё-то важное", null, 30)
                .AddTextInput("Основной текст", TalkEmbedTextId, TextInputStyle.Paragraph, "Я вам расскажу, откуда готовилось нападение на Беларусь");

            await Context.Interaction.RespondWithModalAsync(modalBuilder.Build());
        }

        private async Task HandleBotWriteEmbed(SocketModal modal)
        {
            var components = modal.Data.Components;

            var embedBuilder = new EmbedBuilder()
                .WithAuthor(modal.User)
                .WithColor(new Random().NextColor())
                .WithTitle(components.FirstOrDefault((c) => c.CustomId == TalkEmbedTitleId)?.Value)
                .WithDescription(components.FirstOrDefault((c) => c.CustomId == TalkEmbedTextId)?.Value);

            await modal.RespondAsync(embed: embedBuilder.Build());
        }
    }
}
