using GreyBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GreyBot.Data
{
    public class GreyBotContext : DbContext
    {
        public DbSet<GuildUser> Users { get; set; }

        public DbSet<Compliment> Compliments { get; set; }
        public DbSet<Insult> Insults { get; set; }

        public DbSet<ComplimentLog> ComplimentLogs { get; set; }
        public DbSet<InsultLog> InsultLogs { get; set; }

        public GreyBotContext(DbContextOptions<GreyBotContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            optionsBuilder.UseSqlite("Filename=GreyBotDev.db");
#else
            optionsBuilder.UseSqlite("Filename=GreyBot.db");
#endif
        }
    }
}
