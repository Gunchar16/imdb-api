using Application.Services.Lists;
using Infrastructure.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ApplicationInfrastructure;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Repositories.DTOs;


namespace Api.Controllers
{


    [ApiController]
    [Authorize]
    [Route("")]
    public class FilmListController : ControllerBase
    {
        public readonly IListService _listService;
        public readonly ApplicationContext _context;
        public FilmListController(IListService listService, ApplicationContext context)
        {
            _listService = listService;
            _context = context;
        }

       
        [HttpPost("lists")]
        public async Task<IActionResult> CreateList(CreateListDto createList)
        {
            if (_context.UserId == null) return BadRequest("Missing Id");
            await _listService.Addlist(createList.ListName, createList.Description, (int)_context.UserId);
            return Ok();
        }
        [HttpGet("lists/{listId}")]
        public async Task<IActionResult> GetList([FromQuery] PagingParameters pagingParameters, [Required] int listId)
        {
            return Ok(await _listService.GetList(pagingParameters, listId, _context.UserId.Value));
        }
        [HttpPut("lists/{listId}")]
        public async Task<IActionResult> RenameList(int listId,RenameListDto renameList)
        {
            if (_context.UserId == null) return BadRequest("Missing Id");
            await _listService.RenameList(renameList.NewName, listId, (int)_context.UserId);
            return Ok();
        }

        [HttpGet("users/{userId}/list")]
        public async Task<IActionResult> GetListByUser([FromQuery] PagingParameters pagingParameters, [Required] int userId)
        {
            return Ok(await _listService.GetListsByUser(pagingParameters, userId));
        }
        [HttpDelete("lists/{listId}")]
        public async Task<IActionResult> RemoveList([Required] int listId)
        {

            await _listService.RemoveList(listId, _context.UserId.Value);
            return Ok();
        }
        [HttpDelete("lists/{listId}/films/{filmId}")]
        public async Task<IActionResult> RemoveFilm([Required] int listId, [Required] int filmId)
        {

            await _listService.RemoveFIlm(listId, filmId, _context.UserId.Value);
            return Ok();
        }
        [HttpPost("lists/{listId}/films/{filmId}")]
        public async Task<IActionResult> AddFilm([Required] int listId, [Required] int filmId)
        {
            if (_context.UserId == null) return BadRequest("Missing Id");
            await _listService.addFilm(listId, filmId, (int)_context.UserId);
            return Ok();
        }
    }
}
