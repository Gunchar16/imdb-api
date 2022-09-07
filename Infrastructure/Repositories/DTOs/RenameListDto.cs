using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories.DTOs
{
    public record RenameListDto([Required] string NewName);

}
