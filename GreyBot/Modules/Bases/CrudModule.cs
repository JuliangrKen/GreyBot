using GreyBot.Data;
using GreyBot.Data.Models;
using GreyBot.Data.Repos;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace GreyBot.Modules.Bases
{
    public class CrudModule<T> : ModuleBase where T : class
    {
        protected GreyBotContext dbContext;
        protected IRepository<T> repository;

        protected CrudModule(GreyBotContext dbContext)
        {
            this.dbContext = dbContext;
            repository = new Repository<T>(dbContext);
        }

        protected T GetRandomModel(IEnumerable<T> data)
        {
            var dataArray = data.ToArray();

            return dataArray[RandomNumberGenerator.GetInt32(0, dataArray.Length)];
        }

        protected async Task<bool> UserHasWhiteList(ulong discordId, ulong guildId)
        {
            var repository = new Repository<GuildUser>(dbContext);

            var user = repository.GetAll().FirstOrDefault(i => i.DiscordId == discordId && i.GuildId == guildId);

            if (user is null)
            {
                await repository.Create(new GuildUser() { DiscordId = discordId, GuildId = guildId });
                return false;
            }

            return user.HasWhiteList;
        }
    }
}
