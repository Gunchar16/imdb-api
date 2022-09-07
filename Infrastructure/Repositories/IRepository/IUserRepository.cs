using Imdb.Infrastructure.Entities;
using Infrastructure.Repositories.IRepository;

namespace Imdb.Infrastructure.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        //DataContext Context { get; }

        Task<User?> GetByUserNameAsync(string userName);

        IQueryable<User> QueryRole(int usertier);



    }
}
