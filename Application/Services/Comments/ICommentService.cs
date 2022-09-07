using Imdb.Infrastructure.Entities;

namespace Application.Services.Comments
{
    public interface ICommentService
    {
        Task MakeComment(int ReviewId, int UserId, string Message);
        Task DeleteComment(int CommentId, int UserId);
        Task EditComment(int CommentId, int UserId, string Message);
        Task<IEnumerable<Comment?>> GetComments(int UserId);
        Task<Comment?> GetCommentById(int commentId);
        Task<Comment?> GetCommentByReview(int reviewId);

    }
}
