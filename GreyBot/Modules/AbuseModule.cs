using Discord;
using Discord.Interactions;
using GreyBot.Data;
using GreyBot.Data.Models;
using GreyBot.Data.Repos;
using GreyBot.Extensions;
using GreyBot.Modules.Bases;
using GreyBot.Utils;
using System.Text;

namespace GreyBot.Modules
{
    [Group("abuse", "Команды связанные с оскорблениями")]
    public class AbuseModule : CrudModule<Insult>
    {
        private const string AbuseCloseButtonId = "abuse_close_button";
        private const string AbuseNextButtonId = "abuse_next_button";
        private const string AbusePreviousButtonId = "abuse_previous_button";

        private const int insultViewMaxLength = 100;
        private const int insultsViewNumber = 10;

        protected AbuseModule(GreyBotContext dbContext) : base(dbContext)
        {
        }

        [SlashCommand("user", "Оскорбить пользователя")]
        public async Task OffendUser([Summary("пользователь", "тот, кого оскорбить")] IUser user)
        {
            if (user.IsBot)
            {
                await RespondAsync("Братков не обижаю.", ephemeral: true);
                return;
            }

            try
            {
                var randomInsult = GetRandomModel(repository.GetAll().Where(i => i.GuildId == Context.Guild.Id));

                await AddAbuseLog(user.Id, Context.User.Id, Context.Guild.Id);
                await RespondAsync($"{user.Mention}, {randomInsult.Text}");
            }
            catch
            {
                await WriteErrorMessage();
            }
        }

        [SlashCommand("add", "Добавить оскорбление в базу данных")]
        public async Task AddOffend([Summary("оскорбление", "текст оскорбления")] string text)
        {
            try
            {
                if (!await UserHasWhiteList(Context.User.Id, Context.Guild.Id))
                {
                    await RespondAsync("У вас нет прав на добавление оскорблений!", ephemeral: true);
                    return;
                }

                await repository.Create(new Insult()
                {
                    GuildId = Context.Guild.Id,
                    Text = text,
                });

                await RespondAsync($"Оскорбление было добавлено в базу данных!\n`{text}`", ephemeral: true);
            }
            catch
            {
                await WriteErrorMessage();
            }
        }

        [SlashCommand("get-all", "Получить список всех доступных оскорблений")]
        public async Task GetAllInsults()
        {
            var insults = repository.GetAll().Where((i) => i.GuildId == Context.Guild.Id);

            var embedBuilder = new EmbedBuilder()
                .WithColor(new Random().NextColor())
                .WithTitle("Список существующих оскорблений")
                .WithDescription(BuildInsultsString(insults, 0));

            var componentBuilder = new ComponentBuilder()
                .WithButton(emote: new Emoji("❌"), customId: AbuseCloseButtonId, style: ButtonStyle.Secondary)
                .WithButton("Следующие 11-20", AbuseNextButtonId);

            await RespondAsync(embed: embedBuilder.Build(), components: componentBuilder.Build());
        }

        [SlashCommand("delete", "Удалить оскорбление по ID")]
        public async Task DeleteInsult([Summary("id", "id оскорбления")] int id)
        {
            if (!await UserHasWhiteList(Context.User.Id, Context.Guild.Id))
            {
                await RespondAsync("У вас нет прав на удаление оскорблений!", ephemeral: true);
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

        private string BuildInsultsString(IEnumerable<Insult> insults, int startIndex)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("```ID:\tText:\n");

            var dataStringViewer = new DataStringViewer<Insult>(insults);

            return dataStringViewer.GetView(stringBuilder, startIndex, startIndex + insultsViewNumber, (insult) =>
            {
                var text = insult.Text?.Length > insultViewMaxLength ? insult.Text?[0..insultViewMaxLength] : insult.Text;
                return $"{insult.Id}\t  {text}\n";
            }, () =>
            {
                stringBuilder.Append("```");
            });
        }

        private async Task AddAbuseLog(ulong recipientDiscordId, ulong senderDiscordId, ulong guildId)
        {
            var repository = new Repository<InsultLog>(dbContext);

            await repository.Create(new InsultLog()
            {
                RecipientDiscordId = recipientDiscordId,
                SenderDiscordId = senderDiscordId,
                GuildId = guildId,
            });
        }
    }
}
