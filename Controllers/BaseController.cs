using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Caker.Models;
using Caker.Models.Interfaces;
using Caker.Repositories;
using Caker.Services.CurrentUserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<TModel, TDto, TCreateDto, TUpdateDto>(
        BaseRepository<TModel> repository,
        ICurrentUserService currentUserService
    ) : ControllerBase
        where TModel : BaseModel, IDtoable<TDto>, IAccessibleBy
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected readonly BaseRepository<TModel> _repository = repository;
        protected readonly ICurrentUserService _currentUserService = currentUserService;

        protected bool CanCreate(TModel? model)
        {
            try
            {
                _currentUserService.GetUserId();
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected bool CanRead(TModel? model) => true;

        protected bool CanUpdate(TModel? model) => CheckPermissions(model);

        protected bool CanDelete(TModel? model) => CheckPermissions(model);

        protected abstract TModel CreateModel(TCreateDto dto);
        protected abstract void UpdateModel(TModel model, TUpdateDto dto);

        protected virtual bool CheckPermissions(TModel? model)
        {
            if (!_currentUserService.IsAuthorized())
                return false;

            return _currentUserService.IsAdmin()
                || (model?.AllowedUserIds.Contains(_currentUserService.GetUserId()) ?? true);
        }

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            if (!CanRead(null))
                return Forbid();

            try
            {
                var result = await _repository.GetAll();
                return Ok(result.Select(m => m.ToDto()));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TDto>> GetById(int id)
        {
            var entity = await _repository.GetById(id);

            if (!CanRead(entity))
                return Forbid();

            return entity == null ? NotFound() : Ok(entity.ToDto());
        }

        [Authorize]
        [HttpPost]
        public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreateDto dto)
        {
            try
            {
                var entity = CreateModel(dto);

                if (!CanCreate(entity))
                    return Forbid();

                await _repository.Create(entity);
                return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity.ToDto());
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult<TDto>> Update(int id, [FromBody] TUpdateDto dto)
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
                return NotFound();

            if (!CanUpdate(entity))
                return Forbid();

            UpdateModel(entity, dto);
            await _repository.Update(entity);
            return Ok(entity.ToDto());
        }

        [HttpPatch("{id}")]
        public virtual async Task<ActionResult<TDto>> PartialUpdate(
            int id,
            [FromBody] JsonElement patchDoc
        )
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
                return NotFound();

            if (!CanUpdate(entity))
                return Forbid();

            var modelType = typeof(TModel);
            var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in patchDoc.EnumerateObject())
            {
                var jsonPropertyName = prop.Name;

                // Find the model property with matching JsonPropertyName
                var property = properties.FirstOrDefault(p =>
                {
                    var attribute = p.GetCustomAttribute<JsonPropertyNameAttribute>();
                    return attribute?.Name == jsonPropertyName;
                });

                // Fallback to property name match
                if (property == null)
                {
                    property = modelType.GetProperty(
                        jsonPropertyName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                    );
                }

                if (property != null && property.CanWrite)
                {
                    var value = JsonSerializer.Deserialize(
                        prop.Value.GetRawText(),
                        property.PropertyType
                    );
                    property.SetValue(entity, value);
                }
            }

            await _repository.Update(entity);
            return Ok(entity.ToDto());
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!CanDelete(await _repository.GetById(id)))
                return Forbid();

            await _repository.Delete(id);
            return NoContent();
        }
    }
}
