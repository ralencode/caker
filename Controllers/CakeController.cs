using Caker.Dto;
using Caker.Models;
using Caker.Repositories;
using Caker.Services.ImageService;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/cakes")]
    public class CakeController(CakeRepository repo, IImageService imageService)
        : BaseController<Cake, CakeResponse, CreateCustomCakeRequest, UpdateCustomCakeRequest>(repo)
    {
        private readonly CakeRepository _repo = repo;
        private readonly IImageService _imageService = imageService;

        [HttpPost("regular")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<CakeResponse>> CreateRegular(
            [FromForm] CreateRegularCakeRequest request
        )
        {
            var imagePath =
                (request.Image is null)
                    ? ""
                    : await _imageService.SaveImageAsync(request.Image, request.ConfectionerId);

            var cake = new Cake
            {
                ConfectionerId = request.ConfectionerId,
                Name = request.Name,
                Description = request.Description,
                Diameter = request.Diameter,
                Weight = request.Weight,
                ReqTime = TimeSpan.FromDays(request.ETADays),
                Price = request.Price,
                ImagePath = imagePath,
                IsCustom = false,
                Visible = true,
            };

            await _repo.Create(cake);
            return CreatedAtAction(nameof(_repo.GetById), new { id = cake.Id }, ToDto(cake));
        }

        [HttpPost("custom")]
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
                ReqTime = TimeSpan.FromDays(request.ETADays),
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
            return CreatedAtAction(nameof(_repo.GetById), new { id = cake.Id }, ToDto(cake));
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

            return Ok(cakes.Select(ToDto));
        }

        protected override CakeResponse ToDto(Cake cake)
        {
            return new(
                cake.Id!.Value,
                cake.Name,
                cake.Description,
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
            if (dto.Color.HasValue)
                model.Color = dto.Color.Value;
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
