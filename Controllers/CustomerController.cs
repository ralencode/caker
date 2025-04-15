using Caker.Models;
using Caker.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController(CustomerRepository repository)
        : BaseController<Customer>(repository) { }
}
