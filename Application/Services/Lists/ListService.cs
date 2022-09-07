using Imdb.Infrastructure.Entities;
using Imdb.Services.Users;
using Infrastructure.Paging;
using Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Exceptions;

namespace Application.Services.Lists
{
    public class ListService : IListService
    {
        private readonly IRepository<ListImdb> _listRepo;
        private readonly IUnitOfWork _iunitOfWork;
        private readonly IRepository<Film_List> _FilmListRepo;
        private readonly IFilmRepository _filmRepository;

        public ListService(IRepository<ListImdb> list, IUnitOfWork iunitOfWork, IRepository<Film_List> filmlist, IFilmRepository Film)
        {
            _listRepo = list;
            _iunitOfWork = iunitOfWork;
            _FilmListRepo = filmlist;
            _filmRepository = Film;
        }

        public async Task addFilm(int ListId, int FilmId, int UserId)
        {
            var film = await _filmRepository.GetSingleOrDefaultAsync(FilmId);
            var List = await _listRepo.Query().Where(x =>
                (x.Id == ListId) && (x.UserId == UserId)).SingleOrDefaultAsync();
            if (List == null) throw new NotFoundException("List");
            if (film == null) throw new NotFoundException("Film");
            var films = await _FilmListRepo.Query().AnyAsync(x => (x.ListId == List.Id) && (x.FilmId == film.Id));
            if(films) throw new AppException("Film already is added");
         
            Film_List film_List = new();
            film_List.ListId = List.Id;
            film_List.FilmId = film.Id;
            _FilmListRepo.Add(film_List);
            await _iunitOfWork.SaveChangesAsync();

        }

        public async Task<ListDto> GetList(PagingParameters? pagingParameters, int ListId,int UserId)
        {
            var List = await _listRepo.Query().Where(x =>
                (x.Id == ListId) && (x.UserId == UserId)).SingleOrDefaultAsync();
            if (List == null) throw new NotFoundException("List");
            var films = _FilmListRepo.Query().Where(x => x.ListId == List.Id);

            if(pagingParameters != null)
            {
                films = films.Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize)
                .Take(pagingParameters.PageSize);
            }
                
              
            return new ListDto(List.Name,List.Description, await films.Select(x => x.Film).ToListAsync(),List.Id);
             

        }
        public async Task<List<ListImdb>> GetListsByUser(PagingParameters? pagingParameters,int userid)
        {
            var lists =  _listRepo.Query().Where(x => x.UserId == userid);

            if (pagingParameters != null)
            {
                lists = lists.Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize)
                .Take(pagingParameters.PageSize);
            }
                
            var listsOut = await lists.ToListAsync();
            if(listsOut == null) throw new NotFoundException("Lists");
            
            return listsOut;
        }
        
        public async Task Addlist(string Name, string Des, int UserId)
        {
            var checkList = await _listRepo.Query().Where(x => (x.Name == Name) && (x.Id == UserId)).AnyAsync();
            if (checkList) throw new AppException("List already exists");
            ListImdb list = new()
            {
                Name = Name,
                Description = Des,
                UserId = UserId
            };
            _listRepo.Add(list);
            await _iunitOfWork.SaveChangesAsync();
        }

        public async Task RenameList(string Name,int Id,int UserId)
        {
            var list = await _listRepo.Query().Where(x => (x.Id == Id) && (x.UserId == UserId)).SingleOrDefaultAsync();
            if (list == null) throw new NotFoundException("List");
            list.Name = Name;
            await _iunitOfWork.SaveChangesAsync();
            
        }
        public async Task RemoveList(int ListId,int UserId)
        {
            var List = await _listRepo.Query().Where(x =>
                (x.Id == ListId) && (x.UserId == UserId)).SingleOrDefaultAsync();
            if (List == null) throw new NotFoundException("List");
            var filmList = _FilmListRepo.Query().Where(x => x.ListId == List.Id);
            foreach (var film in filmList)
            {
                _FilmListRepo.Remove(film);
            }
            _listRepo.Remove(List);
            await _iunitOfWork.SaveChangesAsync();
            
        }
        public async Task RemoveFIlm(int ListId, int FilmId, int UserId)
        {
            var List = await _listRepo.Query().Where(x =>
                (x.Id == ListId) && (x.UserId == UserId)).SingleOrDefaultAsync();
            if (List == null) throw new NotFoundException("List");
            var filmToRemove = await _FilmListRepo.Query().Where(x => (x.ListId == List.Id) && (x.FilmId == FilmId)).SingleAsync();
            if (filmToRemove == null) throw new AppException("Film not found");
            _FilmListRepo.Remove(filmToRemove);
            await _iunitOfWork.SaveChangesAsync();

        }
        public async Task<int> ListNameToId(string Name,int UserId)
        {
            var list = await _listRepo.Query().Where(x => (x.Name == Name) && (x.UserId == UserId)).SingleOrDefaultAsync();
            if (list == null) throw new NotFoundException("List Not found");
            return list.Id;
        }
        
    }
}
