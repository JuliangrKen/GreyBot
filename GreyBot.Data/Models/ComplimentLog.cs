using System.ComponentModel.DataAnnotations;

namespace GreyBot.Data.Models
{
    public class ComplimentLog
    {
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }
        public User? Sender { get; set; }

        [Required]
        public int RecipientId { get; set; }
        public User? Recipient { get; set; }
    }
}
