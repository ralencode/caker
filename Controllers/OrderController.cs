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
        CakeRepository cakeRepository,
        ICurrentUserService currentUserService
    )
        : BaseController<Order, OrderResponse, CreateOrderFullRequest, UpdateOrderFullRequest>(
            repository,
            currentUserService
        )
    {
        private readonly OrderRepository _repo = repository;
        private readonly ConfectionerRepository _confectionerRepo = confectionerRepository;
        private readonly CakeRepository _cakeRepo = cakeRepository;
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
                return Unauthorized();
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
                return Unauthorized();
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
            var cake = await _cakeRepo.GetById(request.CakeId, tracking: true);
            if (cake == null)
                return NotFound("Cake not found");

            var order = new Order
            {
                CustomerId = id,
                CakeId = request.CakeId,
                Price = request.Price,
                Quantity = request.Quantity,
                OrderStatus = cake.IsCustom
                    ? OrderStatusType.PENDING_APPROVAL
                    : OrderStatusType.PENDING_PAYMENT,
                CreationDate = DateTime.UtcNow,
            };

            if (!CanCreate(order))
                return Forbid();

            await _repo.Create(order);

            // Fetch the order with included Cake and Confectioner
            var createdOrder = await _repo.GetById(order.Id, tracking: true);
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
                return Unauthorized();
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
                return Unauthorized();
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
                return Unauthorized();
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
                return Unauthorized();
            }
            var id_n = user?.Customer?.Id;
            if (id_n is null)
                return Forbid();

            int id = (int)id_n;

            return await GetByCustomer(id);
        }

        [HttpPost("batchpay")]
        public async Task<ActionResult<ICollection<BatchPaymentResponse>>> BatchPay(
            [FromBody] BatchPaymentRequest requests
        )
        {
            var results = new List<BatchPaymentResponse>();
            PaymentRequest paymentRequest = new(
                requests.CardNumber,
                requests.ExpirationDate,
                requests.Cvc
            );

            foreach (CreateOrderRequest orderRequest in requests.Orders)
            {
                var resultId = -1;
                OrderResponse? resultOrder = null;
                OrderStatusType? resultStatus = null;
                var resultError = "";

                var creationResult = await Create(orderRequest);

                if (creationResult.Result is CreatedAtActionResult created)
                {
                    if (
                        created.RouteValues?.TryGetValue("id", out var idObj) == true
                        && idObj is int orderId
                    )
                    {
                        resultId = orderId;
                        var payResult = await Pay(orderId, paymentRequest);

                        if (payResult.Result is OkObjectResult okResult)
                        {
                            if (okResult.Value is OrderResponse response)
                            {
                                resultOrder = response;
                                resultStatus = response.Status;
                            }
                        }
                        else if (payResult.Result is ObjectResult problem)
                        {
                            resultError = problem.Value?.ToString();
                        }
                    }
                }
                else if (creationResult.Result is ObjectResult errorResult)
                {
                    resultError = errorResult.Value?.ToString();
                }

                results.Add(
                    new BatchPaymentResponse(resultId, resultOrder, resultStatus, resultError)
                );
            }

            return Ok(results);
        }

        [HttpPost("{orderId}/pay")]
        public async Task<ActionResult<OrderResponse>> Pay(
            int orderId,
            [FromBody] PaymentRequest request
        )
        {
            User? user_nullable;
            try
            {
                user_nullable = await _currUserService.GetUser();
                if (user_nullable == null)
                    return Unauthorized("Not logged in (curent user is null)");
            }
            catch
            {
                return Unauthorized("Not logged in (curent user is null)");
            }

            User user = user_nullable;

            var order = await _repo.GetById(orderId, tracking: true);
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
            User? user_nullable;
            try
            {
                user_nullable = await _currUserService.GetUser();
                if (user_nullable == null)
                    return Unauthorized("Not logged in (curent user is null)");
            }
            catch
            {
                return Unauthorized("Not logged in (curent user is null)");
            }

            User user = user_nullable;

            var order = await _repo.GetById(orderId);
            if (order == null)
                return NotFound();

            if (order.CustomerId != user.Customer?.Id)
                return Forbid(
                    "Current customer {id} does not own that order, owned by other customer {orderid}",
                    user.Customer?.Id.ToString() ?? "",
                    order.CustomerId.ToString()
                );

            if (order.OrderStatus != OrderStatusType.DONE)
                return BadRequest("Order is not done.");

            order.OrderStatus = OrderStatusType.RECEIVED;

            await _repo.Update(order);

            var confectioner = order.Cake?.Confectioner;
            if (confectioner == null)
                return NotFound("Confectioner not found.");

            if (order.Cake?.IsCustom ?? false)
            {
                order.Cake.Price = (int)(order.Price / order.Quantity);
                await _cakeRepo.Update(order.Cake);
            }
            else if (order.Cake?.Price * order.Quantity != order.Price)
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

        [HttpPut("{orderId}/status")]
        public async Task<ActionResult<OrderResponse>> ChangeStatus(
            int orderId,
            [FromBody] ChangeStatusRequest request
        )
        {
            User? user_nullable;
            try
            {
                user_nullable = await _currUserService.GetUser();
                if (user_nullable == null)
                    return Unauthorized("Not logged in (curent user is null)");
            }
            catch
            {
                return Unauthorized("Not logged in (curent user is null)");
            }

            User user = user_nullable;

            var order = await _repo.GetById(orderId);
            if (order == null)
                return NotFound();

            if (order.CustomerId == user.Customer?.Id)
            {
                IEnumerable<string> allowedStatuses =
                [
                    "pending_approval",
                    "pendingapproval",
                    "in_progress",
                    "inprogress",
                    "received",
                    "recieved",
                    "canceled",
                    "cancelled",
                ];
                if (allowedStatuses.Contains(request.Status.ToLower()))
                {
                    order.OrderStatus = request.Status switch
                    {
                        "pending_approval" or "pendingapproval" => OrderStatusType.PENDING_APPROVAL,
                        "in_progress" or "inprogress" => OrderStatusType.IN_PROGRESS,
                        "recieved" or "received" => OrderStatusType.RECEIVED,
                        "canceled" or "cancelled" => OrderStatusType.CANCELED,
                        _ => order.OrderStatus,
                    };
                }
                else
                    return Forbid();
            }
            else if (order.Cake?.ConfectionerId == user.Confectioner?.Id)
            {
                IEnumerable<string> allowedStatuses =
                [
                    "pending_approval",
                    "pendingapproval",
                    "in_progress",
                    "inprogress",
                    "pending_payment",
                    "pendingpayment",
                    "done",
                    "rejected",
                ];
                if (allowedStatuses.Contains(request.Status.ToLower()))
                {
                    order.OrderStatus = request.Status switch
                    {
                        "pending_approval" or "pendingapproval" => OrderStatusType.PENDING_APPROVAL,
                        "in_progress" or "inprogress" => OrderStatusType.IN_PROGRESS,
                        "pending_payment" or "pendingpayment" => OrderStatusType.PENDING_PAYMENT,
                        "done" => OrderStatusType.DONE,
                        "rejected" => OrderStatusType.REJECTED,
                        _ => order.OrderStatus,
                    };
                }
                else
                    return Forbid();
            }
            else
                return Forbid();

            await _repo.Update(order);

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
            };
    }
}
