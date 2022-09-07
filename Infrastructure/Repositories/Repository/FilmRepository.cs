using Imdb.Infrastructure;
using Imdb.Infrastructure.Entities;
using Imdb.Infrastructure.IRepository;
using Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Repository
{
    public class FilmRepository : Repository<Film>,IFilmRepository
    {

        public FilmRepository(DataContext context)
            : base(context)
        {
        }
        public async Task<Film?> GetFilmByName(string Name)
        {
            return await _context.Films.Where(x => x.Name == Name).SingleOrDefaultAsync();
        }
    }
}
