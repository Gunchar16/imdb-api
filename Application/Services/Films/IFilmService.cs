using Imdb.Infrastructure.Entities;
using Infrastructure.Filter;
using Infrastructure.Paging;

namespace Application.Services.Films
{
    public interface IFilmService
    {

        Task<Film?> FindByIdAsync(int id);
        Task AddFilm(string name, string description, int releaseYear, IFormFile image);
        Task EditFilm(int id, string name, string description, int releaseYear, IFormFile? image);
        Task DeleteFilm(int id);
        Task<Film> GetFilm(int filmId);
        Task<IEnumerable<Film?>> GetAll(PagingParameters pagingParameters, FilterSettings? sortSettings, FilterSettings? filterSettings, float? lowerLimit, float? upperLimit, bool isDescending);
        Task<IEnumerable<Film?>> SearchAll(PagingParameters pagingParameters, string searchParameter);
        Task<Genre> GetGenre(int id);
        Task<int> GetGenreIdByName(string name);
        Task AddGenre(string name);
        Task DeleteGenre(int id);
        Task EditGenre(int id, string name);
        Task AddFilm_Genre(int filmId, int genreId);
        Task DeleteFilm_Genre(int filmId, int genreId);
        Task<IEnumerable<int>> GetFilmsOfGenre(int genreId);
        Task<IEnumerable<int>> GetGenresOfFilm(int filmId);

    }
}
