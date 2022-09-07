using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories.DTOs
{
    public record ResetPasswordDto([Required] string OldPassword, [Required] string NewPassword, [Required] string NewPasswordR);

}
