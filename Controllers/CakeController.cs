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
                return Unauthorized();
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
                return Unauthorized();
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
                Name = request.name,
                Description = request.description,
                Diameter = request.diameter,
                Weight = request.weight,
                ReqTime = request.required_time,
                Price = request.price,
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
        [HttpPost("custom")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<CakeResponse>> CreateCustom(
            [FromForm] CreateCustomCakeRequest request
        )
        {
            var imagePath =
                (request.Image is null)
                    ? ""
                    : await _imageService.SaveImageAsync(request.Image, request.confectioner_id);

            var cake = new Cake
            {
                ConfectionerId = request.confectioner_id,
                Diameter = request.diameter,
                ReqTime = request.required_time,
                Color = request.color,
                Text = request.text,
                ImagePath =
                    imagePath.Length > "assets/".Length ? imagePath : request.image_path ?? "",
                IsCustom = true,
                TextX = request.text_x,
                TextY = request.text_y,
                TextSize = request.text_size,
                Fillings = request.fillings,
                Visible = false,
                Name = request.name ?? "Custom cake",
                Description = request.description,
                Price = request.price ?? 0,
                ImageScale = request.image_scale,
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
                return Unauthorized();
            }
            var id_n = user?.Confectioner?.Id;
            if (id_n is null)
                return Forbid();

            int id = (int)id_n;

            return await GetByConfectioner(id);
        }

        [HttpGet("sorted/weight")]
        public async Task<ActionResult<IEnumerable<CakeResponse>>> GetSortedByWeight(
            [FromQuery] bool ascending = true
        )
        {
            var cakes = await _repo.GetSortedNonCustomByWeight(ascending);
            return Ok(cakes.Select(c => c.ToDto()));
        }

        [HttpGet("sorted/price")]
        public async Task<ActionResult<IEnumerable<CakeResponse>>> GetSortedByPrice(
            [FromQuery] bool ascending = true
        )
        {
            var cakes = await _repo.GetSortedNonCustomByPrice(ascending);
            return Ok(cakes.Select(c => c.ToDto()));
        }

        [HttpPost("sorted/weight")]
        public async Task<ActionResult<IEnumerable<CakeResponse>>> SearchSortedByWeight(
            [FromBody] SearchQuery query,
            [FromQuery] bool ascending = true
        )
        {
            var cakes = await _repo.SearchSortedNonCustomByWeight(ascending, query.Name);
            return Ok(cakes.Select(c => c.ToDto()));
        }

        [HttpPost("sorted/price")]
        public async Task<ActionResult<IEnumerable<CakeResponse>>> SearchSortedByPrice(
            [FromBody] SearchQuery query,
            [FromQuery] bool ascending = true
        )
        {
            var cakes = await _repo.SearchSortedNonCustomByPrice(ascending, query.Name);
            return Ok(cakes.Select(c => c.ToDto()));
        }

        [HttpPost("search/name")]
        public async Task<ActionResult<IEnumerable<CakeResponse>>> GetByName(
            [FromBody] SearchQuery query
        )
        {
            var cakes = await _repo.SearchByName(query.Name);
            return Ok(cakes.Select(c => c.ToDto()));
        }

        [HttpPatch("{id}/image")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<CakeResponse>> UpdateImage(int id, IFormFile Image)
        {
            var cake = await _repo.GetById(id);
            if (cake == null)
                return NotFound();

            if (!CanUpdate(cake))
                return Forbid();

            cake.ImagePath = await _imageService.SaveImageAsync(Image, cake.ConfectionerId);
            await _repo.Update(cake);
            return Ok(cake.ToDto());
        }

        protected override Cake CreateModel(CreateCustomCakeRequest dto) =>
            new()
            {
                Name = dto.name ?? "Custom cake",
                ConfectionerId = dto.confectioner_id,
                Visible = dto.name is not null,
                Description = dto.description,
                Price = dto.price ?? 0,
                Color = dto.color,
                Diameter = dto.diameter,
                Fillings = dto.fillings,
                Text = dto.text,
                TextSize = dto.text_size,
                TextX = dto.text_x,
                TextY = dto.text_y,
                ImageScale = dto.image_scale,
                IsCustom = true,
                ImagePath = dto.image_path ?? "",
            };

        protected override void UpdateModel(Cake model, UpdateCustomCakeRequest dto)
        {
            if (dto.name is not null)
                model.Name = dto.name;
            if (dto.confectioner_id.HasValue)
                model.ConfectionerId = dto.confectioner_id.Value;
            if (dto.description is not null)
                model.Description = dto.description;
            if (dto.price.HasValue)
                model.Price = dto.price.Value;
            if (dto.color is not null)
                model.Color = dto.color;
            if (dto.diameter.HasValue)
                model.Diameter = dto.diameter.Value;
            if (dto.fillings is not null)
                model.Fillings = dto.fillings;
            if (dto.text is not null)
                model.Text = dto.text;
            if (dto.text_size.HasValue)
                model.TextSize = dto.text_size.Value;
            if (dto.text_x.HasValue)
                model.TextX = dto.text_x.Value;
            if (dto.text_y.HasValue)
                model.TextY = dto.text_y.Value;
            if (dto.image_scale.HasValue)
                model.TextY = dto.image_scale.Value;
            if (dto.image_path is not null && dto.image_path.Length > 1)
                model.ImagePath = dto.image_path;
        }
    }
}
