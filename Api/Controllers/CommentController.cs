using Api.Factories;
using Application.Services.Comments;
using Application.Services.Reviews;
using Imdb.Infrastructure.Entities;
using Infrastructure.Repositories.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ApplicationInfrastructure;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers
{


    [ApiController]
    [Authorize]
    [Route("")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ApplicationContext _context;
        public CommentController(ICommentService commentService, ApplicationContext context)
        {
            _commentService = commentService;
            _context = context;
        }

        [HttpPost("reviews/{reviewId}/comments")]
        public async Task<IActionResult> MakeComment(int reviewId, string message)
        {
            await _commentService.MakeComment(reviewId, _context.UserId.Value, message);
            return Ok();
        }
        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            await _commentService.DeleteComment(commentId, _context.UserId.Value);
            return Ok();
        }
        [HttpPut("comments/{commentId}")]
        public async Task<IActionResult> EditComment(int commentId, EditCommentDto comment)
        {
            await _commentService.EditComment(commentId, _context.UserId.Value, comment.Message);
            return Ok();
        }
        [HttpGet("comments")]
        public async Task<IActionResult> GetComemnt()
        {
            return Ok(await _commentService.GetComments(_context.UserId.Value));
        }
        [HttpGet("reviews/{reviewId}/comments")]
        public async Task<IActionResult> GetComemntByReview(int reviewId)
        {
            return Ok(await _commentService.GetCommentByReview(reviewId));
        }
        [HttpGet("comments/{commentId}")]
        public async Task<IActionResult> GetComment(int commentId)
        {
            return Ok(await _commentService.GetCommentById(commentId));
        }
    }
}
