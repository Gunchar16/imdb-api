using Infrastructure.Repositories.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imdb.Infrastructure.Entities
{
    public class Film_List : BaseEntity
    {
        [Required]
        public int ListId { get; set; }
        [Required]
        [ForeignKey("ListId")]
        public ListImdb List { get; set; }
        public int FilmId { get; set; }
        [Required]
        [ForeignKey("FilmId")]
        public Film Film { get; set; }
    }
}
