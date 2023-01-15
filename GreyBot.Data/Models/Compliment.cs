using System.ComponentModel.DataAnnotations;

namespace GreyBot.Data.Models
{
    public class Compliment
    {
        public int Id { get; set; }

        [Required]
        public ulong GuildId { get; set; }

        [StringLength(500, MinimumLength = 5)]
        public string? Text { get; set; }
    }
}
