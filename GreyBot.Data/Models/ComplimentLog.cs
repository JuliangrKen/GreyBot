using System.ComponentModel.DataAnnotations;

namespace GreyBot.Data.Models
{
    public class ComplimentLog
    {
        public int Id { get; set; }

        [Required]
        public ulong GuildId { get; set; }

        [Required]
        public ulong SenderDiscordId { get; set; }

        [Required]
        public ulong RecipientDiscordId { get; set; }
    }
}
