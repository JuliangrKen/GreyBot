using Discord;
using Discord.Interactions;
using GreyBot.Data;
using GreyBot.Data.Models;
using GreyBot.Data.Repos;
using System.Security.Cryptography;

namespace GreyBot.Modules
{
    [Group("abuse", "Команды связанные с оскорблениями")]
    public class AbuseModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly GreyBotContext dbContext;

        public AbuseModule(GreyBotContext dbContext)
        {
            this.dbContext = dbContext;
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
                var repository = new Repository<Insult>(dbContext);

                var randomInsult = GetRandomInsult(repository.GetAll().Where(i => i.GuildId == Context.Guild.Id));

                await AddAbuseLog(user.Id, Context.User.Id, Context.Guild.Id);
                await RespondAsync($"{user.Mention}, {randomInsult.Text}");
            }
            catch
            {
                await RespondAsync("Что-то пошло не так!", ephemeral: true);
            }
        }

        [SlashCommand("add", "Добавить оскорбление в базу данных")]
        public async Task AddOffend([Summary("оскорбление", "текст оскорбления")] string text)
        {
            try
            {
                if (!UserHasWhiteList(Context.User.Id, Context.Guild.Id))
                {
                    await RespondAsync("У вас нет прав на добавление оскорблений!", ephemeral: true);
                    return;
                }

                var repository = new Repository<Insult>(dbContext);

                await repository.Create(new Insult()
                {
                    GuildId = Context.Guild.Id,
                    Text = text,
                });

                await RespondAsync($"Оскорбление было добавлено в базу данных!\n`{text}`", ephemeral: true);
            }
            catch
            {
                await RespondAsync("Что-то пошло не так!", ephemeral: true);
            }
        }

        private bool UserHasWhiteList(ulong discordId, ulong guildId)
        {
            var repository = new Repository<GuildUser>(dbContext);

            var user = repository.GetAll().FirstOrDefault(i => i.DiscordId == discordId && i.GuildId == guildId);

            if (user is null)
            {
                repository.Create(new GuildUser() { DiscordId = discordId, GuildId = guildId });
                return false;
            }

            return user.HasWhiteList;
        }

        private Insult GetRandomInsult(IEnumerable<Insult> insults)
            => insults.FirstOrDefault(x => x.Id == RandomNumberGenerator.GetInt32(1, insults.Count()))
                ?? throw new NullReferenceException();

        private async Task AddAbuseLog(ulong recipientId, ulong senderId, ulong guildId)
        {
            var repository = new Repository<InsultLog>(dbContext);

            await repository.Create(new InsultLog()
            {
                RecipientId = recipientId,
                SenderId = senderId,
                GuildId = guildId,
            });
        }
    }
}
