using Caker.Dto;
using Caker.Models;
using Caker.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/confectioners")]
    public class ConfectionerController(ConfectionerRepository repo)
        : BaseController<
            Confectioner,
            ConfectionerResponse,
            CreateConfectionerRequest,
            UpdateConfectionerRequest
        >(repo)
    {
        [HttpGet("{id}/settings")]
        public async Task<ActionResult<ConfectionerSettingsResponse>> GetSettings(int id)
        {
            var confectioner = await _repository.GetById(id);
            if (confectioner == null)
                return NotFound();

            return Ok(
                new ConfectionerSettingsResponse(
                    confectioner.Id,
                    confectioner.MinDiameter,
                    confectioner.MaxDiameter,
                    confectioner.MinEta,
                    confectioner.MaxEta,
                    confectioner.Fillings ?? [],
                    confectioner.DoImages,
                    confectioner.DoShapes
                )
            );
        }

        [HttpPut("{id}/settings")]
        public async Task<ActionResult<ConfectionerSettingsResponse>> UpdateSettings(
            int id,
            [FromBody] UpdateConfectionerSettingsRequest request
        )
        {
            var confectioner = await _repository.GetById(id);
            if (confectioner == null)
                return NotFound();

            confectioner.MinDiameter = request.MinDiameter;
            confectioner.MaxDiameter = request.MaxDiameter;
            confectioner.MinEta = request.MinETADays;
            confectioner.MaxEta = request.MaxETADays;
            confectioner.Fillings = request.Fillings;
            confectioner.DoImages = request.DoImages;
            confectioner.DoShapes = request.DoShapes;

            await _repository.Update(confectioner);
            return Ok(
                new ConfectionerSettingsResponse(
                    confectioner.Id,
                    confectioner.MinDiameter,
                    confectioner.MaxDiameter,
                    confectioner.MinEta,
                    confectioner.MaxEta,
                    confectioner.Fillings,
                    confectioner.DoImages,
                    confectioner.DoShapes
                )
            );
        }

        protected override Confectioner CreateModel(CreateConfectionerRequest dto)
        {
            return new Confectioner
            {
                UserId = dto.UserId,
                Description = dto.Description,
                Address = dto.Address,
                MinDiameter = dto.MinDiameter,
                MaxDiameter = dto.MaxDiameter,
                MinEta = dto.MinEta,
                MaxEta = dto.MaxEta,
                Fillings = dto.Fillings,
                DoImages = dto.DoImages,
                DoShapes = dto.DoShapes,
                Rating = dto.Rating,
            };
        }

        protected override void UpdateModel(Confectioner model, UpdateConfectionerRequest dto)
        {
            if (dto.UserId.HasValue)
                model.UserId = dto.UserId.Value;
            if (dto.Description is not null)
                model.Description = dto.Description;
            if (dto.Address is not null)
                model.Address = dto.Address;
            if (dto.MinDiameter.HasValue)
                model.MinDiameter = dto.MinDiameter.Value;
            if (dto.MaxDiameter.HasValue)
                model.MaxDiameter = dto.MaxDiameter.Value;
            if (dto.MinEta.HasValue)
                model.MinEta = dto.MinEta.Value;
            if (dto.MaxEta.HasValue)
                model.MaxEta = dto.MaxEta.Value;
            if (dto.Fillings is not null)
                model.Fillings = dto.Fillings;
            if (dto.DoImages.HasValue)
                model.DoImages = dto.DoImages.Value;
            if (dto.DoShapes.HasValue)
                model.DoShapes = dto.DoShapes.Value;
            if (dto.Rating.HasValue)
                model.Rating = dto.Rating.Value;
        }
    }
}
