using Imdb.Infrastructure.Entities;
using Imdb.Services.Users;
using Infrastructure.Paging;

namespace Application.Services.Lists
{
     
    public interface IListService
    {
        Task addFilm(int ListId, int FilmId, int UserId);
        Task Addlist(string Name, string Des, int UserId);
        Task<ListDto> GetList(PagingParameters? pagingParameters,int ListId, int UserId);
        Task RemoveFIlm(int ListId, int FilmId, int UserId);
        Task RemoveList(int ListId, int UserId);
        Task<int> ListNameToId(string Name, int UserId);
        Task RenameList(string Name, int Id, int UserId);
        Task<List<ListImdb>> GetListsByUser(PagingParameters? pagingParameters,int userid);
    }
}
