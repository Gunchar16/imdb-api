using Imdb.Infrastructure.Entities;
using Infrastructure.Repositories.DTOs;

namespace Application.Services.Persons
{
    public interface IPersonService
    {
        Task<Person?> Get(int id);
        Task Add(PersonDTO p);
        Task Delete(int id);
        Task Edit(int personId, PersonDTO p);
        Task AddPerson_Film(int personId, int filmId);
        Task RemovePerson_Film(int personId, int filmId);
        Task<IEnumerable<Person_Film>> GetFilmsOfPerson(int personId);
        Task<IEnumerable<Person_Film>> GetPeopleOfFilm(int filmId);
        Task<Person?> GetPersonIdByName(string name);
        Task<Imdb.Infrastructure.Entities.Type> GetGenre(int id);
        Task<int> GetGenreIdByName(string name);
        Task AddType(string name);
        Task EditType(int id, string name);
        Task DeleteType(int id);
        Task AddPerson_Type(int filmId, int genreId);
        Task DeletePerson_Type(int filmId, int genreId);
        Task<IEnumerable<Person_Type>> GetPersonsOfType(int genreId);
        Task<IEnumerable<Person_Type>> GetTypesOfPerson(int filmId);
    }
}
