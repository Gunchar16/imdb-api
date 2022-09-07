using AutoMapper;
using Imdb.Infrastructure.Entities;
using Imdb.Infrastructure.Repository;
using Imdb.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ApplicationInfrastructure;
using System.ComponentModel.DataAnnotations;

namespace Imdb.Controllers
{

    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AdminController(IUserService userService, ApplicationContext context, IMapper mapper)
        {
            _userService = userService;
            _context = context;
            _mapper = mapper;
        }

        //[HttpGet("GetUserByUserName")]
        //public async Task<ActionResult<User>> Get([Required] string userName)
        //{
        //    return Ok(await _userService.UserNameToId(userName));

        //}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<User>> Get(int userId)
        {
            return Ok(await _userService.FindByIdAsync(userId));

        }
        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            return Ok(await _userService.DeleteUser(_context.UserId.Value, userId));
        }
        [HttpPut("user/{userId}")]
        public async Task<IActionResult> EditUser(int userId, RegistrationDTO user)
        {
            return Ok(await _userService.EditUser(userId, user));
        }
    }
}