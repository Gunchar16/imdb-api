using Application.Services.Films;
using Imdb.Infrastructure.Entities;
using Infrastructure.Filter;
using Infrastructure.Paging;
using Infrastructure.Repositories.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.ApplicationInfrastructure;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers
{
    [ApiController]
    [Route("")]
    public class FilmController : ControllerBase
    {
        private readonly IFilmService _filmService;

        public FilmController(IFilmService filmService)
        {
            _filmService = filmService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("films")]
        public async Task<IActionResult> AddFilm([FromForm] FilmDto film)
        {
            await _filmService.AddFilm(film.Name, film.Description, film.ReleaseYear, film.Image);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("films/{filmId}")]
        public async Task<IActionResult> EditFilm(int filmId, [FromForm] FilmDto film)
        {
            await _filmService.EditFilm(filmId, film.Name, film.Description, film.ReleaseYear, film.Image);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("films/{filmId}")]
        public async Task<IActionResult> DeleteFilm(int filmId)
        {
            await _filmService.DeleteFilm(filmId);
            return Ok();
        }
        [HttpGet("films/{filmId}")]
        public async Task<ActionResult<Film?>> GetFilm(int filmId)
        {
            return Ok(await _filmService.GetFilm(filmId));
        }
        [HttpGet("films")]
        public async Task<ActionResult<Film?>> GetAll([FromQuery] PagingParameters pagingParameters, [FromQuery] FilterSettings? sortSettings, [FromQuery] FilterSettings? filterSettings, float? lowerLimit, float? upperLimit, bool isDescending)
        {
            return Ok(await _filmService.GetAll(pagingParameters, sortSettings, filterSettings, lowerLimit, upperLimit, isDescending));
        }
        [HttpGet("search")]
        public async Task<ActionResult<Film?>> SearchAll([FromQuery] PagingParameters pagingParameters, string searchParameter)
        {
            return Ok(await _filmService.SearchAll(pagingParameters, searchParameter));
        }
    }
}
