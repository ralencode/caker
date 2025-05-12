using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Caker.Models;
using Caker.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<TModel, TDto, TCreateDto, TUpdateDto>(
        BaseRepository<TModel> repository
    ) : ControllerBase
        where TModel : BaseModel
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected readonly BaseRepository<TModel> _repository = repository;

        protected abstract TModel CreateModel(TCreateDto dto);
        protected abstract void UpdateModel(TModel model, TUpdateDto dto);
        protected abstract TDto ToDto(TModel model);

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            try
            {
                var result = await _repository.GetAll();
                return Ok(result.Select(ToDto));
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
            return entity == null ? NotFound() : Ok(ToDto(entity));
        }

        [HttpPost]
        public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreateDto dto)
        {
            try
            {
                var entity = CreateModel(dto);
                await _repository.Create(entity);
                return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDto(entity));
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(int id, [FromBody] TUpdateDto dto)
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
                return NotFound();

            UpdateModel(entity, dto);
            await _repository.Update(entity);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public virtual async Task<IActionResult> PartialUpdate(
            int id,
            [FromBody] JsonElement patchDoc
        )
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
                return NotFound();

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
            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            await _repository.Delete(id);
            return NoContent();
        }
    }
}
