using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories.DTOs
{
    public record FilmDto([Required] string Name, [Required] string Description, [Required][Range(0.0, 60)] int MinuteDuration, [Required][Range(0, 2022)] int ReleaseYear, [Display(Name = "Image")][Required] IFormFile Image);

}
