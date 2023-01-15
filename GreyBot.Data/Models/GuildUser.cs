using System.ComponentModel.DataAnnotations;

namespace GreyBot.Data.Models
{
    public class GuildUser
    {
        public int Id { get; set; }

        [Required]
        public ulong DiscordId { get; set; }

        [Required]
        public ulong GuildId { get; set; }

        public bool HasWhiteList { get; set; }
    }
}
