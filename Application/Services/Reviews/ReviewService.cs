using Imdb.Infrastructure.Entities;
using Imdb.Infrastructure.IRepository;
using Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Exceptions;

namespace Application.Services.Reviews
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> _review;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFilmRepository _filmRepository;

        public ReviewService(IRepository<Review> review, IUserRepository userRepository, IUnitOfWork unitOfWork, IFilmRepository filmRepository)
        {
            _review = review;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _filmRepository = filmRepository;
        }

        public async Task Add(string Text, int? Rating, int UserId, int FilmId)
        {
            if (await _review.Query().Where(x => x.FilmId == FilmId && x.UserId == UserId).AnyAsync())
                throw new AppException("Review already placed");

            var user = _userRepository.GetSingleOrDefaultAsync(UserId).GetAwaiter().GetResult();
            if (user is null)
                throw new NotFoundException("user id is incorrect");

            _review.Add(new Review()
            {
                Text = Text,
                Rating = Rating,
                UserId = UserId,
                Name = user.UserName,
                Date = DateTime.UtcNow,
                Like = 0,
                FilmId = FilmId
            });
            if (Rating == null)
            {
                await _unitOfWork.SaveChangesAsync();
                return;
            }
            var ratingCount = (await _review.Query().Where(x => x.FilmId == FilmId && x.Rating != null).ToListAsync()).Count;
            var film = await _filmRepository.Query().SingleOrDefaultAsync(x => x.Id == FilmId);
            if (film is null)
                throw new NotFoundException("Film id is incorrect");
            film.Rating = ((double)Rating.Value + film.Rating) / (ratingCount + 1);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task Edit(string Text, int? Rating, int UserId, int reviewId)
        {
            var query = _review.Query().Where(x => x.Id == reviewId);
            if (!await query.AnyAsync())
                throw new NotFoundException("Review could not be found");
            var review  = await query.SingleOrDefaultAsync();
            if (review.UserId != UserId)
                throw new ValidationException("User does not have access to edit this review");
            review.Text = Text;
            var ratingCount = (await _review.Query().Where(x => x.FilmId == review.FilmId && x.Rating != null).ToListAsync()).Count;
            var film = await _filmRepository.Query().SingleOrDefaultAsync(x => x.Id == review.FilmId);
            if (film is null)
                throw new NotFoundException("Film id is incorrect");
            if (review.Rating is null && Rating is not null)
            {
                film.Rating = ((double)Rating.Value + film.Rating) / (ratingCount + 1);
                review.Rating = Rating;
                await _unitOfWork.SaveChangesAsync();
                return;
            }
            if (review.Rating is not null && Rating is null)
                throw new AppException("Cannot delete rating");
            if (Rating is not null)
            {
                film.Rating += (double)(Rating - review.Rating).Value / ratingCount;
                review.Rating = Rating;
                await _unitOfWork.SaveChangesAsync();
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task Delete(int reviewId, int userId)
        {
            var query = _review.Query().Where(x => x.Id == reviewId);
            if (!await query.AnyAsync())
                throw new NotFoundException("Review could not be found");
            var review = await query.SingleOrDefaultAsync();
            if (review.UserId != userId)
                throw new ValidationException("User does not have access to delete this review");
            var film = await _filmRepository.Query().SingleOrDefaultAsync(x => x.Id == review.FilmId);
            var ratingCount = (await _review.Query().Where(x => x.FilmId == review.FilmId && x.Rating != null).ToListAsync()).Count;
            if (review.Rating is not null)
            {
                if (ratingCount == 1)
                    film.Rating = 0;
                else
                    film.Rating = (double)(film.Rating * ratingCount - review.Rating.Value) / (ratingCount - 1);
            }
            _review.Remove(review);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<IEnumerable<Review?>> GetReviews(int FilmId)
        {
            return await _review.Query().Where(x => x.FilmId == FilmId).ToListAsync();
        }
        public async Task<Review?> GetReview(int reviewId)
        {
            return await _review.Query().Where(x => x.Id == reviewId).SingleOrDefaultAsync();
        }
    }
}
