using Infrastructure.Repositories.Entities;
using System.ComponentModel.DataAnnotations;

namespace Imdb.Infrastructure.Entities
{
    public class Type : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
