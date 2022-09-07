using Application.Services.Reviews;
using Imdb.Infrastructure.Entities;
using Infrastructure.Repositories.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.ApplicationInfrastructure;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers
{

    [ApiController]
    [Route("")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewSerivce;
        private readonly ApplicationContext _context;
        public ReviewController(IReviewService reviewService, ApplicationContext context)
        {
            _reviewSerivce = reviewService;
            _context = context;
        }
        [Authorize]
        [HttpPost("films/{filmId}/reviews")]
        public async Task<IActionResult> Add(int filmId, ReviewDto review)
        {
            await _reviewSerivce.Add(review.Text, review.Rating, _context.UserId.Value, filmId);
            return Ok();
        }
        [Authorize]
        [HttpPut("reviews/{reviewId}")]
        public async Task<IActionResult> Edit(int reviewId, ReviewDto review)
        {
            await _reviewSerivce.Edit(review.Text, review.Rating, _context.UserId.Value, reviewId);
            return Ok();
        }
        [HttpDelete("reviews/{reviewId}")]
        public async Task<IActionResult> Delete(int reviewId)
        {
            await _reviewSerivce.Delete(reviewId, _context.UserId.Value);
            return Ok();
        }
        [HttpGet("films/{filmId}/reviews")]
        public async Task<ActionResult<Review?>> GetReviews(int filmId)
        {
            if (_context.UserId == null)
                throw new AppException("Not Authorized");
            return Ok(await _reviewSerivce.GetReviews(filmId));
        }
        [HttpGet("reviews/{reviewId}")]
        public async Task<ActionResult<Review?>> GetReview(int reviewId)
        {
            return Ok(await _reviewSerivce.GetReview(reviewId));
        }
    }
}
