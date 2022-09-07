using Infrastructure.Repositories.Entities;
using System.ComponentModel.DataAnnotations;

namespace Imdb.Infrastructure.Entities
{
    public class Film : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public double Rating { get; set; }
        [Required]
        public int ReleaseYear { get; set; }
        public string? ImageURL { get; set; }
    }
}
