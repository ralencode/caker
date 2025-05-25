using Caker.Dto;
using Caker.Models;
using Caker.Repositories;
using Caker.Services.CurrentUserService;
using Caker.Services.ImageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/cakes")]
    public class CakeController(
        CakeRepository repository,
        IImageService imageService,
        ICurrentUserService currentUserService
    )
        : BaseController<Cake, CakeResponse, CreateCustomCakeRequest, UpdateCustomCakeRequest>(
            repository,
            currentUserService
        )
    {
        private readonly CakeRepository _repo = repository;
        private readonly IImageService _imageService = imageService;
        private readonly ICurrentUserService _currUserService = currentUserService;

        /// <summary>
        /// Create cake for current confectioner by token from cookies.
        /// </summary>
        [HttpPost("regular/self")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<CakeResponse>> CreateRegular(
            [FromForm] CreateRegularCakeRequest request
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
            var id_n = user?.Confectioner?.Id;
            if (id_n is null)
                return Forbid();

            int id = (int)id_n;

            return await CreateRegularById(request, id);
        }

        /// <summary>
        /// Create cake for confectioner by id
        /// </summary>
        [HttpPost("regular/{confectionerId}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<CakeResponse>> CreateRegular(
            [FromForm] CreateRegularCakeRequest request,
            int confectionerId
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
            var id_n = user?.Confectioner?.Id;
            if (id_n is null)
                return Forbid();

            int id = (int)id_n;

            if (user?.Type != UserType.ADMIN && id != confectionerId)
                return Forbid();

            return await CreateRegularById(request, confectionerId);
        }

        private async Task<ActionResult<CakeResponse>> CreateRegularById(
            CreateRegularCakeRequest request,
            int id
        )
        {
            var imagePath =
                (request.Image is null)
                    ? ""
                    : await _imageService.SaveImageAsync(request.Image, id);

            var cake = new Cake
            {
                ConfectionerId = id,
                Name = request.Name,
                Description = request.Description,
                Diameter = request.Diameter,
                Weight = request.Weight,
                ReqTime = request.ReqTime,
                Price = request.Price,
                ImagePath = imagePath,
                IsCustom = false,
                Visible = true,
            };

            await _repo.Create(cake);
            var createdCake = await _repo.GetById(cake.Id); // Reload to include Confectioner
            return CreatedAtAction(
                nameof(_repo.GetById),
                new { id = cake.Id },
                createdCake?.ToDto()
            );
        }

        [Authorize]
        [HttpPost("custom/self")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<CakeResponse>> CreateCustom(
            [FromForm] CreateCustomCakeRequest request
        )
        {
            var imagePath =
                (request.Image is null)
                    ? ""
                    : await _imageService.SaveImageAsync(request.Image, request.ConfectionerId);

            var cake = new Cake
            {
                ConfectionerId = request.ConfectionerId,
                Diameter = request.Diameter,
                ReqTime = request.ReqTime,
                Color = request.Color,
                Text = request.Text,
                ImagePath = imagePath,
                IsCustom = true,
                TextX = request.TextX,
                TextY = request.TextY,
                TextSize = request.TextSize,
                Fillings = request.Fillings,
                Visible = false,
                Name = request.Name ?? "Custom cake",
                Description = request.Description,
                Price = request.Price ?? 0,
            };

            await _repo.Create(cake);
            var createdCake = await _repo.GetById(cake.Id); // Reloads with Confectioner included
            return CreatedAtAction(
                nameof(_repo.GetById),
                new { id = cake.Id },
                createdCake?.ToDto()
            );
        }

        [HttpGet("confectioner/{confectionerId}")]
        public async Task<ActionResult<IEnumerable<CakeResponse>>> GetByConfectioner(
            int confectionerId
        )
        {
            var cakes = await _repo.GetByConfectioner(confectionerId);

            if (cakes == null || !cakes.Any())
            {
                return NotFound(new { message = "No cakes found for this confectioner." });
            }

            return Ok(cakes.Select(c => c.ToDto()));
        }

        [HttpGet("confectioner/self")]
        public async Task<ActionResult<IEnumerable<CakeResponse>>> GetByConfectioner()
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

        protected override Cake CreateModel(CreateCustomCakeRequest dto) =>
            new()
            {
                Name = dto.Name ?? "Custom cake",
                ConfectionerId = dto.ConfectionerId,
                Visible = dto.Name is not null,
                Description = dto.Description,
                Price = dto.Price ?? 0,
                Color = dto.Color,
                Diameter = dto.Diameter,
                Fillings = dto.Fillings,
                Text = dto.Text,
                TextSize = dto.TextSize,
                TextX = dto.TextX,
                TextY = dto.TextY,
            };

        protected override void UpdateModel(Cake model, UpdateCustomCakeRequest dto)
        {
            if (dto.Name is not null)
                model.Name = dto.Name;
            if (dto.ConfectionerId.HasValue)
                model.ConfectionerId = dto.ConfectionerId.Value;
            if (dto.Description is not null)
                model.Description = dto.Description;
            if (dto.Price.HasValue)
                model.Price = dto.Price.Value;
            if (dto.Color is not null)
                model.Color = dto.Color;
            if (dto.Diameter.HasValue)
                model.Diameter = dto.Diameter.Value;
            if (dto.Fillings is not null)
                model.Fillings = dto.Fillings;
            if (dto.Text is not null)
                model.Text = dto.Text;
            if (dto.TextSize.HasValue)
                model.TextSize = dto.TextSize.Value;
            if (dto.TextX.HasValue)
                model.TextX = dto.TextX.Value;
            if (dto.TextY.HasValue)
                model.TextY = dto.TextY.Value;
        }
    }
}
