using System.Text.Json;
using Caker.Dto;
using Caker.Models;
using Caker.Repositories;
using Caker.Services.CurrentUserService;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController(
        CustomerRepository repository,
        ICurrentUserService currentUserService
    ) : BaseController<Customer, CustomerResponse, object, object>(repository, currentUserService)
    {
        // Disable unsupported operations
        [HttpPost]
        public override Task<ActionResult<CustomerResponse>> Create([FromBody] object dto) =>
            throw new NotImplementedException();

        [HttpPut("{id}")]
        public override Task<ActionResult<CustomerResponse>> Update(
            int id,
            [FromBody] object dto
        ) => throw new NotImplementedException();

        [HttpPatch("{id}")]
        public override Task<ActionResult<CustomerResponse>> PartialUpdate(
            int id,
            [FromBody] JsonElement patchDoc
        ) => throw new NotImplementedException();

        // Custom implementation
        protected override Customer CreateModel(object dto) => throw new NotImplementedException();

        protected override void UpdateModel(Customer model, object dto) =>
            throw new NotImplementedException();
    }
}
