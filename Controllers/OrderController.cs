using Caker.Dto;
using Caker.Models;
using Caker.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController(OrderRepository repo)
        : BaseController<Order, OrderResponse, CreateOrderFullRequest, UpdateOrderFullRequest>(repo)
    {
        readonly OrderRepository _repo = repo;

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderRequest request)
        {
            var order = new Order
            {
                CustomerId = request.CustomerId,
                CakeId = request.CakeId,
                Price = request.Price,
                Quantity = request.Quantity,
                OrderStatus = OrderStatusType.PENDING_APPROVAL,
                CreationDate = DateTime.UtcNow,
            };

            await _repo.Create(order);

            // Fetch the order with included Cake and Confectioner
            var createdOrder = await _repo.GetById(order.Id);
            if (createdOrder == null)
            {
                return NotFound();
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdOrder.Id },
                createdOrder.ToDto()
            );
        }

        [HttpPost("full")]
        public override async Task<ActionResult<OrderResponse>> Create(
            [FromBody] CreateOrderFullRequest dto
        )
        {
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                CakeId = dto.CakeId,
                Price = dto.Price,
                Quantity = dto.Quantity,
                OrderStatus = dto.OrderStatus,
                IsCustom = dto.IsCustom,
                CreationDate = DateTime.UtcNow,
            };

            await _repository.Create(order);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order.ToDto());
        }

        protected override void UpdateModel(Order model, UpdateOrderFullRequest dto)
        {
            if (dto.CustomerId.HasValue)
                model.CustomerId = dto.CustomerId.Value;
            if (dto.CakeId.HasValue)
                model.CakeId = dto.CakeId.Value;
            if (dto.Price.HasValue)
                model.Price = dto.Price.Value;
            if (dto.Quantity.HasValue)
                model.Quantity = dto.Quantity.Value;
            if (dto.OrderStatus.HasValue)
                model.OrderStatus = dto.OrderStatus.Value;
            if (dto.IsCustom.HasValue)
                model.IsCustom = dto.IsCustom.Value;
            if (dto.CreatedAt.HasValue)
                model.CreationDate = dto.CreatedAt.Value;
        }

        protected override Order CreateModel(CreateOrderFullRequest dto) =>
            new()
            {
                CustomerId = dto.CustomerId,
                CakeId = dto.CakeId,
                Price = dto.Price,
                Quantity = dto.Quantity,
                OrderStatus = dto.OrderStatus,
                CreationDate = dto.CreatedAt,
                IsCustom = dto.IsCustom,
            };
    }
}
