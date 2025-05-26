using Caker.Dto;
using Caker.Models;
using Caker.Repositories;
using Caker.Services.CurrentUserService;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController(
        OrderRepository repository,
        ConfectionerRepository confectionerRepository,
        ICurrentUserService currentUserService
    )
        : BaseController<Order, OrderResponse, CreateOrderFullRequest, UpdateOrderFullRequest>(
            repository,
            currentUserService
        )
    {
        private readonly OrderRepository _repo = repository;
        private readonly ConfectionerRepository _confectionerRepo = confectionerRepository;
        private readonly ICurrentUserService _currUserService = currentUserService;

        /// <summary>
        /// Create order from current customer by token from cookies.
        /// </summary>
        [HttpPost("self")]
        public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderRequest request)
        {
            User? user;
            try
            {
                user = await _currUserService.GetUser();
            }
            catch
            {
                return Forbid();
            }
            var id_n = user?.Customer?.Id;
            if (id_n is null)
                return Forbid();

            int id = (int)id_n;

            return await CreateById(request, id);
        }

        /// <summary>
        /// Create order from customer by id
        /// </summary>
        [HttpPost("{customerId}")]
        public async Task<ActionResult<OrderResponse>> Create(
            [FromBody] CreateOrderRequest request,
            int customerId
        )
        {
            User? user;
            try
            {
                user = await _currUserService.GetUser();
            }
            catch
            {
                return Forbid();
            }
            var id_n = user?.Customer?.Id;
            if (id_n is null)
                return Forbid();

            int id = (int)id_n;

            if (user?.Type != UserType.ADMIN && id != customerId)
                return Forbid();

            return await CreateById(request, customerId);
        }

        private async Task<ActionResult<OrderResponse>> CreateById(
            CreateOrderRequest request,
            int id
        )
        {
            var order = new Order
            {
                CustomerId = id,
                CakeId = request.CakeId,
                Price = request.Price,
                Quantity = request.Quantity,
                OrderStatus = OrderStatusType.PENDING_APPROVAL,
                CreationDate = DateTime.UtcNow,
            };

            if (!CanCreate(order))
                return Forbid();

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

        /// <summary>
        /// Create full order from current customer by token from cookies.
        /// </summary>
        [HttpPost("full/self")]
        public override async Task<ActionResult<OrderResponse>> Create(
            [FromBody] CreateOrderFullRequest request
        )
        {
            User? user;
            try
            {
                user = await _currUserService.GetUser();
            }
            catch
            {
                return Forbid();
            }
            var id_n = user?.Customer?.Id;
            if (id_n is null)
                return Forbid();

            int id = (int)id_n;

            return await CreateFullById(request, id);
        }

        /// <summary>
        /// Create full order from customer by id
        /// </summary>
        [HttpPost("full/{customerId}")]
        public async Task<ActionResult<OrderResponse>> Create(
            [FromBody] CreateOrderFullRequest request,
            int customerId
        )
        {
            User? user;
            try
            {
                user = await _currUserService.GetUser();
            }
            catch
            {
                return Forbid();
            }
            var id_n = user?.Customer?.Id;
            if (id_n is null)
                return Forbid();

            int id = (int)id_n;

            if (user?.Type != UserType.ADMIN && id != customerId)
                return Forbid();

            return await CreateFullById(request, customerId);
        }

        private async Task<ActionResult<OrderResponse>> CreateFullById(
            CreateOrderFullRequest dto,
            int customerId
        )
        {
            var order = new Order
            {
                CustomerId = customerId,
                CakeId = dto.CakeId,
                Price = dto.Price,
                Quantity = dto.Quantity,
                OrderStatus = dto.OrderStatus,
                IsCustom = dto.IsCustom,
                CreationDate = DateTime.UtcNow,
            };

            await _repo.Create(order);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order.ToDto());
        }

        [HttpGet("confectioner/{confectionerId}")]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetByConfectioner(
            int confectionerId
        )
        {
            var orders = await _repo.GetByConfectioner(confectionerId);

            if (orders == null || !orders.Any())
            {
                return NotFound(new { message = "No orders found for this confectioner." });
            }

            return Ok(orders.Select(o => o.ToDto()));
        }

        [HttpGet("confectioner/self")]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetByConfectioner()
        {
            User? user;
            try
            {
                user = await _currUserService.GetUser();
            }
            catch
            {
                return Forbid();
            }
            var id_n = user?.Confectioner?.Id;
            if (id_n is null)
                return Forbid();

            int id = (int)id_n;

            return await GetByConfectioner(id);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetByCustomer(int customerId)
        {
            var orders = await _repo.GetByCustomer(customerId);

            if (orders == null || !orders.Any())
            {
                return NotFound(new { message = "No orders found for this customer." });
            }

            return Ok(orders.Select(o => o.ToDto()));
        }

        [HttpGet("customer/self")]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetByCustomer()
        {
            User? user;
            try
            {
                user = await _currUserService.GetUser();
            }
            catch
            {
                return Forbid();
            }
            var id_n = user?.Customer?.Id;
            if (id_n is null)
                return Forbid();

            int id = (int)id_n;

            return await GetByCustomer(id);
        }

        [HttpPost("batchpay")]
        public async Task<ICollection<ActionResult<OrderResponse>>> BatchPay(
            [FromBody] BatchPaymentRequest reqeusts
        )
        {
            ICollection<ActionResult<OrderResponse>> creationResults = [];
            foreach (CreateOrderRequest orderRequest in reqeusts.Orders)
            {
                creationResults.Add(await Create(orderRequest));
            }

            ICollection<ActionResult<OrderResponse>> paymentResults = [];
            PaymentRequest paymentRequest = new(
                reqeusts.CardNumber,
                reqeusts.ExpirationDate,
                reqeusts.Cvc
            );

            foreach (ActionResult<OrderResponse> result in creationResults)
            {
                OrderResponse? response = result?.Value;
                if (response == null)
                {
                    paymentResults.Add(result ?? NotFound());
                    continue;
                }
                paymentResults.Add(await Pay(response.Id, paymentRequest));
            }
            return paymentResults;
        }

        [HttpPost("{orderId}/pay")]
        public async Task<ActionResult<OrderResponse>> Pay(
            int orderId,
            [FromBody] PaymentRequest request
        )
        {
            var user = await _currUserService.GetUser();
            if (user == null)
                return Forbid("Not logged in (curent user is null)");

            var order = await _repo.GetById(orderId);
            if (order == null)
                return NotFound();

            if (order.CustomerId != user.Customer?.Id)
                return Forbid(
                    "Current customer {id} does not own that order, owned by other customer {orderid}",
                    user.Customer?.Id.ToString() ?? "",
                    order.CustomerId.ToString()
                );

            if (order.OrderStatus != OrderStatusType.PENDING_PAYMENT)
            {
                return BadRequest("Order is not in a state that allows payment.");
            }

            if (order.Cake?.Price * order.Quantity != order.Price)
            {
                return BadRequest("The sum of cakes' prices is not equal to order's price.");
            }

            order.OrderStatus = OrderStatusType.IN_PROGRESS;

            await _repo.Update(order);

            var confectioner = order.Cake?.Confectioner;
            if (confectioner == null)
                return NotFound("Confectioner not found.");

            int amount = (int)order.Price;
            confectioner.BalanceFreezed += amount;

            await _confectionerRepo.Update(confectioner);

            return Ok(order.ToDto());
        }

        [HttpPost("{orderId}/receive")]
        public async Task<ActionResult<OrderResponse>> Receive(int orderId)
        {
            var user = await _currUserService.GetUser();
            if (user == null)
                return Forbid("Not logged in (curent user is null)");

            var order = await _repo.GetById(orderId);
            if (order == null)
                return NotFound();

            if (order.CustomerId != user.Customer?.Id)
                return Forbid(
                    "Current customer {id} does not own that order, owned by other customer {orderid}",
                    user.Customer?.Id.ToString() ?? "",
                    order.CustomerId.ToString()
                );

            if (order.OrderStatus != OrderStatusType.IN_PROGRESS)
                return BadRequest("Order is not in progress.");

            order.OrderStatus = OrderStatusType.RECEIVED;

            await _repo.Update(order);

            var confectioner = order.Cake?.Confectioner;
            if (confectioner == null)
                return NotFound("Confectioner not found.");

            if (order.Cake?.Price * order.Quantity != order.Price)
            {
                return BadRequest("The sum of cakes' prices is not equal to order's price.");
            }

            int amount = (int)order.Price;
            if (confectioner.BalanceFreezed < amount)
                return StatusCode(500, "Insufficient freezed balance. Contact support.");

            confectioner.BalanceFreezed -= amount;
            confectioner.BalanceAvailable += amount;

            await _confectionerRepo.Update(confectioner);

            return Ok(order.ToDto());
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
