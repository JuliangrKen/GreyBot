using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using GreyBot.Data;
using GreyBot.Data.Models;
using GreyBot.Data.Repos;
using GreyBot.Extensions;
using GreyBot.Modules.Bases;
using GreyBot.Utils;
using System.Text;

namespace GreyBot.Modules
{
    [Group("compliment", "Команды связанные с комплиментами")]
    public class ComplimentModule : CrudModule<Compliment>
    {
        private const string ComplimentPreviousButtonId = "compliment_previous_button";
        private const string ComplimentNextButtonId = "compliment_next_button";

        private const int complimentViewMaxLength = 100;
        private const int complimentsViewNumber = 10;

        private static ComplimentModule? Singleton;

        protected ComplimentModule(DiscordSocketClient socketClient, GreyBotContext dbContext) : base(dbContext)
        {
            if (Singleton != null) return;
            Singleton = this;

            socketClient.ButtonExecuted += HandleGetAllCompliments;
        }

        [SlashCommand("user", "Сделать комплимент пользователю")]
        public async Task OffendUser([Summary("пользователь", "тот, кому сделать комплимент")] IUser user)
        {
            try
            {
                var randomCompliment = GetRandomModel(repository.GetAll().Where(i => i.GuildId == Context.Guild.Id));

                await AddAbuseLog(user.Id, Context.User.Id, Context.Guild.Id);
                await RespondAsync($"{user.Mention}, {randomCompliment.Text}");
            }
            catch
            {
                await WriteErrorMessage();
            }
        }

        [SlashCommand("add", "Добавить комплимент в базу данных")]
        public async Task AddOffend([Summary("комплимент", "текст комплимента")] string text)
        {
            try
            {
                if (!await UserHasWhiteList(Context.User.Id, Context.Guild.Id))
                {
                    await RespondAsync("У вас нет прав на добавление оскорблений!", ephemeral: true);
                    return;
                }

                await repository.Create(new Compliment()
                {
                    GuildId = Context.Guild.Id,
                    Text = text,
                });

                await RespondAsync($"комплимент было добавлено в базу данных!\n`{text}`", ephemeral: true);
            }
            catch
            {
                await WriteErrorMessage();
            }
        }

        [SlashCommand("get-all", "Получить список всех доступных оскорблений")]
        public async Task GetAllCompliments()
        {
            var compliments = repository.GetAll().Where((i) => i.GuildId == Context.Guild.Id);

            var embedBuilder = new EmbedBuilder()
                .WithColor(new Random().NextColor())
                .WithTitle("Список существующих комплиментов")
                .WithDescription(BuildComplimentsString(compliments, 0))
                .WithFooter("стр. 1");

            await RespondAsync(embed: embedBuilder.Build(),
                components: GetBottonsForNavigation(0,
                                                    compliments.Count(),
                                                    complimentsViewNumber,
                                                    ComplimentPreviousButtonId,
                                                    ComplimentNextButtonId).Build(),
                ephemeral: true);
        }

        [SlashCommand("delete", "Удалить комплимент по ID")]
        public async Task DeleteCompliment([Summary("id", "id комплимента")] int id)
        {
            if (!await UserHasWhiteList(Context.User.Id, Context.Guild.Id))
            {
                await RespondAsync("У вас нет прав на удаление комплиментов!", ephemeral: true);
                return;
            }

            try
            {
                await repository.Delete(repository.GetAll().FirstOrDefault((i) => i.Id == id && i.GuildId == Context.Guild.Id) ??
                    throw new NullReferenceException());

                await RespondAsync("Удаление произошло успешно!", ephemeral: true);
            }
            catch
            {
                await WriteErrorMessage();
            }
        }

        private async Task HandleGetAllCompliments(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case ComplimentNextButtonId:
                    await WriteNextDataCompliment(GetPageNumFromFooter(component.Message.Embeds.First()) + 1, component);
                    return;
                case ComplimentPreviousButtonId:
                    await WriteNextDataCompliment(GetPageNumFromFooter(component.Message.Embeds.First()) - 1, component);
                    return;
            };
        }

        private async Task WriteNextDataCompliment(int newPageNumber, SocketMessageComponent component)
        {
            var compliments = repository.GetAll().Where((i) => i.GuildId == component.GuildId);
            var startIndex = (newPageNumber - 1) * complimentsViewNumber;

            await component.UpdateAsync((messageProperties) =>
            {
                var embedBuilder = component.Message.Embeds.First().ToEmbedBuilder()
                    .WithDescription(BuildComplimentsString(compliments, startIndex));

                InstallPageInFooter(embedBuilder, newPageNumber);

                messageProperties.Embed = embedBuilder.Build();
                messageProperties.Components = GetBottonsForNavigation(startIndex,
                                                                       compliments.Count(),
                                                                       complimentsViewNumber,
                                                                       ComplimentPreviousButtonId,
                                                                       ComplimentNextButtonId).Build();
            });
        }

        private static string BuildComplimentsString(IEnumerable<Compliment> compliments, int startIndex)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("```ID:\tText:\n");

            var dataStringViewer = new DataStringViewer<Compliment>(compliments);

            return dataStringViewer.GetView(stringBuilder, startIndex, startIndex + complimentsViewNumber, (compliment) =>
            {
                var text = compliment.Text?.Length > complimentViewMaxLength ? compliment.Text?[0..complimentViewMaxLength] : compliment.Text;
                return $"{compliment.Id}\t  {text}\n";
            }, () =>
            {
                stringBuilder.Append("```");
            });
        }

        private async Task AddAbuseLog(ulong recipientDiscordId, ulong senderDiscordId, ulong guildId)
        {
            var repository = new Repository<ComplimentLog>(dbContext);

            await repository.Create(new ComplimentLog()
            {
                RecipientDiscordId = recipientDiscordId,
                SenderDiscordId = senderDiscordId,
                GuildId = guildId,
            });
        }
    }
}
