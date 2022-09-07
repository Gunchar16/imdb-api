using Imdb.Infrastructure.Entities;
using Imdb.Infrastructure.Repository;
using System.IdentityModel.Tokens.Jwt;

namespace Imdb.Services.Users
{
    public record UserTokenDto(string JwtToken,string RefreshToken);
    public record UserDto(int Id, string Username, int RoleId, DateTime DateCreated, DateTime? DateUpdated);
    public record UserChangePasswordDto(string Username,string OldPassword,string NewPassword);
    public record ListDto(string Name,string des,List<Film> Films,int Id);
    public interface IUserService
    {
        Task RevokeToken(string token);
        Task<User> UserNameToId(string userName);
        Task<UserTokenDto?> Login(string Username, string Password);
        Task ChangePassword(string oldP, string newP, string newPR, int UserId);
        Task<User> FindByIdAsync(int id);
        Task Register(RegistrationDTO req);
        Task ForgotPassword(string username,string url);
        Task<User> DeleteUser(int userId, int idToDelete);
        Task<User> EditUser(int userId, RegistrationDTO editUser);
        Task<UserTokenDto> RefreshToken(string token, string refreshToken, int userid);
        Task<User> Recover(string token, string NewP, string OldP);
    }
}
