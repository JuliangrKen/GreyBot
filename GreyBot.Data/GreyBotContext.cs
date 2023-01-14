using GreyBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GreyBot.Data
{
    public class GreyBotContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Compliment> Compliments { get; set; }
        public DbSet<Insult> Insults { get; set; }

        public DbSet<ComplimentLog> ComplimentLogs { get; set; }
        public DbSet<InsultLog> InsultLogs { get; set; }

        public GreyBotContext(DbContextOptions<GreyBotContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
