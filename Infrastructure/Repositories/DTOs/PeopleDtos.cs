using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories.DTOs
{
    public class PersonDTO
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
