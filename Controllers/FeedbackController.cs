using Caker.Models;
using Caker.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/feedback")]
    public class FeedbackController(FeedbackRepository repository)
        : BaseController<Feedback>(repository)
    {
        // GET api/feedback/confectioner/{confectioner_id}
        [HttpGet("confectioner/{confectionerId}")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetByConfectioner(int confectionerId)
        {
            try
            {
                var feedbacks = await repository.GetByConfectioner(confectionerId);
                if (feedbacks == null || !feedbacks.Any())
                {
                    return NotFound();
                }
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
