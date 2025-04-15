using Caker.Models;
using Caker.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/cakes")]
    public class CakeController(CakeRepository repository) : BaseController<Cake>(repository) { 
        // GET api/cakes/confectioner/{confectioner_id}
        [HttpGet("confectioner/{confectionerId}")]
        public async Task<ActionResult<IEnumerable<Cake>>> GetByConfectioner(int confectionerId)
        {
            try
            {
                var cakes = await repository.GetByConfectioner(confectionerId);
                if (cakes == null || !cakes.Any())
                {
                    return NotFound();
                }
                return Ok(cakes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
