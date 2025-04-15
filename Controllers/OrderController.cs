using Caker.Models;
using Caker.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController(OrderRepository repository) : BaseController<Order>(repository)
    {
        // GET api/orders/customer/{customer_id}
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetByCustomer(int customerId)
        {
            try
            {
                var orders = await repository.GetByCustomer(customerId);
                if (orders == null || !orders.Any())
                {
                    return NotFound();
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/orders/confectioner/{confectioner_id}
        [HttpGet("confectioner/{confectionerId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetByConfectioner(int confectionerId)
        {
            try
            {
                var orders = await repository.GetByConfectioner(confectionerId);
                if (orders == null || !orders.Any())
                {
                    return NotFound();
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
