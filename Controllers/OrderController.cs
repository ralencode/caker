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
            return CreatedAtAction(nameof(_repo.GetById), new { id = order.Id }, ToDto(order));
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
                PaymentStatus = dto.PaymentStatus,
                Eta = dto.Eta,
                IsCustom = dto.IsCustom,
                CreationDate = DateTime.UtcNow,
            };

            await _repository.Create(order);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, ToDto(order));
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
            if (dto.PaymentStatus.HasValue)
                model.PaymentStatus = dto.PaymentStatus.Value;
            if (dto.Eta.HasValue)
                model.Eta = dto.Eta.Value;
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
                PaymentStatus = dto.PaymentStatus,
                Eta = dto.Eta,
                IsCustom = dto.IsCustom,
            };

        protected override OrderResponse ToDto(Order model) =>
            new(
                model.Id!.Value,
                model.CustomerId,
                model.Cake!.ConfectionerId,
                MapToCakeResponse(model.Cake),
                model.Price,
                model.OrderStatus,
                model.Quantity,
                model.CreationDate,
                model.IsCustom
            );

        private static CakeResponse MapToCakeResponse(Cake cake) =>
            new(
                cake.Id!.Value,
                cake.ConfectionerId,
                cake.Name,
                cake.Description,
                cake.Fillings,
                cake.ReqTime,
                cake.Color,
                $"assets/{cake.ImagePath}",
                cake.Price,
                cake.Diameter,
                cake.Weight,
                cake.Text,
                cake.TextSize,
                cake.TextX,
                cake.TextY,
                cake.IsCustom
            );
    }
}
