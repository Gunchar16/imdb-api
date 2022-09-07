using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories.DTOs
{
    public record ReviewDto([Required] string Text, [Range(1.0, 10.0, ErrorMessage = "The field must be between 1-10 range")] int? Rating = null);

}
