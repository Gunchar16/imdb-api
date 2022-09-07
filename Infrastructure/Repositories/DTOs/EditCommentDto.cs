using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories.DTOs
{
    public record EditCommentDto([Required] string Message);

}
