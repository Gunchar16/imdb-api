using Imdb.Infrastructure.Entities;
using Imdb.Infrastructure.IRepository;
using Infrastructure.Repositories.IRepository;

namespace Application.Services.Reviews
{
    public interface IReviewService
    {
        Task Add(string Text, int? Rating, int UserId, int FilmId);
        Task Edit(string Text, int? Rating, int UserId, int reviewId);
        Task Delete(int reviewId, int userId);
        Task<IEnumerable<Review?>> GetReviews(int FilmId);
        Task<Review?> GetReview(int reviewId);
    }
}
