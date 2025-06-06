using Caker.Dto;
using Caker.Models;
using Caker.Repositories;
using Caker.Services.CurrentUserService;
using Microsoft.AspNetCore.Mvc;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/confectioners")]
    public class ConfectionerController(
        ConfectionerRepository repository,
        ICurrentUserService currentUserService
    )
        : BaseController<
            Confectioner,
            ConfectionerResponse,
            CreateConfectionerRequest,
            UpdateConfectionerRequest
        >(repository, currentUserService)
    {
        private readonly ConfectionerRepository _repo = repository;
        private readonly ICurrentUserService _currUserService = currentUserService;

        [HttpGet("sorted/cakes")]
        public async Task<ActionResult<IEnumerable<ConfectionerResponse>>> GetSortedByCakesCount(
            [FromQuery] bool ascending = true
        )
        {
            var confectioners = await _repo.GetSortedByCakesCount(ascending);
            return Ok(confectioners.Select(c => c.ToDto()));
        }

        [HttpPost("sorted/cakes")]
        public async Task<ActionResult<IEnumerable<ConfectionerResponse>>> SearchSortedByCakesCount(
            [FromBody] SearchQuery query,
            [FromQuery] bool ascending = true
        )
        {
            var confectioners = await _repo.SearchSortedByCakesCount(ascending, query.Name);
            return Ok(confectioners.Select(c => c.ToDto()));
        }

        [HttpPost("search/name")]
        public async Task<ActionResult<IEnumerable<ConfectionerResponse>>> GetByName(
            [FromBody] SearchQuery query
        )
        {
            var confectioners = await _repo.SearchByName(query.Name);
            return Ok(confectioners.Select(c => c.ToDto()));
        }

        /// <summary>
        /// Get settings by id.
        /// </summary>
        [HttpGet("{id}/settings")]
        public async Task<ActionResult<ConfectionerSettingsResponse>> GetSettings(int id)
        {
            var confectioner = await _repo.GetById(id);
            if (confectioner == null)
                return NotFound();

            return GetSettings(confectioner);
        }

        /// <summary>
        /// Get settings by token from cookies.
        /// </summary>
        [HttpGet("self/settings")]
        public async Task<ActionResult<ConfectionerSettingsResponse>> GetSettings()
        {
            User? user_nullable;
            try
            {
                user_nullable = await _currUserService.GetUser();
                if (user_nullable == null)
                    return Unauthorized();
            }
            catch
            {
                return Unauthorized();
            }

            User user = user_nullable;

            var confectioner = user.Confectioner;
            if (confectioner == null)
                return Forbid();

            return GetSettings(confectioner);
        }

        private ActionResult<ConfectionerSettingsResponse> GetSettings(Confectioner confectioner) =>
            Ok(
                new ConfectionerSettingsResponse(
                    confectioner.Id,
                    confectioner.MinDiameter,
                    confectioner.MaxDiameter,
                    confectioner.MinEta,
                    confectioner.MaxEta,
                    confectioner.Fillings ?? [],
                    confectioner.DoImages,
                    confectioner.DoShapes,
                    confectioner.DoCustom
                )
            );

        /// <summary>
        /// Update settings by id.
        /// </summary>
        [HttpPut("{id}/settings")]
        public async Task<ActionResult<ConfectionerSettingsResponse>> UpdateSettings(
            int id,
            [FromBody] UpdateConfectionerSettingsRequest request
        )
        {
            var confectioner = await _repo.GetById(id);
            if (confectioner == null)
                return NotFound();

            return await UpdateSettings(confectioner, request);
        }

        /// <summary>
        /// Update settings by token from cookies.
        /// </summary>
        [HttpPut("self/settings")]
        public async Task<ActionResult<ConfectionerSettingsResponse>> UpdateSettings(
            [FromBody] UpdateConfectionerSettingsRequest request
        )
        {
            User? user_nullable;
            try
            {
                user_nullable = await _currUserService.GetUser();
                if (user_nullable == null)
                    return Unauthorized();
            }
            catch
            {
                return Unauthorized();
            }

            User user = user_nullable;

            var confectioner = user.Confectioner;
            if (confectioner == null)
                return Forbid();

            return await UpdateSettings(confectioner, request);
        }

        private async Task<ActionResult<ConfectionerSettingsResponse>> UpdateSettings(
            Confectioner confectioner,
            UpdateConfectionerSettingsRequest request
        )
        {
            confectioner.MinDiameter = request.MinDiameter;
            confectioner.MaxDiameter = request.MaxDiameter;
            confectioner.MinEta = request.MinETADays;
            confectioner.MaxEta = request.MaxETADays;
            confectioner.Fillings = request.Fillings;
            confectioner.DoImages = request.DoImages;
            confectioner.DoShapes = request.DoShapes;
            confectioner.DoCustom = request.DoCustom;

            await _repo.Update(confectioner);
            return Ok(
                new ConfectionerSettingsResponse(
                    confectioner.Id,
                    confectioner.MinDiameter,
                    confectioner.MaxDiameter,
                    confectioner.MinEta,
                    confectioner.MaxEta,
                    confectioner.Fillings ?? [],
                    confectioner.DoImages,
                    confectioner.DoShapes,
                    confectioner.DoCustom
                )
            );
        }

        [HttpGet("self/balance")]
        public async Task<ActionResult<ConfectionerBalanceResponse>> GetBalance()
        {
            User? user_nullable;
            try
            {
                user_nullable = await _currUserService.GetUser();
                if (user_nullable == null)
                    return Unauthorized();
            }
            catch
            {
                return Unauthorized();
            }

            User user = user_nullable;

            var confectioner = user.Confectioner;
            if (confectioner == null)
                return Forbid();

            return Ok(
                new ConfectionerBalanceResponse(
                    confectioner.BalanceAvailable,
                    confectioner.BalanceFreezed
                )
            );
        }

        [HttpPost("self/withdraw")]
        public async Task<ActionResult<ConfectionerBalanceResponse>> Withdraw(
            [FromBody] WithdrawRequest request
        )
        {
            User? user_nullable;
            try
            {
                user_nullable = await _currUserService.GetUser();
                if (user_nullable == null || user_nullable.Confectioner == null)
                    return Unauthorized();
            }
            catch
            {
                return Unauthorized();
            }
            User user = user_nullable;

            var confectioner = user.Confectioner;
            if (confectioner == null)
            {
                return Forbid();
            }

            if (request.Amount <= 0)
                return BadRequest("Amount must be positive.");

            if (confectioner.BalanceAvailable < request.Amount)
                return BadRequest("Insufficient available balance.");

            confectioner.BalanceAvailable -= request.Amount;

            await _repo.Update(confectioner);

            return Ok(
                new ConfectionerBalanceResponse(
                    confectioner.BalanceAvailable,
                    confectioner.BalanceFreezed
                )
            );
        }

        protected override Confectioner CreateModel(CreateConfectionerRequest dto)
        {
            return new Confectioner
            {
                UserId = dto.UserId,
                MinDiameter = dto.MinDiameter,
                MaxDiameter = dto.MaxDiameter,
                MinEta = dto.MinEta,
                MaxEta = dto.MaxEta,
                Fillings = dto.Fillings,
                DoImages = dto.DoImages,
                DoShapes = dto.DoShapes,
                DoCustom = dto.DoCustom,
                Rating = dto.Rating,
            };
        }

        protected override void UpdateModel(Confectioner model, UpdateConfectionerRequest dto)
        {
            if (dto.UserId.HasValue)
                model.UserId = dto.UserId.Value;
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
            if (dto.DoCustom.HasValue)
                model.DoShapes = dto.DoCustom.Value;
            if (dto.Rating.HasValue)
                model.Rating = dto.Rating.Value;
        }
    }
}
