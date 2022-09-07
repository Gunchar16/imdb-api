using Application.Services.Reactions;
using Microsoft.AspNetCore.Mvc;
using Shared.ApplicationInfrastructure;

namespace Api.Controllers
{

    [ApiController]
    [Route("")]
    public class ReactionController : ControllerBase
    {
        private readonly IReactionService _reactionService;
        private readonly ApplicationContext _context;
        public ReactionController(IReactionService reactionService, ApplicationContext context)
        {
            _reactionService = reactionService;
            _context = context;
        }

        [HttpPost("reviews/{reviewId}/like")]
        public async Task<IActionResult> Like(int reviewId)
        {
            await _reactionService.Like(reviewId, _context.UserId.Value);
            return Ok();
        }
    }
}
