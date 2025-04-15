using Caker.Models;
using Caker.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessageController(MessageRepository repository)
        : BaseController<Message>(repository)
    {
        // GET api/messages/{from_id}/{to_id}
        [HttpGet("{from_id}/{to_id}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetMessagesBetweenUsers(
            int from_id,
            int to_id
        )
        {
            try
            {
                var messages = await repository.GetMessagesBetweenUsers(from_id, to_id);
                if (messages == null || !messages.Any())
                {
                    return NotFound();
                }
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
