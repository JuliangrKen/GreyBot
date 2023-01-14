namespace GreyBot.Data.Repos
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task Create(T item);
        Task Update(T item);
        Task Delete(T item);
    }
}
