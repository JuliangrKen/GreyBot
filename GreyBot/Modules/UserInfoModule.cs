using Discord;
using Discord.Interactions;
using GreyBot.Data;
using GreyBot.Data.Models;
using GreyBot.Data.Repos;
using GreyBot.Extensions;
using GreyBot.Modules.Bases;

namespace GreyBot.Modules
{
    [Group("userinfo", "Получить некую информацию о пользователе")]
    public class UserInfoModule : ModuleBase
    {
        private readonly IEnumerable<ComplimentLog> complimentLogs;
        private readonly IEnumerable<InsultLog> insultLogs;

        public UserInfoModule(GreyBotContext dbContext)
        {
            complimentLogs = new Repository<ComplimentLog>(dbContext).GetAll();
            insultLogs = new Repository<InsultLog>(dbContext).GetAll();
        }

        [SlashCommand("all", "Получить всю доступную информацию о пользователе")]
        public async Task GetAllUserInfo(IUser? user = null)
            => await RespondAsync(embed: GetEmbedAllUserInfo(user ?? Context.User).Build(), ephemeral: true);

        private EmbedBuilder GetEmbedAllUserInfo(IUser user)
            => new EmbedBuilder()
                .WithColor(new Random().NextColor())
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithTitle($"{user.Username} - информация")
                .AddField(new EmbedFieldBuilder()
                    .WithName("Полученные комплименты:")
                    .WithValue("`❤️\t" + complimentLogs.Where((c) => c.RecipientDiscordId == user.Id && c.GuildId == Context.Guild.Id).Count() + "\t  `"))
                .AddField(new EmbedFieldBuilder()
                    .WithName("Отправленные комплименты:")
                    .WithValue("`😘\t" + complimentLogs.Where((c) => c.SenderDiscordId == user.Id && c.GuildId == Context.Guild.Id).Count() + "\t  `"))
                .AddField(new EmbedFieldBuilder()
                    .WithName("Полученные оскорбления:")
                    .WithValue("`🥺\t" + insultLogs.Where((c) => c.RecipientDiscordId == user.Id && c.GuildId == Context.Guild.Id).Count() + "\t  `"))
                .AddField(new EmbedFieldBuilder()
                    .WithName("Отправленные оскорбления:")
                    .WithValue("`😈\t" + insultLogs.Where((c) => c.SenderDiscordId == user.Id && c.GuildId == Context.Guild.Id).Count() + "\t  `"));
    }
}
