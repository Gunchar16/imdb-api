using Microsoft.AspNetCore.Mvc;
using Shared;
using Imdb.Infrastructure.Entities;
using Infrastructure.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;
using Infrastructure.Repositories.DTOs;

namespace Application.Services.Persons
{
    public class PersonService : IPersonService
    {
        private readonly IRepository<Person> _personRepo;
        private readonly IRepository<Imdb.Infrastructure.Entities.Type> _typeRepo;
        private readonly IRepository<Person_Type> _person_typeRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFilmRepository _filmRepo;
        private readonly IRepository<Person_Film> _person_FilmRepo;

        public PersonService(IRepository<Person> personRepo, IRepository<Imdb.Infrastructure.Entities.Type> typeRepo, IRepository<Person_Type> person_typeRepo, IUnitOfWork unitOfWork, IFilmRepository filmRepo, IRepository<Person_Film> person_FilmRepo)
        {
            _personRepo = personRepo;
            _typeRepo = typeRepo;
            _person_typeRepo = person_typeRepo;
            _unitOfWork = unitOfWork;
            _filmRepo = filmRepo;
            _person_FilmRepo = person_FilmRepo;
        }
        public async Task<Person?> Get(int id)
        {
            Person? person = await _personRepo.Query().Where(x => x.Id == id).SingleOrDefaultAsync();
            if (person == null) throw new NotFoundException("Person not found");
            return person;
        }
        public async Task Add(PersonDTO p)
        {
            _personRepo.Add(new Person
            {
                Name = p.Name,
                Bio = p.Bio,
                Age = p.Age,
                ImageURL = p.ImageURL
            });
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            Person? person = await _personRepo.Query().Where(x => x.Id == id).SingleOrDefaultAsync();
            if (person == null) throw new NotFoundException("Person not found");
            _personRepo.Remove(person);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task Edit(int personId, PersonDTO p)
        {
            Person? person = await _personRepo.Query().Where(x => x.Id == personId).SingleOrDefaultAsync();
            if (person == null) throw new NotFoundException("Person not found");
            person.Name = p.Name;
            person.Bio = p.Bio;
            person.Age = p.Age;
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task AddPerson_Film(int personId, int filmId)
        {
            Person? person = await _personRepo.Query().Where(x => x.Id == personId).SingleOrDefaultAsync();
            if (person == null) throw new NotFoundException("Person not found");
            Film? film = await _filmRepo.Query().Where(x => x.Id == filmId).SingleOrDefaultAsync();
            if (person == null) throw new NotFoundException("Film not found");

            Person_Film person_film = new();
            person_film.FilmId = filmId;
            person_film.PersonId = personId;
            _person_FilmRepo.Add(person_film);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemovePerson_Film(int personId, int filmId)
        {
            Person_Film? pf = await _person_FilmRepo.Query().Where(x => x.PersonId == personId && x.FilmId == filmId).SingleOrDefaultAsync();
            if (pf == null) throw new NotFoundException("entry not found");
            _person_FilmRepo.Remove(pf);
            await _unitOfWork.SaveChangesAsync();
        }
        // todo: how can i return just ids?
        public async Task<IEnumerable<Person_Film>> GetFilmsOfPerson(int personId)
        {
            return await _person_FilmRepo.Query().Where(x => x.PersonId == personId).ToListAsync();
        }

        public async Task<IEnumerable<Person_Film>> GetPeopleOfFilm(int filmId)
        {
            return await _person_FilmRepo.Query().Where(x => x.FilmId == filmId).ToListAsync();
        }

        public async Task<Person?> GetPersonIdByName(string name)
        {
            Person? p = await _personRepo.Query().Where(x => x.Name == name).SingleOrDefaultAsync();
            if (p == null) throw new NotFoundException("Person not found");
            return p;
        }

        public async Task AddType(string name)
        {
            Imdb.Infrastructure.Entities.Type type = new();
            type.Name = name;
            _typeRepo.Add(type);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EditType(int id, string name)
        {
            var type = await _typeRepo.Query().Where(genre => genre.Id == id).SingleOrDefaultAsync();
            if (type is null)
            {
                throw new NotFoundException("type id is incorrect");
            }
            type.Name = name;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteType(int id)
        {
            Imdb.Infrastructure.Entities.Type? type = await _typeRepo.Query().Where(type => type.Id == id).SingleOrDefaultAsync();
            if (type is null)
            {
                throw new NotFoundException("type id is incorrect");
            }
            _typeRepo.Remove(type);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddPerson_Type(int personId, int typeId)
        {
            Imdb.Infrastructure.Entities.Type? type = await _typeRepo.Query().Where(type => type.Id == typeId).SingleOrDefaultAsync();
            if (type is null)
            {
                throw new NotFoundException("type id is incorrect");
            }
            Person? person = await _personRepo.Query().Where(person => person.Id == personId).SingleOrDefaultAsync();
            if (person is null)
            {
                throw new NotFoundException("type id is incorrect");
            }
            Person_Type person_Genre = new();
            person_Genre.PersonId = personId;
            person_Genre.TypeId = typeId;
            _person_typeRepo.Add(person_Genre);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletePerson_Type(int personId, int typeId)
        {
            Person_Type? person_type = await _person_typeRepo.Query().Where(x => x.PersonId == personId && x.TypeId == typeId).SingleOrDefaultAsync();
            if (person_type is null)
            {
                throw new NotFoundException("personId and/or typeId is incorrect");
            }
            _person_typeRepo.Remove(person_type);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<Person_Type>> GetPersonsOfType(int typeId)
        {
            List<Person_Type>? lst = await _person_typeRepo.Query().Where(x => x.TypeId == typeId).ToListAsync();
            return lst;
        }

        public async Task<IEnumerable<Person_Type>> GetTypesOfPerson(int personId)
        {
            List<Person_Type>? lst = await _person_typeRepo.Query().Where(x => x.PersonId == personId).ToListAsync();
            return lst;
        }

        public async Task<Imdb.Infrastructure.Entities.Type> GetGenre(int id)
        {
            Imdb.Infrastructure.Entities.Type? type = await _typeRepo.Query().Where(type => type.Id == id).SingleOrDefaultAsync();
            if (type is null)
            {
                throw new NotFoundException("type id is incorrect");
            }
            return type;
        }

        public async Task<int> GetGenreIdByName(string name)
        {
            Imdb.Infrastructure.Entities.Type? type = await _typeRepo.Query().Where(type => type.Name == name).SingleOrDefaultAsync();
            if (type is null)
            {
                throw new NotFoundException("type name is incorrect");
            }
            return type.Id;
        }
    }
}
