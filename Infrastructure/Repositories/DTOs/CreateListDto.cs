using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories.DTOs
{
    public record CreateListDto([Required] string ListName, [Required] string Description);

}
