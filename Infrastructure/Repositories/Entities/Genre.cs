using Infrastructure.Repositories.Entities;
using System.ComponentModel.DataAnnotations;

namespace Imdb.Infrastructure.Entities
{
    public class Genre : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public int? TmdbId { get; set; }

    }
}
