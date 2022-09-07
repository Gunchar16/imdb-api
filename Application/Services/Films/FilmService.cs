using Imdb.Infrastructure.Entities;
using Infrastructure.Filter;
using Infrastructure.Paging;
using Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Exceptions;
using System.Linq.Expressions;

namespace Application.Services.Films
{
    public class FilmService : IFilmService
    {
        private readonly IFilmRepository _filmRepo;
        private readonly IRepository<Genre> _genreRepo;
        private readonly IRepository<Film_Genre> _film_genreRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FilmService> _logger;


        public FilmService(ILogger<FilmService> logger, IFilmRepository film, IRepository<Film_Genre> film_genreRepo, IRepository<Genre> genreRepo, IWebHostEnvironment webHostEnvironment, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _filmRepo = film;
            _genreRepo = genreRepo;
            _film_genreRepo = film_genreRepo;
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }
        public async Task<Film?> FindByIdAsync(int id)
        {
            return await _filmRepo.GetSingleOrDefaultAsync(id);
        }

        public async Task<Film> GetFilm(int filmId)
        {
            var film = await _filmRepo.Query().Where(x => x.Id == filmId).SingleOrDefaultAsync();
            if (film is null)
                throw new NotFoundException("Film with that ID does not exist");

            return film;
        }
        public async Task AddFilm(string name, string description, int releaseYear, IFormFile image)
        {
            if (await _filmRepo.GetFilmByName(name) is not null)
                throw new AppException("That film already exists");
            string path = _webHostEnvironment.WebRootPath + @"/FilmPhotos/";
            if (image.Length > 0)
            {
                using (FileStream fileStream = File.Create(path + image.FileName))
                {
                    await image.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
            else
                throw new AppException("Problems with uploading a photo");
            var newFilm = new Film()
            {
                Name = name,
                Description = description,
                Rating = 0,
                ReleaseYear = releaseYear,
                ImageURL = $"FilmPhotos/{image.FileName}"
        };
            _filmRepo.Add(newFilm);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task EditFilm(int id, string name, string description, int releaseYear, IFormFile? image)
        {
            var filmToEdit = await _filmRepo.GetSingleOrDefaultAsync(id);
            if (filmToEdit is null)
                throw new AppException("Cannot edit film, it does not exist");
            string path = _webHostEnvironment.WebRootPath + @"/FilmPhotos/";
            File.Delete(_webHostEnvironment.WebRootPath + "/" + filmToEdit.ImageURL);
            if (image.Length > 0)
            {
                using (FileStream fileStream = File.Create(path + image.FileName))
                {
                    await image.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
            else
                throw new AppException("Problems with uploading a photo");
            filmToEdit.Name = name;
            filmToEdit.Description = description;
            filmToEdit.ReleaseYear = releaseYear;
            filmToEdit.ImageURL = $"FilmPhotos/{image.FileName}";
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteFilm(int id)
        {
            var Film = await _filmRepo.GetSingleOrDefaultAsync(id);
            if (Film is null)
                throw new NotFoundException("Cannot delete film. Film does not exist");
            _filmRepo.Remove(Film);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<IEnumerable<Film?>> GetAll(PagingParameters pagingParameters, FilterSettings? sortSettings, FilterSettings? filterSettings, float? lowerLimit, float? upperLimit, bool isDescending)
        {
            var query = _filmRepo.Query();
            if (query is null)
                throw new NotFoundException("No Films Found");
            switch(filterSettings)
            {
                case FilterSettings.Rating:
                    query = from item in query
                            where item.Rating >= lowerLimit && item.Rating <= upperLimit
                            select item;
                    break;
                case FilterSettings.Year:
                    query = from item in query
                            where item.ReleaseYear >= lowerLimit && item.ReleaseYear <= upperLimit
                            select item;
                    break;
                default:
                    break;
            }
            switch(sortSettings)
            {
                case FilterSettings.Rating:
                    query = isDescending ? query.OrderByDescending(x => x.Rating) : query.OrderBy(x => x.Rating);
                    break;
                case FilterSettings.Year:
                    query = isDescending ? query.OrderByDescending(x => x.ReleaseYear) : query.OrderBy(x => x.ReleaseYear);
                    break;
                case FilterSettings.Name:
                    query = isDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                    break;
                default:
                    break;
            }
            return await query
                .Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize)
                .Take(pagingParameters.PageSize)
                .ToListAsync();
        }
        public async Task<IEnumerable<Film?>> SearchAll(PagingParameters pagingParameters, string searchParameter)
        {
            var query = _filmRepo.Query();
            if (query == null)
                throw new NotFoundException("No Films Found");
            return await query.Where(x => x.Name.Contains(searchParameter)).ToListAsync();
        }

        public async Task<IEnumerable<Film?>> GetSortedByRating(PagingParameters pagingParameters)
        {
            var query = _filmRepo.Query();
            if (query == null)
                throw new NotFoundException("No Films Found");
            return await query.OrderBy(x => x.Rating)
                .Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize)
                .Take(pagingParameters.PageSize)
                .ToListAsync();
        }
        public async Task<IEnumerable<Film?>> GetSortedByYear(PagingParameters pagingParameters)
        {
            var query = _filmRepo.Query();
            if (query == null)
                throw new NotFoundException("No Films Found");
            return await query.OrderBy(x => x.ReleaseYear)
                .Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize)
                .Take(pagingParameters.PageSize)
                .ToListAsync();
        }
        public async Task<IEnumerable<Film?>> GetSortedByName(PagingParameters pagingParameters)
        {
            var query = _filmRepo.Query();
            if (query == null)
                throw new NotFoundException("No Films Found");
            return await query.OrderBy(x => x.Name[0])
                .Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize)
                .Take(pagingParameters.PageSize)
                .ToListAsync();
        }
        public async Task<IEnumerable<Film?>> FilterByYear(PagingParameters pagingParameters, int lowerLimit, int upperLimit)
        {
            var query = _filmRepo.Query();
            if (query == null)
                throw new NotFoundException("No Films Found");
            return await (from item in query
                          where item.ReleaseYear >= lowerLimit && item.ReleaseYear <= upperLimit
                          select item)
                          .Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize)
                          .Take(pagingParameters.PageSize)
                          .ToListAsync();
        }
        public async Task<IEnumerable<Film?>> FilterByScore(PagingParameters pagingParameters, float lowerLimit, float upperLimit)
        {
            var query = _filmRepo.Query();
            if (query == null)
                throw new NotFoundException("No Films Found");

            return await (from item in query
                          where item.Rating >= lowerLimit && item.Rating <= upperLimit
                          select item)
                          .Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize)
                          .Take(pagingParameters.PageSize)
                          .ToListAsync();
        }

        public async Task<Genre> GetGenre(int id)
        {
            Genre? genre = await _genreRepo.Query().Where(genre => genre.Id == id).SingleOrDefaultAsync();
            if (genre is null)
            {
                throw new NotFoundException("genre id is incorrect");
            }
            return genre;
        }

        public async Task<int> GetGenreIdByName(string name)
        {
            Genre? genre = await _genreRepo.Query().Where(genre => genre.Name == name).SingleOrDefaultAsync();
            if (genre is null)
            {
                throw new NotFoundException("genre name is incorrect");
            }
            return genre.Id;
        }

        public async Task AddGenre(string name)
        {
            var genre = new Genre()
            {
                Name = name,
                TmdbId = null
            };
            _genreRepo.Add(genre);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteGenre(int id)
        {
            Genre? genre = await _genreRepo.Query().Where(genre => genre.Id == id).SingleOrDefaultAsync();
            if (genre is null)
            {
                throw new NotFoundException("genre id is incorrect");
            }
            _genreRepo.Remove(genre);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EditGenre(int id, string name)
        {
            Genre? genre = await _genreRepo.Query().Where(genre => genre.Id == id).SingleOrDefaultAsync();
            if (genre is null)
            {
                throw new NotFoundException("genre id is incorrect");
            }
            genre.Name = name;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddFilm_Genre(int filmId, int genreId)
        {
            Genre? genre = await _genreRepo.Query().Where(genre => genre.Id == genreId).SingleOrDefaultAsync();
            if (genre is null)
            {
                throw new NotFoundException("genre id is incorrect");
            }
            Film? film = await _filmRepo.Query().Where(film => film.Id == filmId).SingleOrDefaultAsync();
            if (film is null)
            {
                throw new NotFoundException("genre id is incorrect");
            }
            Film_Genre film_Genre = new();
            film_Genre.FilmId = filmId;
            film_Genre.GenreId = genreId;
            _film_genreRepo.Add(film_Genre);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteFilm_Genre(int filmId, int genreId)
        {
            Film_Genre? film_Genre = await _film_genreRepo.Query().Where(x => x.FilmId == filmId && x.GenreId == genreId).SingleOrDefaultAsync();
            if (film_Genre is null)
            {
                throw new NotFoundException("filmId and/or genreId is incorrect");
            }
            _film_genreRepo.Remove(film_Genre);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<int>> GetFilmsOfGenre(int genreId)
        {
            return await _film_genreRepo.Query().Where(x => x.GenreId == genreId).Select(x => x.FilmId).ToListAsync();

        }

        public async Task<IEnumerable<int>> GetGenresOfFilm(int filmId)
        {
            return await _film_genreRepo.Query().Where(x => x.FilmId == filmId).Select(x => x.GenreId).ToListAsync();
        }
    }
}
