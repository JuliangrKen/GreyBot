using System.ComponentModel.DataAnnotations;

namespace GreyBot.Data.Models
{
    public class ComplimentLog
    {
        public int Id { get; set; }

        [Required]
        public ulong GuildId { get; set; }

        [Required]
        public ulong SenderId { get; set; }
        public GuildUser? Sender { get; set; }

        [Required]
        public ulong RecipientId { get; set; }
        public GuildUser? Recipient { get; set; }
    }
}
