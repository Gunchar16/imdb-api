using Imdb.Infrastructure.Entities;

namespace Application.Services.Reactions
{
    public interface IReactionService
    {
        Task Like(int ReviewId, int UserId);
    }
}
 