using Imdb.Infrastructure.Entities;
using Imdb.Infrastructure.IRepository;

namespace Infrastructure.Repositories.DTOs
{
    public class ApiMovieRootObject
    {
        public ApiMovie[] results { get; set; }
    }

    public class ApiMovie
    {
        public int[] genre_ids { get; set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
        public string release_date { get; set; }
    }

}