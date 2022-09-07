using Application.Services.Persons;
using Imdb.Infrastructure.Entities;
using Infrastructure.Repositories.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("")]
    public class PersonController : ControllerBase
    {
        public readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet("people/{personId}")]
        public async Task<ActionResult<Person?>> Get(int personId)
        {
            Person? person = await _personService.Get(personId);
            return Ok(person);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("people")]
        public async Task<IActionResult> Add(PersonDTO person)
        {
            await _personService.Add(person);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("people/{personId}")]
        public async Task<IActionResult> Delete(int personId)
        {
            await _personService.Delete(personId);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("people/{personId}")]
        public async Task<IActionResult> Edit(int personId, PersonDTO p)
        {
            await _personService.Edit(personId, p);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("people/{personId}/films/{filmId}")]
        public async Task<IActionResult> AddPerson_Film(int personId, int filmId)
        {
            await _personService.AddPerson_Film(personId, filmId);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("people/{personId}/films/{filmId}")]
        public async Task<IActionResult> RemovePerson_Film(int personId, int filmId)
        {
            await _personService.RemovePerson_Film(personId, filmId);
            return Ok();
        }

        [HttpGet("people/{personId}/films")]
        public async Task<ActionResult<IEnumerable<Person_Film>>> GetFilmsOfPerson(int personId)
        {
            return Ok(await _personService.GetFilmsOfPerson(personId));
        }

        [HttpGet("films/{filmId}/people")]
        public async Task<ActionResult<IEnumerable<Person_Film>>> GetPeopleOfFilm(int filmId)
        {
            return Ok(await _personService.GetPeopleOfFilm(filmId));
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("types")]
        public async Task<IActionResult> AddType(string name)
        {
            await _personService.AddType(name);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("types/{typeId}")]
        public async Task<IActionResult> EditType(int typeId, string name)
        {
            await _personService.EditType(typeId, name);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("types/{typeId}")]
        public async Task<IActionResult> DeleteType(int typeId)
        {
            await _personService.DeleteType(typeId);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("people/{personId}/types/{typeId}")]
        public async Task<IActionResult> AddPerson_Type(int personId, int typeId)
        {
            await _personService.AddPerson_Type(personId, typeId);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("people/{personId}/types/{typeId}")]
        public async Task<IActionResult> DeletePerson_Type(int personId, int typeId)
        {
            await _personService.DeletePerson_Type(personId, typeId);
            return Ok();
        }
        [HttpGet("types/{typeId}")]
        public async Task<ActionResult<Person_Type>> GetPersonsOfType(int typeId)
        {
            return Ok(await _personService.GetPersonsOfType(typeId));
        }
        [HttpGet("people/{personId}/types")]
        public async Task<ActionResult<Person_Type>> GetTypesOfPerson(int personId)
        {
            return Ok(await _personService.GetTypesOfPerson(personId));
        }
    }
}
