using Infrastructure.Repositories.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imdb.Infrastructure.Entities
{
    public class User : BaseEntity
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public int Age { get; set; } = 0;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Salt { get; set; } = string.Empty;
        [Required]
        public int RoleId { get; set; } = 0;
        [Required]
        [ForeignKey("RoleId")]
        public Role Role { get; set; } 
        [Required]
        public string Bio { get; set; } = String.Empty;
    }
}
