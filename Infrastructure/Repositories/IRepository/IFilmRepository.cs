using Imdb.Infrastructure.Entities;
using Imdb.Infrastructure.IRepository;

namespace Infrastructure.Repositories.IRepository
{
    public interface IFilmRepository : IRepository<Film>
    {
        Task<Film?> GetFilmByName(string Name);
    }
}
