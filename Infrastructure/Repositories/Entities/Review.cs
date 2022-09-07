using Infrastructure.Repositories.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imdb.Infrastructure.Entities
{
    public class Review : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Text { get; set; } = string.Empty;
        public int? Rating { get; set; }
        [Required]
        public int Like { get; set; } = 0;

        [ForeignKey("FilmId")]
        public int FilmId { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
    }
}
