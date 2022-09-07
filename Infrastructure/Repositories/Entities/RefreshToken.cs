using Imdb.Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Repositories.Entities
{
    public class RefreshToken:BaseEntity
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string JwtId { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Required]
        public bool invalidated { get; set; }
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
