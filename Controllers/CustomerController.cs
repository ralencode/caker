using System.Text.Json;
using Caker.Dto;
using Caker.Models;
using Caker.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController(CustomerRepository repository)
        : BaseController<Customer, CustomerResponse, object, object>(repository)
    {
        // Disable unsupported operations
        [HttpPost]
        public override Task<ActionResult<CustomerResponse>> Create([FromBody] object dto) =>
            throw new NotImplementedException();

        [HttpPut("{id}")]
        public override Task<IActionResult> Update(int id, [FromBody] object dto) =>
            throw new NotImplementedException();

        [HttpPatch("{id}")]
        public override Task<IActionResult> PartialUpdate(
            int id,
            [FromBody] JsonElement patchDoc
        ) => throw new NotImplementedException();

        // Custom implementation
        protected override Customer CreateModel(object dto) => throw new NotImplementedException();

        protected override void UpdateModel(Customer model, object dto) =>
            throw new NotImplementedException();

        protected override CustomerResponse ToDto(Customer model) => new(model.Id, model.UserId);
    }
}
