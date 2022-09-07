using Infrastructure.Repositories.Entities;
using System.ComponentModel.DataAnnotations;

namespace Imdb.Infrastructure.Entities
{
    public class Person : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Bio { get; set; } = string.Empty;
        [Required]
        public int Age { get; set; }
        public string? ImageURL { get; set; }
    }
}
