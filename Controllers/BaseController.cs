using Caker.Models;
using Caker.Repositories;
using Microsoft.AspNetCore.Mvc;

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
                if (id != entity.Id)
                {
                    return BadRequest();
                }
                await _repository.Update(entity);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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
