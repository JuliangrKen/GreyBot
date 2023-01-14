using Microsoft.EntityFrameworkCore;

namespace GreyBot.Data.Repos
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public DbSet<T> Set { get; private set; }

        protected DbContext db;

        public Repository(GreyBotContext db)
        {
            this.db = db;

            var set = db.Set<T>();
            set.Load();

            Set = set;
        }

        public IEnumerable<T> GetAll()
            => Set;

        public Task Create(T item)
        {
            Set.Add(item);
            db.SaveChanges();

            return Task.CompletedTask;
        }

        public Task Delete(T item)
        {
            Set.Remove(item);
            db.SaveChanges();

            return Task.CompletedTask;
        }

        public Task Update(T item)
        {
            Set.Update(item);
            db.SaveChanges();

            return Task.CompletedTask;
        }
    }
}
