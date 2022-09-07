using Imdb.Infrastructure;
using Infrastructure.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.IRepository
{
    public interface IRepository<T>
    {
        Task<T?> GetSingleOrDefaultAsync(int id);
        IQueryable<T> Query();
        void Add(T entity);
        void Remove(T entity);
        
        }

    public class Repository<T> : IRepository<T>
        where T : class, IPrimaryKey
    {
        protected DataContext _context;

        public Repository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task<T?> GetSingleOrDefaultAsync(int id)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(x => x.Id == id);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
     
        }

        public virtual IQueryable<T> Query()
        {
            return _context.Set<T>().AsQueryable();
        }
        public void Remove (T entity)
        {
            _context.Remove(entity);
        }
    }
}
