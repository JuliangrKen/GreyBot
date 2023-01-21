using Discord;
using Discord.Interactions;
using GreyBot.Data;
using GreyBot.Data.Models;
using GreyBot.Modules.Bases;

namespace GreyBot.Modules
{
    [Group("whitelist", "Команды для работы с вайт-листом")]
    public class WhiteListModule : CrudModule<GuildUser>
    {
        public WhiteListModule(GreyBotContext dbContext) : base(dbContext)
        {
        }

        [RequireOwner]
        [SlashCommand("add", "Добавить пользователя в вайт-лист")]
        public async Task AddGuildUserWhiteList([Summary("пользователь")] IUser user)
        {
            try
            {
                await CreateOrUpdateGuildUser(user.Id, Context.Guild.Id, true);

                await RespondAsync("Пользователь был успешно добавлен в whitelist!", ephemeral: true);
            }
            catch
            {
                await WriteErrorMessage();
            }
        }

        [RequireOwner]
        [SlashCommand("delete", "Убрать пользователя из вайт-листа")]
        public async Task DeleteGuildUserWhiteList([Summary("пользователь")] IUser user)
        {
            try
            {
                await CreateOrUpdateGuildUser(user.Id, Context.Guild.Id, false);

                await RespondAsync("Пользователь больше не в whitelist!", ephemeral: true);
            }
            catch
            {
                await RespondAsync("Что-то пошло не так!", ephemeral: true);
            }
        }

        private async Task CreateOrUpdateGuildUser(ulong discordId, ulong guildId, bool hasWhiteList)
        {
            var guildUser = repository.GetAll().FirstOrDefault(u => u.DiscordId == discordId && u.GuildId == guildId);

            if (guildUser == null)
            {
                await repository.Create(new GuildUser()
                {
                    DiscordId = discordId,
                    GuildId = guildId,
                    HasWhiteList = hasWhiteList
                });
            }
            else
            {
                guildUser.HasWhiteList = hasWhiteList;
                await repository.Update(guildUser);
            }
        }
    }
}
