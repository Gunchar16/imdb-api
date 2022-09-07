using Infrastructure.Repositories.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imdb.Infrastructure.Entities
{
    public class Film_Genre: BaseEntity
    {
        [Required]
        public int FilmId { get; set; }
        [Required]
        [ForeignKey("FilmId")]
        public Film Film { get; set; }
        [Required]
        public int GenreId { get; set; }
        [Required]
        [ForeignKey("GenreId")]
        public Genre Genre { get; set; }
    }
}

