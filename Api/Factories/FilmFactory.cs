using Imdb.Infrastructure.Entities;

namespace Api.Factories
{
    //Having the same List for multiple requests.
    public static class FilmFactory
    {
        public static List<Film?> FilmList;
    }
}
