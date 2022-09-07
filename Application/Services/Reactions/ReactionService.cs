using Imdb.Infrastructure.Entities;
using Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Exceptions;

namespace Application.Services.Reactions
{
    public class ReactionService : IReactionService
    {
        private readonly IRepository<Reaction> _reactionRepo;
        private readonly IRepository<Review> _reviewRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ReactionService(IRepository<Reaction> reactionRepo, IRepository<Review> reviewRepo, IUnitOfWork unitOfWork)
        {
            _reactionRepo = reactionRepo;
            _reviewRepo = reviewRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task Like(int ReviewId, int UserId)
        {
            var reaction = await _reactionRepo.Query().Where(x => x.ReviewId == ReviewId && x.UserId == UserId).SingleOrDefaultAsync();
            Review? review = await _reviewRepo.GetSingleOrDefaultAsync(ReviewId);
            if (review is null)
            {
                throw new NotFoundException("Review id is incorrect");
            }
            if (reaction is not null)
            {
                review.Like -= 1;
                _reactionRepo.Remove(reaction);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                review.Like += 1;
                _reactionRepo.Add(new Reaction()
                {
                    ReviewId = ReviewId,
                    UserId = UserId
                });
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
