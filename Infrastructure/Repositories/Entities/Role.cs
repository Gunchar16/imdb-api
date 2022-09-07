using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories.Entities
{
    public class Role:BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Tier { get; set; }
    }
}
