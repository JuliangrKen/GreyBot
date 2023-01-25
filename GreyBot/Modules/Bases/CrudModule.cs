using Discord;
using GreyBot.Data;
using GreyBot.Data.Models;
using GreyBot.Data.Repos;
using System.Security.Cryptography;

namespace GreyBot.Modules.Bases
{
    public abstract class CrudModule<T> : ModuleBase where T : class
    {
        protected GreyBotContext dbContext;
        protected IRepository<T> repository;

        protected const string PageSignature = "Стр. ";

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


        protected void InstallPageInFooter(EmbedBuilder embed, int pageNumber)
            => embed.WithFooter(PageSignature + pageNumber);

        protected int GetPageNumFromFooter(Embed embed)
        {
            var text = embed.Footer?.Text ?? string.Empty;

            if (string.IsNullOrEmpty(text)) return 1; // its first page

            return Convert.ToInt32(text[PageSignature.Length..]);
        }

        protected ComponentBuilder GetBottonsForNavigation(int startIndex, int countItems, int itemViewNumber, string previousButtonId, string nextButtonId)
        {
            var componentBuilder = new ComponentBuilder()
                .WithButton($"Предыдущие {itemViewNumber}", previousButtonId, disabled: startIndex < itemViewNumber - 1)
                .WithButton($"Следующие {itemViewNumber}", nextButtonId, disabled: startIndex + itemViewNumber >= countItems);

            return componentBuilder;
        }
    }
}
