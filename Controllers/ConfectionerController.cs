using Caker.Models;
using Caker.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/confectioners")]
    public class ConfectionerController(ConfectionerRepository repository)
        : BaseController<Confectioner>(repository) { }
}
