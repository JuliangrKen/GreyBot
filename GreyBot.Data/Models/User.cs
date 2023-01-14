using System.ComponentModel.DataAnnotations;

namespace GreyBot.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public ulong DiscordId { get; set; }
        public bool HasWhiteList { get; set; }
    }
}
