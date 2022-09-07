using Infrastructure.Repositories.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imdb.Infrastructure.Entities
{
    public class Reaction : BaseEntity
    {
        [Required]
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        [Required]
        [ForeignKey("ReviewId")]
        public int ReviewId { get; set; }
    }
}
