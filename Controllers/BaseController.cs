using System.Reflection;
using System.Text.Json;
using Caker.Models;
using Caker.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Caker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<T>(BaseRepository<T> repository) : ControllerBase
        where T : BaseModel
    {
        protected readonly BaseRepository<T> _repository = repository;

        // GET api/[controller]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T>>> GetAll()
        {
            try
            {
                var result = await _repository.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/[controller]/id
        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetById(int id)
        {
            try
            {
                var result = await _repository.GetById(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/[controller]
        [HttpPost]
        public async Task<ActionResult<T>> Create([FromBody] T entity)
        {
            try
            {
                await _repository.Create(entity);
                return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(501, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/[controller]/id
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] T entity)
        {
            try
            {
                entity.Id = id;
                await _repository.Update(entity);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(501, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdate(int id, [FromBody] JsonElement partialEntity)
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
                return NotFound();

            // Use reflection or a DTO to map properties
            var entityType = typeof(T);
            foreach (var prop in partialEntity.EnumerateObject())
            {
                var propertyName = prop.Name;
                var propertyInfo = entityType.GetProperty(
                    propertyName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                );
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    var value = JsonSerializer.Deserialize(
                        prop.Value.GetRawText(),
                        propertyInfo.PropertyType
                    );
                    propertyInfo.SetValue(entity, value);
                }
            }

            await _repository.Update(entity);
            return NoContent();
        }

        // DELETE api/[controller]/id
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _repository.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
