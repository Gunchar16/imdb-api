using Application.Services.Films;
using Imdb.Infrastructure.Entities;
using Infrastructure.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.ApplicationInfrastructure;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers
{


    [ApiController]
    [Route("")]
    public class GenreController : ControllerBase
    {
        private readonly IFilmService _filmService;

        public GenreController(IFilmService filmService)
        {
            _filmService = filmService;
        }
        [HttpGet("genres/{id}")]
        public async Task<ActionResult<Genre>> GetGenre(int id)
        {
            return Ok(await _filmService.GetGenre(id));
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("genres")]
        public async Task<IActionResult> AddGenre(string name)
        {
            await _filmService.AddGenre(name);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("genres/{genreId}")]
        public async Task<IActionResult> EditGenre(int genreId, string name)
        {
            await _filmService.EditGenre(genreId, name);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("genres/{genreId}")]
        public async Task<IActionResult> DeleteGenre(int genreId)
        {
            await _filmService.DeleteGenre(genreId);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("films/{filmId}/genres/{genreId}")]
        public async Task<IActionResult> AddFilm_Genre(int filmId, int genreId)
        {
            await _filmService.AddFilm_Genre(filmId, genreId);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("films/{filmId}/genres/{genreId}")]
        public async Task<IActionResult> DeleteFilm_Genre(int filmId, int genreId)
        {
            await _filmService.DeleteFilm_Genre(filmId, genreId);
            return Ok();
        }
        [HttpGet("genres/{genreId}/films")]
        public async Task<ActionResult<Film_Genre>> GetFilmsOfGenre(int genreId)
        {

            return Ok(await _filmService.GetFilmsOfGenre(genreId));
        }
        [HttpGet("films/{filmId}/genres")]
        public async Task<ActionResult<Film_Genre>> GetGenresOfFilm(int filmId)
        {

            return Ok(await _filmService.GetGenresOfFilm(filmId));
        }
    }
}
