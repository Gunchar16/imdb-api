using Infrastructure.Repositories.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imdb.Infrastructure.Entities
{
    public class Person_Film : BaseEntity
    {
        [Required]
        public int PersonId { get; set; }
        [Required]
        [ForeignKey("PersonId")]
        public Person Person { get; set; }
        [Required]
        public int FilmId { get; set; }
        [Required]
        [ForeignKey("FilmId")]
        public Film Film { get; set; }
    }
}
