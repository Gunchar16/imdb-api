using Imdb.Infrastructure.Repository;
using Imdb.Services.Users;
using Infrastructure.Repositories.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ApplicationInfrastructure;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers
{

    [ApiController]
    [Route("")]
    public class AuthController : ControllerBase
    {
        public readonly IUserService _userService;
        public readonly ApplicationContext _context;
        public AuthController(IUserService userService, ApplicationContext context)
        {
            _userService = userService;
            _context = context;
        }

        [HttpPost("auth/login")]
        public async Task<ActionResult<UserTokenDto>> Login(LoginDto model)
        {
            var user = await _userService.Login(model.Username, model.Password);
            return user != null ? Ok(user) : BadRequest("User Model incorrect");
        }

        [HttpPost("auth/register")]
        public async Task<ActionResult> Register(RegistrationDTO req)
        {
            await _userService.Register(req);
            return Ok();
        }

        [Authorize]
        [HttpGet("auth/me")]
        public async Task<ActionResult<UserDto>> UserContext()
        {

            return Ok(await _userService.FindByIdAsync(_context.UserId.Value));
        }

        [HttpPut("auth/changePassword")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(ResetPasswordDto model)
        {
            await _userService.ChangePassword(model.OldPassword, model.NewPassword, model.NewPasswordR, _context.UserId.Value);
            return Ok();
        }

        [HttpPost("auth/forgotPassword")]
        public async Task<ActionResult> ForgotPassword(string username)
        {
            var callbackUrl = Url.Action(
               "Recovery", "Auth",
               new { },
               protocol: Request.Scheme);
            await _userService.ForgotPassword(username, callbackUrl);
            return Ok();
        }

        [HttpGet("auth/recovery")]
        public IActionResult Recovery(string token)
        {
            return Ok(token);
        }

        [HttpPost("auth/recovery")]
        public async Task<IActionResult> Recovery(string token, string NewPassword, string OldPassword)
        {
            return Ok(await _userService.Recover(token, NewPassword, OldPassword));


        }
        [Authorize]
        [HttpPost("auth/refreshToken")]
        public async Task<IActionResult> RefreshToken(UserTokenDto userTokenDto)
        {
            return Ok(await _userService.RefreshToken(userTokenDto.JwtToken,userTokenDto.RefreshToken, _context.UserId.Value));


        }
        [Authorize]
        [HttpPost("auth/revokeToken")]
        public async Task<IActionResult> RevokeToken(string token)
        {
            await _userService.RevokeToken(token);
            return Ok();


        }



    }
}
