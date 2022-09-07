using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories.DTOs
{
    public record LoginDto([Required] string Username, [Required] string Password);

}
