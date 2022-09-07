using Imdb.Infrastructure.Entities;
using Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Exceptions;

namespace Application.Services.Comments
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment> _commentRepo;
        private readonly IRepository<Review> _reviewRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IRepository<Comment> commentRepo, IRepository<Review> reviewRepo, IUnitOfWork unitOfWork)
        {
            _commentRepo = commentRepo;
            _reviewRepo = reviewRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task DeleteComment(int CommentId, int UserId)
        {
            var comment = await _commentRepo.GetSingleOrDefaultAsync(CommentId);
            if (comment is null)
                throw new NotFoundException("Cannot delete comment. Comment does not exist");
            if (comment.UserId != UserId)
                throw new ValidationException("Cannot delete a comment that does not belong to you");
            _commentRepo.Remove(comment);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task EditComment(int CommentId, int UserId, string Message)
        {
            var comment = await _commentRepo.GetSingleOrDefaultAsync(CommentId);
            if (comment is null)
                throw new NotFoundException("Cannot edit comment. Comment does not exist");
            if (comment.UserId != UserId)
                throw new ValidationException("Cannot edit a comment that does not belong to you");
            comment.Message = Message;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task MakeComment(int ReviewId, int UserId, string Message)
        {
            if (await _reviewRepo.GetSingleOrDefaultAsync(ReviewId) is null)
                throw new NotFoundException("Review does not exist");
            _commentRepo.Add(new Comment()
            {
                ReviewId = ReviewId,
                UserId = UserId,
                Message = Message
            });
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<IEnumerable<Comment?>> GetComments(int UserId)
        {
            return await _commentRepo.Query().Where(x => x.UserId == UserId).ToListAsync();
        }
        public async Task<Comment?> GetCommentByReview(int reviewId)
        {
            return await _commentRepo.Query().Where(x => x.ReviewId == reviewId).SingleOrDefaultAsync();
        }
        public async Task<Comment?> GetCommentById(int commentId)
        {
            return await _commentRepo.Query().Where(x => x.Id == commentId).SingleOrDefaultAsync();
        }
        public async Task<Comment?> GetComment(int commentId)
        {
            return await _commentRepo.Query().Where(x => x.Id == commentId).SingleOrDefaultAsync();
        }

    }
}
